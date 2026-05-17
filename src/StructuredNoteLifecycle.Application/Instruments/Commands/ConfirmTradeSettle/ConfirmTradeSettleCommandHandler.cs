using MediatR;
using StructuredNoteLifecycle.Application.Common.Interfaces;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;
using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.ConfirmTradeSettle;

public sealed class ConfirmTradeSettleCommandHandler(
    IInstrumentRepository repository,
    IUnitOfWork unitOfWork,
    IOutboxService outbox)
    : IRequestHandler<ConfirmTradeSettleCommand, Result>
{
    public async Task<Result> Handle(ConfirmTradeSettleCommand cmd, CancellationToken ct)
    {
        var instrument = await repository.GetByIdAsync(new InstrumentId(cmd.InstrumentId), ct);
        if (instrument is null)
            return Result.Failure($"Instrument {cmd.InstrumentId} not found.");

        instrument.ConfirmSettlement(cmd.TradeId, cmd.SettlementRef, cmd.SettlementDate,
            cmd.SettledCash, cmd.SettlementCurrency, cmd.SettlementStatus);

        await repository.UpdateAsync(instrument, ct);
        await outbox.PublishAsync(instrument.DomainEvents, ct);
        instrument.ClearDomainEvents();
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
