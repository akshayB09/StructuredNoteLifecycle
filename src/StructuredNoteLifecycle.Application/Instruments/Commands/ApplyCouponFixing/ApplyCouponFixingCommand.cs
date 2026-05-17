using MediatR;
using StructuredNoteLifecycle.Application.Common.Behaviors;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;
using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.ApplyCouponFixing;

public sealed record FixingEntry(
    string UnderlyingId,
    decimal InitialLevel,
    decimal ObservedLevel,
    string Source,
    bool IsOfficial);

public sealed record ApplyCouponFixingCommand(
    Guid MessageId,
    Guid CorrelationId,
    string Source,
    Guid InstrumentId,
    DateOnly ObservationDate,
    int SequenceNumber,
    IReadOnlyList<FixingEntry> Fixings) : IRequest<Result>, IIdempotentCommand
{
    public string EventType => "CouponFixingEvent";
}
