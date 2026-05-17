using MediatR;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;

namespace StructuredNoteLifecycle.Application.Instruments.Queries.GetInstrumentById;

public sealed record GetInstrumentByIdQuery(Guid InstrumentId) : IRequest<Instrument?>;
