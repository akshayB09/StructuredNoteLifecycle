using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Domain.Aggregates.Instruments.DomainEvents;

public sealed record TradeSettledDomainEvent(
    InstrumentId InstrumentId,
    string TradeId,
    string SettlementRef,
    DateOnly SettlementDate,
    decimal SettledCash,
    string SettlementCurrency,
    DateTimeOffset OccurredOn) : IDomainEvent;
