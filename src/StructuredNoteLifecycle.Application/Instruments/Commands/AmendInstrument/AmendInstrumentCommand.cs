using MediatR;
using StructuredNoteLifecycle.Application.Common.Behaviors;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;
using StructuredNoteLifecycle.Domain.Common;

namespace StructuredNoteLifecycle.Application.Instruments.Commands.AmendInstrument;

public sealed record AmendInstrumentCommand(
    Guid MessageId,
    Guid CorrelationId,
    string Source,
    Guid InstrumentId,
    int ExpectedCurrentVersion,
    string AmendmentReason,
    string AmendedBy,
    IReadOnlyDictionary<string, object?> Changes,
    AmendmentType AmendmentType) : IRequest<Result>, IIdempotentCommand
{
    public string EventType => "AmendInstrument";
}
