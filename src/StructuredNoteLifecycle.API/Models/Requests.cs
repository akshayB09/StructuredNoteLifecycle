using StructuredNoteLifecycle.Domain.Aggregates.Instruments;

namespace StructuredNoteLifecycle.API.Models;

public sealed record BookInstrumentRequest(
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
    EconomicTerms EconomicTerms);

public sealed record AmendInstrumentRequest(
    int ExpectedCurrentVersion,
    string AmendmentReason,
    string AmendedBy,
    Dictionary<string, object?> Changes,
    AmendmentType AmendmentType);

public sealed record ConfirmTradeBookingRequest(
    string TradeId,
    string ExternalConfirmRef,
    string ConfirmationType,
    string MatchStatus,
    string ConfirmedBy,
    DateTimeOffset ConfirmationTimestamp);

public sealed record ConfirmTradeSettleRequest(
    string TradeId,
    string SettlementRef,
    DateOnly SettlementDate,
    DateOnly ActualSettlementDate,
    string SettlementStatus,
    string DeliveryType,
    decimal SettledQuantity,
    decimal SettledCash,
    string SettlementCurrency,
    string CustodianRef);

public sealed record FixingEntryRequest(
    string UnderlyingId,
    decimal InitialLevel,
    decimal ObservedLevel,
    string Source,
    bool IsOfficial);

public sealed record CouponFixingRequest(
    DateOnly ObservationDate,
    int SequenceNumber,
    List<FixingEntryRequest> Fixings);

public sealed record RedemptionFixingRequest(
    DateOnly ObservationDate,
    string ObservationType,
    int SequenceNumber,
    List<FixingEntryRequest> Fixings,
    KnockInHistoryRequest? KnockInHistory,
    bool PhysicalSettlement = false);

public sealed record KnockInHistoryRequest(
    bool HasBreached,
    DateOnly? BreachDate,
    string? BreachUnderlying);
