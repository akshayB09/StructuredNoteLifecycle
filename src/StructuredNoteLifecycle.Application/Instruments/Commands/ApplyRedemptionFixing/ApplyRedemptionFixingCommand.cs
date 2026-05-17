using MediatR;
using StructuredNoteLifecycle.Application.Common.Behaviors;
using StructuredNoteLifecycle.Application.Instruments.Commands.ApplyCouponFixing;
using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.ApplyRedemptionFixing;

public enum ObservationType { Autocall, FinalMaturity }

public sealed record KnockInHistory(bool HasBreached, DateOnly? BreachDate, string? BreachUnderlying);

public sealed record ApplyRedemptionFixingCommand(
    Guid MessageId,
    Guid CorrelationId,
    string Source,
    Guid InstrumentId,
    DateOnly ObservationDate,
    ObservationType ObservationType,
    int SequenceNumber,
    IReadOnlyList<FixingEntry> Fixings,
    KnockInHistory? KnockInHistory,
    bool PhysicalSettlement = false) : IRequest<Result>, IIdempotentCommand
{
    public string EventType => "RedemptionFixingEvent";
}
