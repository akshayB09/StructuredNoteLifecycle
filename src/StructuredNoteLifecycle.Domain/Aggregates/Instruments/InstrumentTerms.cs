namespace StructuredNoteLifecycle.Domain.Aggregates.Instruments;

public sealed record InstrumentTerms(
    string OrderId,
    string? QuoteId,
    string CounterpartyId,
    DateOnly TradeDate,
    DateOnly ValueDate,
    DateOnly IssueDate,
    DateOnly MaturityDate,
    decimal Notional,
    string Currency,
    string IssuerId,
    string ProductType,
    string? Isin,
    PayoffDefinition PayoffDefinition,
    EconomicTerms EconomicTerms);

public sealed record PayoffDefinition(
    IReadOnlyList<UnderlyingWeight> Underlyings,
    DateOnly StrikeDate,
    BarrierLevels Barriers,
    IReadOnlyList<CouponScheduleEntry> CouponSchedule,
    IReadOnlyList<DateOnly> AutocallObservationDates,
    string DayCount,
    bool MemoryFeature);

public sealed record UnderlyingWeight(string UnderlyingId, decimal Weight);

public sealed record BarrierLevels(decimal KnockIn, decimal CouponBarrier, decimal AutocallBarrier);

public sealed record CouponScheduleEntry(DateOnly ObservationDate, DateOnly PaymentDate, decimal Rate);

public sealed record EconomicTerms(decimal Price, decimal? Yield, decimal UpfrontFee, string? HedgeRef);
