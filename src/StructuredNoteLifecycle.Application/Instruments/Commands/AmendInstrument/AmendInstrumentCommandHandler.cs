using MediatR;
using StructuredNoteLifecycle.Application.Common.Interfaces;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;
using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.AmendInstrument;

public sealed class AmendInstrumentCommandHandler(
    IInstrumentRepository repository,
    IUnitOfWork unitOfWork,
    IOutboxService outbox)
    : IRequestHandler<AmendInstrumentCommand, Result>
{
    public async Task<Result> Handle(AmendInstrumentCommand cmd, CancellationToken ct)
    {
        var instrument = await repository.GetByIdAsync(new InstrumentId(cmd.InstrumentId), ct);
        if (instrument is null)
            return Result.Failure($"Instrument {cmd.InstrumentId} not found.");

        instrument.Amend(cmd.ExpectedCurrentVersion, cmd.AmendmentReason, cmd.AmendedBy,
            cmd.Changes, cmd.AmendmentType);

        await repository.UpdateAsync(instrument, ct);
        await outbox.PublishAsync(instrument.DomainEvents, ct);
        instrument.ClearDomainEvents();
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
