using FluentValidation;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.ApplyCouponFixing;

public sealed class ApplyCouponFixingValidator : AbstractValidator<ApplyCouponFixingCommand>
{
    public ApplyCouponFixingValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.InstrumentId).NotEmpty();
        RuleFor(x => x.SequenceNumber).GreaterThan(0);
        RuleFor(x => x.Fixings).NotEmpty();
        RuleForEach(x => x.Fixings).ChildRules(f =>
        {
            f.RuleFor(x => x.UnderlyingId).NotEmpty();
            f.RuleFor(x => x.InitialLevel).GreaterThan(0);
            f.RuleFor(x => x.ObservedLevel).GreaterThan(0);
            f.RuleFor(x => x.IsOfficial).Equal(true).WithMessage("Only official fixings are accepted.");
        });
    }
}
