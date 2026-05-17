using MediatR;
using StructuredNoteLifecycle.Application.Common.Interfaces;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;
using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.ApplyRedemptionFixing;

public sealed class ApplyRedemptionFixingCommandHandler(
    IInstrumentRepository repository,
    IUnitOfWork unitOfWork,
    IOutboxService outbox)
    : IRequestHandler<ApplyRedemptionFixingCommand, Result>
{
    public async Task<Result> Handle(ApplyRedemptionFixingCommand cmd, CancellationToken ct)
    {
        var instrument = await repository.GetByIdAsync(new InstrumentId(cmd.InstrumentId), ct);
        if (instrument is null)
            return Result.Failure($"Instrument {cmd.InstrumentId} not found.");

        var fixings = cmd.Fixings
            .Select(f => new UnderlyingFixing(f.UnderlyingId, f.InitialLevel, f.ObservedLevel))
            .ToList();

        var terms = instrument.Terms;

        if (cmd.ObservationType == ObservationType.Autocall)
        {
            instrument.ApplyAutocallFixing(cmd.ObservationDate, cmd.SequenceNumber, fixings,
                terms.PayoffDefinition.Barriers.AutocallBarrier,
                terms.PayoffDefinition.CouponSchedule.FirstOrDefault()?.Rate ?? 0m);
        }
        else
        {
            var knockIn = cmd.KnockInHistory!;
            instrument.ApplyFinalMaturityFixing(cmd.ObservationDate, fixings,
                knockIn.HasBreached, terms.PayoffDefinition.Barriers.KnockIn,
                cmd.PhysicalSettlement,
                terms.PayoffDefinition.CouponSchedule.FirstOrDefault()?.Rate ?? 0m);
        }

        await repository.UpdateAsync(instrument, ct);
        await outbox.PublishAsync(instrument.DomainEvents, ct);
        instrument.ClearDomainEvents();
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
