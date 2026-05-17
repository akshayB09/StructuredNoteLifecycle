using Microsoft.EntityFrameworkCore;
using StructuredNoteLifecycle.Application.Common.Interfaces;
using StructuredNoteLifecycle.Infrastructure.Persistence;

namespace StructuredNoteLifecycle.Infrastructure.Idempotency;

public sealed class IdempotencyStore(ApplicationDbContext db) : IIdempotencyStore
{
    public Task<bool> HasBeenProcessedAsync(Guid messageId, CancellationToken ct = default) =>
        db.ProcessedMessages.AnyAsync(m => m.MessageId == messageId, ct);

    public async Task MarkProcessedAsync(Guid messageId, string eventType, CancellationToken ct = default) =>
        await db.ProcessedMessages.AddAsync(new ProcessedMessage
        {
            MessageId = messageId,
            EventType = eventType
        }, ct);
}
