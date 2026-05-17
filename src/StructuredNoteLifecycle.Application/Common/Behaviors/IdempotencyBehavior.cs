using MediatR;
using StructuredNoteLifecycle.Application.Common.Interfaces;

namespace StructuredNoteLifecycle.Application.Common.Behaviors;

public interface IIdempotentCommand
{
    Guid MessageId { get; }
    string EventType { get; }
}

public sealed class IdempotencyBehavior<TRequest, TResponse>(IIdempotencyStore store)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentCommand
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (await store.HasBeenProcessedAsync(request.MessageId, ct))
            return default!;

        var response = await next(ct);

        await store.MarkProcessedAsync(request.MessageId, request.EventType, ct);

        return response;
    }
}
