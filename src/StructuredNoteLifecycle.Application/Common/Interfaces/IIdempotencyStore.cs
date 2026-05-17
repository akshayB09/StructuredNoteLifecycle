namespace StructuredNoteLifecycle.Application.Common.Interfaces;

public interface IIdempotencyStore
{
    Task<bool> HasBeenProcessedAsync(Guid messageId, CancellationToken ct = default);
    Task MarkProcessedAsync(Guid messageId, string eventType, CancellationToken ct = default);
}
