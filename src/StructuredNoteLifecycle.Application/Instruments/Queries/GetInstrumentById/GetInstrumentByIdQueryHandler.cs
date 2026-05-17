using MediatR;
using StructuredNoteLifecycle.Application.Common.Interfaces;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;

namespace StructuredNoteLifecycle.Application.Instruments.Queries.GetInstrumentById;

public sealed class GetInstrumentByIdQueryHandler(IInstrumentRepository repository)
    : IRequestHandler<GetInstrumentByIdQuery, Instrument?>
{
    public Task<Instrument?> Handle(GetInstrumentByIdQuery query, CancellationToken ct) =>
        repository.GetByIdAsync(new InstrumentId(query.InstrumentId), ct);
}
