using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Domain.Aggregates.Instruments.DomainEvents;

public sealed record TradeBookingConfirmedDomainEvent(
    InstrumentId InstrumentId,
    string TradeId,
    string ExternalConfirmRef,
    string ConfirmationType,
    DateTimeOffset OccurredOn) : IDomainEvent;
