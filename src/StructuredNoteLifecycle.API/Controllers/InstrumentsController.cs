using MediatR;
using Microsoft.AspNetCore.Mvc;
using StructuredNoteLifecycle.API.Models;
using StructuredNoteLifecycle.Application.Instruments.Commands.AmendInstrument;
using StructuredNoteLifecycle.Application.Instruments.Commands.ApplyCouponFixing;
using StructuredNoteLifecycle.Application.Instruments.Commands.ApplyRedemptionFixing;
using StructuredNoteLifecycle.Application.Instruments.Commands.BookInstrument;
using StructuredNoteLifecycle.Application.Instruments.Commands.ConfirmTradeBooking;
using StructuredNoteLifecycle.Application.Instruments.Commands.ConfirmTradeSettle;
using StructuredNoteLifecycle.Application.Instruments.Queries.GetInstrumentById;

namespace StructuredNoteLifecycle.API.Controllers;

[ApiController]
[Route("api/instruments")]
public sealed class InstrumentsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> BookInstrument(
        [FromBody] EventEnvelope<BookInstrumentRequest> envelope, CancellationToken ct)
    {
        var p = envelope.Payload;
        var cmd = new BookInstrumentCommand(
            envelope.MessageId, envelope.CorrelationId, envelope.Source,
            p.OrderId, p.QuoteId, p.CounterpartyId,
            p.TradeDate, p.ValueDate, p.IssueDate, p.MaturityDate,
            p.Notional, p.Currency, p.IssuerId, p.ProductType, p.Isin,
            p.PayoffDefinition, p.EconomicTerms);

        var result = await sender.Send(cmd, ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetInstrument), new { id = result.Value }, new { instrumentId = result.Value?.Value })
            : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetInstrument(Guid id, CancellationToken ct)
    {
        var instrument = await sender.Send(new GetInstrumentByIdQuery(id), ct);
        return instrument is null ? NotFound() : Ok(instrument);
    }

    [HttpPut("{id:guid}/amend")]
    public async Task<IActionResult> AmendInstrument(
        Guid id, [FromBody] EventEnvelope<AmendInstrumentRequest> envelope, CancellationToken ct)
    {
        var p = envelope.Payload;
        var cmd = new AmendInstrumentCommand(
            envelope.MessageId, envelope.CorrelationId, envelope.Source,
            id, p.ExpectedCurrentVersion, p.AmendmentReason, p.AmendedBy,
            p.Changes, p.AmendmentType);

        var result = await sender.Send(cmd, ct);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/confirm-booking")]
    public async Task<IActionResult> ConfirmTradeBooking(
        Guid id, [FromBody] EventEnvelope<ConfirmTradeBookingRequest> envelope, CancellationToken ct)
    {
        var p = envelope.Payload;
        var cmd = new ConfirmTradeBookingCommand(
            envelope.MessageId, envelope.CorrelationId, envelope.Source,
            id, p.TradeId, p.ExternalConfirmRef, p.ConfirmationType,
            p.MatchStatus, p.ConfirmedBy, p.ConfirmationTimestamp);

        var result = await sender.Send(cmd, ct);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/confirm-settlement")]
    public async Task<IActionResult> ConfirmTradeSettle(
        Guid id, [FromBody] EventEnvelope<ConfirmTradeSettleRequest> envelope, CancellationToken ct)
    {
        var p = envelope.Payload;
        var cmd = new ConfirmTradeSettleCommand(
            envelope.MessageId, envelope.CorrelationId, envelope.Source,
            id, p.TradeId, p.SettlementRef, p.SettlementDate, p.ActualSettlementDate,
            p.SettlementStatus, p.DeliveryType, p.SettledQuantity,
            p.SettledCash, p.SettlementCurrency, p.CustodianRef);

        var result = await sender.Send(cmd, ct);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/coupon-fixing")]
    public async Task<IActionResult> ApplyCouponFixing(
        Guid id, [FromBody] EventEnvelope<CouponFixingRequest> envelope, CancellationToken ct)
    {
        var p = envelope.Payload;
        var fixings = p.Fixings
            .Select(f => new FixingEntry(f.UnderlyingId, f.InitialLevel, f.ObservedLevel, f.Source, f.IsOfficial))
            .ToList();

        var cmd = new ApplyCouponFixingCommand(
            envelope.MessageId, envelope.CorrelationId, envelope.Source,
            id, p.ObservationDate, p.SequenceNumber, fixings);

        var result = await sender.Send(cmd, ct);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/redemption-fixing")]
    public async Task<IActionResult> ApplyRedemptionFixing(
        Guid id, [FromBody] EventEnvelope<RedemptionFixingRequest> envelope, CancellationToken ct)
    {
        var p = envelope.Payload;
        var fixings = p.Fixings
            .Select(f => new FixingEntry(f.UnderlyingId, f.InitialLevel, f.ObservedLevel, f.Source, f.IsOfficial))
            .ToList();

        var knockIn = p.KnockInHistory is { } ki
            ? new KnockInHistory(ki.HasBreached, ki.BreachDate, ki.BreachUnderlying)
            : null;

        var obsType = p.ObservationType == "FINAL_MATURITY" ? ObservationType.FinalMaturity : ObservationType.Autocall;

        var cmd = new ApplyRedemptionFixingCommand(
            envelope.MessageId, envelope.CorrelationId, envelope.Source,
            id, p.ObservationDate, obsType, p.SequenceNumber, fixings, knockIn, p.PhysicalSettlement);

        var result = await sender.Send(cmd, ct);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
