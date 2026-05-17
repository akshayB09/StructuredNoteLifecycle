using FluentValidation;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.AmendInstrument;

public sealed class AmendInstrumentValidator : AbstractValidator<AmendInstrumentCommand>
{
    public AmendInstrumentValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.InstrumentId).NotEmpty();
        RuleFor(x => x.ExpectedCurrentVersion).GreaterThan(0);
        RuleFor(x => x.AmendmentReason).NotEmpty();
        RuleFor(x => x.AmendedBy).NotEmpty();
        RuleFor(x => x.Changes).NotEmpty();
    }
}
