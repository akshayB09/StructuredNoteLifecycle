using System.Text.Json;
using StructuredNoteLifecycle.Application.Common.Interfaces;
using StructuredNoteLifecycle.Domain.Common;
using StructuredNoteLifecycle.Infrastructure.Persistence;

namespace StructuredNoteLifecycle.Infrastructure.Outbox;

public sealed class OutboxService(ApplicationDbContext db) : IOutboxService
{
    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default)
    {
        var messages = domainEvents.Select(e => new OutboxMessage
        {
            EventType = e.GetType().Name,
            Payload = JsonSerializer.Serialize(e, e.GetType())
        });

        await db.OutboxMessages.AddRangeAsync(messages, ct);
    }
}
