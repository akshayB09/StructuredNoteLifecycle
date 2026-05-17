using MediatR;
using StructuredNoteLifecycle.Application.Common.Behaviors;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;
using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.BookInstrument;

public sealed record BookInstrumentCommand(
    Guid MessageId,
    Guid CorrelationId,
    string Source,
    string OrderId,
    string? QuoteId,
    string CounterpartyId,
    DateOnly TradeDate,
    DateOnly ValueDate,
    DateOnly IssueDate,
    DateOnly MaturityDate,
    decimal Notional,
    string Currency,
    string IssuerId,
    string ProductType,
    string? Isin,
    PayoffDefinition PayoffDefinition,
    EconomicTerms EconomicTerms) : IRequest<Result<InstrumentId>>, IIdempotentCommand
{
    public string EventType => "BookInstrument";
}
