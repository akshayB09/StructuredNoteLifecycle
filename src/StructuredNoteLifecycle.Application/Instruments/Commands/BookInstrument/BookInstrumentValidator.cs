using FluentValidation;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.BookInstrument;

public sealed class BookInstrumentValidator : AbstractValidator<BookInstrumentCommand>
{
    public BookInstrumentValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.CounterpartyId).NotEmpty();
        RuleFor(x => x.IssuerId).NotEmpty();
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.ProductType).NotEmpty();
        RuleFor(x => x.Notional).GreaterThan(0);
        RuleFor(x => x.PayoffDefinition).NotNull();
        RuleFor(x => x.PayoffDefinition.Underlyings).NotEmpty().When(x => x.PayoffDefinition is not null);
        RuleFor(x => x.PayoffDefinition.CouponSchedule).NotEmpty().When(x => x.PayoffDefinition is not null);

        RuleFor(x => x).Must(HaveCoherentDates)
            .WithMessage("Dates must satisfy: TradeDate <= ValueDate <= IssueDate < MaturityDate.");
    }

    private static bool HaveCoherentDates(BookInstrumentCommand cmd) =>
        cmd.TradeDate <= cmd.ValueDate &&
        cmd.ValueDate <= cmd.IssueDate &&
        cmd.IssueDate < cmd.MaturityDate;
}
