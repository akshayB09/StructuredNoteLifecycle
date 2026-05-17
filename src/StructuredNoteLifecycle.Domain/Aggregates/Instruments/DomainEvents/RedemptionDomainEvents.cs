using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Domain.Aggregates.Instruments.DomainEvents;

public sealed record AutocallTriggeredDomainEvent(
    InstrumentId InstrumentId,
    DateOnly ObservationDate,
    int SequenceNumber,
    decimal RedemptionAmount,
    DateTimeOffset OccurredOn) : IDomainEvent;

public sealed record AutocallNotTriggeredDomainEvent(
    InstrumentId InstrumentId,
    DateOnly ObservationDate,
    int SequenceNumber,
    DateTimeOffset OccurredOn) : IDomainEvent;

public enum RedemptionType { Cash, Physical, CashAtRisk }

public sealed record FinalRedemptionDeterminedDomainEvent(
    InstrumentId InstrumentId,
    DateOnly ObservationDate,
    decimal? RedemptionAmount,
    RedemptionType RedemptionType,
    bool KnockInOccurred,
    DateTimeOffset OccurredOn) : IDomainEvent;
