using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Domain.Aggregates.Instruments.DomainEvents;

public sealed record InstrumentAmendedDomainEvent(
    InstrumentId InstrumentId,
    int NewVersion,
    string AmendmentReason,
    string AmendedBy,
    IReadOnlyDictionary<string, object?> Changes,
    DateTimeOffset OccurredOn) : IDomainEvent;
