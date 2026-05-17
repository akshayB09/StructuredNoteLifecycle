using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Domain.Aggregates.Instruments.DomainEvents;

public sealed record CouponDeterminedDomainEvent(
    InstrumentId InstrumentId,
    int SequenceNumber,
    DateOnly ObservationDate,
    DateOnly PaymentDate,
    decimal CouponAmount,
    bool IncludesMemoryRecapture,
    DateTimeOffset OccurredOn) : IDomainEvent;

public sealed record CouponSkippedDomainEvent(
    InstrumentId InstrumentId,
    int SequenceNumber,
    DateOnly ObservationDate,
    bool MemoryPending,
    DateTimeOffset OccurredOn) : IDomainEvent;
