using StructuredNoteLifecycle.Domain.Aggregates.Instruments.DomainEvents;
using StructuredNoteLifecycle.Domain.Common;
using StructuredNoteLifecycle.Domain.Exceptions;

namespace StructuredNoteLifecycle.Domain.Aggregates.Instruments;

public sealed class Instrument : AggregateRoot
{
    private readonly List<CouponPeriod> _couponPeriods = [];
    private readonly List<InstrumentSnapshot> _snapshots = [];

    public InstrumentId Id { get; private set; } = null!;
    public int Version { get; private set; }
    public InstrumentStatus Status { get; private set; }
    public InstrumentTerms Terms { get; private set; } = null!;
    public string? TradeId { get; private set; }
    public string? ExternalConfirmRef { get; private set; }
    public string? SettlementRef { get; private set; }
    public bool KnockInOccurred { get; private set; }

    public IReadOnlyList<CouponPeriod> CouponPeriods => _couponPeriods.AsReadOnly();
    public IReadOnlyList<InstrumentSnapshot> Snapshots => _snapshots.AsReadOnly();

    private Instrument() { }

    // ── Factory: Book ─────────────────────────────────────────────────────────

    public static Instrument Book(
        string orderId, string? quoteId, string counterpartyId,
        DateOnly tradeDate, DateOnly valueDate, DateOnly issueDate, DateOnly maturityDate,
        decimal notional, string currency, string issuerId, string productType, string? isin,
        PayoffDefinition payoffDefinition, EconomicTerms economicTerms)
    {
        var instrument = new Instrument
        {
            Id = InstrumentId.New(),
            Version = 1,
            Status = InstrumentStatus.Booked,
            Terms = new InstrumentTerms(orderId, quoteId, counterpartyId, tradeDate, valueDate,
                issueDate, maturityDate, notional, currency, issuerId, productType, isin,
                payoffDefinition, economicTerms)
        };

        instrument.InitialiseCouponPeriods();

        instrument.Raise(new InstrumentBookedDomainEvent(
            instrument.Id, orderId, counterpartyId, tradeDate,
            notional, currency, productType, DateTimeOffset.UtcNow));

        return instrument;
    }

    // ── Amend ─────────────────────────────────────────────────────────────────

    public void Amend(
        int expectedVersion, string amendmentReason, string amendedBy,
        IReadOnlyDictionary<string, object?> changes, AmendmentType amendmentType)
    {
        EnsureNotTerminal();

        if (Version != expectedVersion)
            throw new DomainException($"Concurrency conflict: expected version {expectedVersion}, current is {Version}.");

        if (amendmentType == AmendmentType.CancelAndRebook)
            throw new DomainException("This change requires cancel-and-rebook, not an amendment.");

        if (Status == InstrumentStatus.Live && amendmentType != AmendmentType.NonEconomic)
            throw new DomainException("Post-settlement: only non-economic amendments are allowed.");

        _snapshots.Add(new InstrumentSnapshot(Version, Terms));
        Version++;

        Raise(new InstrumentAmendedDomainEvent(Id, Version, amendmentReason, amendedBy, changes, DateTimeOffset.UtcNow));
    }

    // ── Confirm Booking ───────────────────────────────────────────────────────

    public void ConfirmBooking(string tradeId, string externalConfirmRef, string confirmationType)
    {
        if (Status != InstrumentStatus.Booked)
            throw new DomainException($"ConfirmTradeBooking invalid from state {Status}.");

        TradeId = tradeId;
        ExternalConfirmRef = externalConfirmRef;
        Status = InstrumentStatus.BookingConfirmed;

        Raise(new TradeBookingConfirmedDomainEvent(Id, tradeId, externalConfirmRef, confirmationType, DateTimeOffset.UtcNow));
    }

    // ── Confirm Settlement ────────────────────────────────────────────────────

    public void ConfirmSettlement(
        string tradeId, string settlementRef, DateOnly settlementDate,
        decimal settledCash, string settlementCurrency, string settlementStatus)
    {
        if (Status != InstrumentStatus.BookingConfirmed)
            throw new DomainException($"ConfirmTradeSettle invalid from state {Status}.");

        SettlementRef = settlementRef;

        Status = settlementStatus switch
        {
            "SETTLED" => InstrumentStatus.Live,
            "FAILED" => InstrumentStatus.SettlementFailed,
            "PARTIAL" => InstrumentStatus.PartiallySettled,
            _ => throw new DomainException($"Unknown settlement status: {settlementStatus}")
        };

        if (Status != InstrumentStatus.Live) return;

        Raise(new TradeSettledDomainEvent(Id, tradeId, settlementRef, settlementDate,
            settledCash, settlementCurrency, DateTimeOffset.UtcNow));
    }

    // ── Coupon Fixing ─────────────────────────────────────────────────────────

