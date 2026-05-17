using FluentValidation;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.ConfirmTradeSettle;

public sealed class ConfirmTradeSettleValidator : AbstractValidator<ConfirmTradeSettleCommand>
{
    private static readonly string[] ValidStatuses = ["SETTLED", "PARTIAL", "FAILED"];

    public ConfirmTradeSettleValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.InstrumentId).NotEmpty();
        RuleFor(x => x.TradeId).NotEmpty();
        RuleFor(x => x.SettlementRef).NotEmpty();
        RuleFor(x => x.SettlementStatus).Must(s => ValidStatuses.Contains(s))
            .WithMessage("SettlementStatus must be SETTLED, PARTIAL, or FAILED.");
        RuleFor(x => x.SettledCash).GreaterThan(0);
        RuleFor(x => x.SettlementCurrency).NotEmpty().Length(3);
    }
}
