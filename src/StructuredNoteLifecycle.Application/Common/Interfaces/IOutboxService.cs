using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Common.Interfaces;

public interface IOutboxService
{
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default);
}