    public void ApplyCouponFixing(
        DateOnly observationDate, int sequenceNumber,
        IReadOnlyList<UnderlyingFixing> fixings, decimal couponBarrier, decimal couponRate)
    {
        if (Status != InstrumentStatus.Live)
            throw new DomainException($"CouponFixingEvent invalid from state {Status}.");

        var period = _couponPeriods.FirstOrDefault(p => p.SequenceNumber == sequenceNumber)
            ?? throw new DomainException($"Coupon period {sequenceNumber} not found.");

        if (period.Status is CouponPeriodStatus.Determined or CouponPeriodStatus.Paid)
            throw new DomainException($"Coupon period {sequenceNumber} already determined (idempotency guard).");

        var worstPerformance = fixings.Min(f => f.Performance);
        var paymentDate = Terms.PayoffDefinition.CouponSchedule
            .First(c => c.ObservationDate == observationDate).PaymentDate;

        if (worstPerformance >= couponBarrier)
        {
            var couponAmount = Terms.Notional * couponRate;
            var memoryAmount = 0m;
            var hasMemory = false;

            if (Terms.PayoffDefinition.MemoryFeature)
            {
                var pendingPeriods = _couponPeriods.Where(p => p.Status == CouponPeriodStatus.PendingMemory).ToList();
                memoryAmount = pendingPeriods.Sum(p => Terms.Notional * couponRate);
                foreach (var p in pendingPeriods) p.MarkPaid(Terms.Notional * couponRate);
                hasMemory = pendingPeriods.Count > 0;
            }

            var totalAmount = couponAmount + memoryAmount;
            period.MarkDetermined(totalAmount);

            Raise(new CouponDeterminedDomainEvent(Id, sequenceNumber, observationDate, paymentDate,
                totalAmount, hasMemory, DateTimeOffset.UtcNow));
        }
        else
        {
            if (Terms.PayoffDefinition.MemoryFeature)
                period.MarkPendingMemory();
            else
                period.MarkSkipped();

            Raise(new CouponSkippedDomainEvent(Id, sequenceNumber, observationDate,
                Terms.PayoffDefinition.MemoryFeature, DateTimeOffset.UtcNow));
        }
    }

    // ── Redemption Fixing ─────────────────────────────────────────────────────

    public void ApplyAutocallFixing(
        DateOnly observationDate, int sequenceNumber,
        IReadOnlyList<UnderlyingFixing> fixings, decimal autocallBarrier, decimal couponRate)
    {
        if (Status != InstrumentStatus.Live)
            throw new DomainException($"RedemptionFixingEvent invalid from state {Status}.");

        var worstPerformance = fixings.Min(f => f.Performance);

        if (worstPerformance >= autocallBarrier)
        {
            var pendingMemory = Terms.PayoffDefinition.MemoryFeature
                ? _couponPeriods.Where(p => p.Status == CouponPeriodStatus.PendingMemory)
                                .Sum(p => Terms.Notional * couponRate)
                : 0m;

            var totalPayout = Terms.Notional + (Terms.Notional * couponRate) + pendingMemory;
            Status = InstrumentStatus.Redeemed;

            Raise(new AutocallTriggeredDomainEvent(Id, observationDate, sequenceNumber, totalPayout, DateTimeOffset.UtcNow));
        }
        else
        {
            Raise(new AutocallNotTriggeredDomainEvent(Id, observationDate, sequenceNumber, DateTimeOffset.UtcNow));
        }
    }

    public void ApplyFinalMaturityFixing(
        DateOnly observationDate, IReadOnlyList<UnderlyingFixing> fixings,
        bool knockInOccurred, decimal strikePerformance, bool physicalSettlement, decimal couponRate)
    {
        if (Status != InstrumentStatus.Live)
            throw new DomainException($"Final maturity fixing invalid from state {Status}.");

        var worstPerformance = fixings.Min(f => f.Performance);
        KnockInOccurred = knockInOccurred;

        decimal? redemptionAmount;
        RedemptionType redemptionType;

        if (!knockInOccurred || worstPerformance >= strikePerformance)
        {
            redemptionAmount = Terms.Notional;
            redemptionType = RedemptionType.Cash;
        }
        else if (physicalSettlement)
        {
            redemptionAmount = null;
            redemptionType = RedemptionType.Physical;
        }
        else
        {
            redemptionAmount = Terms.Notional * worstPerformance;
            redemptionType = RedemptionType.CashAtRisk;
        }

        Status = InstrumentStatus.Matured;

        Raise(new FinalRedemptionDeterminedDomainEvent(Id, observationDate, redemptionAmount,
            redemptionType, knockInOccurred, DateTimeOffset.UtcNow));
    }

    // ── Cancel ────────────────────────────────────────────────────────────────

    public void Cancel()
    {
        if (Status is not (InstrumentStatus.Booked or InstrumentStatus.BookingConfirmed))
            throw new DomainException($"Cancel invalid from state {Status}.");

        Status = InstrumentStatus.Cancelled;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void EnsureNotTerminal()
    {
        if (Status is InstrumentStatus.Matured or InstrumentStatus.Redeemed or InstrumentStatus.Cancelled)
            throw new DomainException($"Instrument is in terminal state {Status} and cannot be modified.");
    }

    private void InitialiseCouponPeriods()
    {
        var schedule = Terms.PayoffDefinition.CouponSchedule;
        for (var i = 0; i < schedule.Count; i++)
            _couponPeriods.Add(CouponPeriod.Create(i + 1, schedule[i].ObservationDate, schedule[i].PaymentDate));
    }
}

public sealed record UnderlyingFixing(string UnderlyingId, decimal InitialLevel, decimal ObservedLevel)
{
    public decimal Performance => ObservedLevel / InitialLevel;
}

public sealed record InstrumentSnapshot(int Version, InstrumentTerms Terms);

public enum AmendmentType
{
    NonEconomic,
    Economic,
    CancelAndRebook
}
