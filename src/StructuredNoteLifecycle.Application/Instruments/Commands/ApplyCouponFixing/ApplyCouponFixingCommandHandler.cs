using MediatR;
using StructuredNoteLifecycle.Application.Common.Interfaces;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;
using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.ApplyCouponFixing;

public sealed class ApplyCouponFixingCommandHandler(
    IInstrumentRepository repository,
    IUnitOfWork unitOfWork,
    IOutboxService outbox)
    : IRequestHandler<ApplyCouponFixingCommand, Result>
{
    public async Task<Result> Handle(ApplyCouponFixingCommand cmd, CancellationToken ct)
    {
        var instrument = await repository.GetByIdAsync(new InstrumentId(cmd.InstrumentId), ct);
        if (instrument is null)
            return Result.Failure($"Instrument {cmd.InstrumentId} not found.");

        var fixings = cmd.Fixings
            .Select(f => new UnderlyingFixing(f.UnderlyingId, f.InitialLevel, f.ObservedLevel))
            .ToList();

        var terms = instrument.Terms;
        var couponEntry = terms.PayoffDefinition.CouponSchedule
            .FirstOrDefault(c => c.ObservationDate == cmd.ObservationDate);

        if (couponEntry is null)
            return Result.Failure($"No coupon schedule entry found for observation date {cmd.ObservationDate}.");

        instrument.ApplyCouponFixing(cmd.ObservationDate, cmd.SequenceNumber, fixings,
            terms.PayoffDefinition.Barriers.CouponBarrier, couponEntry.Rate);

        await repository.UpdateAsync(instrument, ct);
        await outbox.PublishAsync(instrument.DomainEvents, ct);
        instrument.ClearDomainEvents();
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
