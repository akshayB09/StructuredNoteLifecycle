using MediatR;
using StructuredNoteLifecycle.Application.Common.Interfaces;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;
using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.BookInstrument;

public sealed class BookInstrumentCommandHandler(
    IInstrumentRepository repository,
    IUnitOfWork unitOfWork,
    IOutboxService outbox)
    : IRequestHandler<BookInstrumentCommand, Result<InstrumentId>>
{
    public async Task<Result<InstrumentId>> Handle(BookInstrumentCommand cmd, CancellationToken ct)
    {
        var instrument = Instrument.Book(
            cmd.OrderId, cmd.QuoteId, cmd.CounterpartyId,
            cmd.TradeDate, cmd.ValueDate, cmd.IssueDate, cmd.MaturityDate,
            cmd.Notional, cmd.Currency, cmd.IssuerId, cmd.ProductType, cmd.Isin,
            cmd.PayoffDefinition, cmd.EconomicTerms);

        await repository.AddAsync(instrument, ct);
        await outbox.PublishAsync(instrument.DomainEvents, ct);
        instrument.ClearDomainEvents();
        await unitOfWork.SaveChangesAsync(ct);

        return Result<InstrumentId>.Success(instrument.Id);
    }
}
