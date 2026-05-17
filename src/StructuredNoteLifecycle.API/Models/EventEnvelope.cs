namespace StructuredNoteLifecycle.API.Models;

public sealed record EventEnvelope<TPayload>(
    Guid MessageId,
    Guid CorrelationId,
    string EventType,
    string EventSchemaVersion,
    DateTimeOffset Timestamp,
    string Source,
    TPayload Payload);
