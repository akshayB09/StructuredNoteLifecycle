using MediatR;
using StructuredNoteLifecycle.Application.Common.Behaviors;
using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.ConfirmTradeBooking;

public sealed record ConfirmTradeBookingCommand(
    Guid MessageId,
    Guid CorrelationId,
    string Source,
    Guid InstrumentId,
    string TradeId,
    string ExternalConfirmRef,
    string ConfirmationType,
    string MatchStatus,
    string ConfirmedBy,
    DateTimeOffset ConfirmationTimestamp) : IRequest<Result>, IIdempotentCommand
{
    public string EventType => "ConfirmTradeBooking";
}
