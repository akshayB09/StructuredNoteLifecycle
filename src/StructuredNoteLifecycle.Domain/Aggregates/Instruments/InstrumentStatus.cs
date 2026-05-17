namespace StructuredNoteLifecycle.Domain.Aggregates.Instruments;

public enum InstrumentStatus
{
    Booked,
    BookingConfirmed,
    Live,
    Redeemed,
    Matured,
    Cancelled,
    SettlementFailed,
    PartiallySettled
}
