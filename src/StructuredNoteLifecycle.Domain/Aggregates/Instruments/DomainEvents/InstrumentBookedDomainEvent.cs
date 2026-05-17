using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Domain.Aggregates.Instruments.DomainEvents;

public sealed record InstrumentBookedDomainEvent(
    InstrumentId InstrumentId,
    string OrderId,
    string CounterpartyId,
    DateOnly TradeDate,
    decimal Notional,
    string Currency,
    string ProductType,
    DateTimeOffset OccurredOn) : IDomainEvent;
