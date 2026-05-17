using FluentValidation;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.ApplyRedemptionFixing;

public sealed class ApplyRedemptionFixingValidator : AbstractValidator<ApplyRedemptionFixingCommand>
{
    public ApplyRedemptionFixingValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.InstrumentId).NotEmpty();
        RuleFor(x => x.SequenceNumber).GreaterThan(0);
        RuleFor(x => x.Fixings).NotEmpty();
        RuleFor(x => x.KnockInHistory).NotNull()
            .When(x => x.ObservationType == ObservationType.FinalMaturity)
            .WithMessage("KnockInHistory is required for final maturity fixing.");
    }
}
