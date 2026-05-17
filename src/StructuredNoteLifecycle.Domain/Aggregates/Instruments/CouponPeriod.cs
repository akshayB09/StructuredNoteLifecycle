namespace StructuredNoteLifecycle.Domain.Aggregates.Instruments;

public enum CouponPeriodStatus
{
    Scheduled,
    Determined,
    Paid,
    Skipped,
    PendingMemory
}

public sealed class CouponPeriod
{
    public Guid PeriodId { get; private set; } = Guid.NewGuid();
    public int SequenceNumber { get; private set; }
    public DateOnly ObservationDate { get; private set; }
    public DateOnly PaymentDate { get; private set; }
    public CouponPeriodStatus Status { get; private set; } = CouponPeriodStatus.Scheduled;
    public decimal? DeterminedAmount { get; private set; }
    public decimal? PaidAmount { get; private set; }

    private CouponPeriod() { }

    public static CouponPeriod Create(int sequenceNumber, DateOnly observationDate, DateOnly paymentDate) =>
        new() { SequenceNumber = sequenceNumber, ObservationDate = observationDate, PaymentDate = paymentDate };

    public void MarkDetermined(decimal amount)
    {
        Status = CouponPeriodStatus.Determined;
        DeterminedAmount = amount;
    }

    public void MarkPaid(decimal amount)
    {
        Status = CouponPeriodStatus.Paid;
        PaidAmount = amount;
    }

    public void MarkSkipped() => Status = CouponPeriodStatus.Skipped;

    public void MarkPendingMemory() => Status = CouponPeriodStatus.PendingMemory;
}
