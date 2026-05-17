namespace StructuredNoteLifecycle.Infrastructure.Idempotency;

public sealed class ProcessedMessage
{
    public Guid MessageId { get; init; }
    public string EventType { get; init; } = string.Empty;
    public DateTimeOffset ProcessedAt { get; init; } = DateTimeOffset.UtcNow;
}
