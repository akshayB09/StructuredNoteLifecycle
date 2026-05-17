using MediatR;
using StructuredNoteLifecycle.Application.Common.Behaviors;
using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.ConfirmTradeSettle;

public sealed record ConfirmTradeSettleCommand(
    Guid MessageId,
    Guid CorrelationId,
    string Source,
    Guid InstrumentId,
    string TradeId,
    string SettlementRef,
    DateOnly SettlementDate,
    DateOnly ActualSettlementDate,
    string SettlementStatus,
    string DeliveryType,
    decimal SettledQuantity,
    decimal SettledCash,
    string SettlementCurrency,
    string CustodianRef) : IRequest<Result>, IIdempotentCommand
{
    public string EventType => "ConfirmTradeSettle";
}
