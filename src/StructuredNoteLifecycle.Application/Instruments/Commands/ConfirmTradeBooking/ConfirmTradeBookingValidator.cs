using FluentValidation;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.ConfirmTradeBooking;

public sealed class ConfirmTradeBookingValidator : AbstractValidator<ConfirmTradeBookingCommand>
{
    private static readonly string[] ValidMatchStatuses = ["MATCHED", "AFFIRMED", "DISPUTED"];

    public ConfirmTradeBookingValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.InstrumentId).NotEmpty();
        RuleFor(x => x.TradeId).NotEmpty();
        RuleFor(x => x.ExternalConfirmRef).NotEmpty();
        RuleFor(x => x.MatchStatus).Must(s => ValidMatchStatuses.Contains(s))
            .WithMessage("MatchStatus must be MATCHED, AFFIRMED, or DISPUTED.");
        RuleFor(x => x.ConfirmationTimestamp).LessThanOrEqualTo(_ => DateTimeOffset.UtcNow)
            .WithMessage("ConfirmationTimestamp cannot be in the future.");
    }
}
