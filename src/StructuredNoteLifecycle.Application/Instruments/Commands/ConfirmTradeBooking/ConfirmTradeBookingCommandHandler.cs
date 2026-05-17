using MediatR;
using StructuredNoteLifecycle.Application.Common.Interfaces;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;
using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.ConfirmTradeBooking;

public sealed class ConfirmTradeBookingCommandHandler(
    IInstrumentRepository repository,
    IUnitOfWork unitOfWork,
    IOutboxService outbox)
    : IRequestHandler<ConfirmTradeBookingCommand, Result>
{
    public async Task<Result> Handle(ConfirmTradeBookingCommand cmd, CancellationToken ct)
    {
        if (cmd.MatchStatus == "DISPUTED")
            return Result.Failure("ConfirmationBreak: trade is DISPUTED — ops intervention required.");

        var instrument = await repository.GetByIdAsync(new InstrumentId(cmd.InstrumentId), ct);
        if (instrument is null)
            return Result.Failure($"Instrument {cmd.InstrumentId} not found.");

        instrument.ConfirmBooking(cmd.TradeId, cmd.ExternalConfirmRef, cmd.ConfirmationType);

        await repository.UpdateAsync(instrument, ct);
        await outbox.PublishAsync(instrument.DomainEvents, ct);
        instrument.ClearDomainEvents();
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
