using StructuredNoteLifecycle.Domain.Aggregates.Instruments;

namespace StructuredNoteLifecycle.Application.Common.Interfaces;

public interface IInstrumentRepository
{
    Task<Instrument?> GetByIdAsync(InstrumentId id, CancellationToken ct = default);
    Task<Instrument?> GetByOrderIdAsync(string orderId, CancellationToken ct = default);
    Task AddAsync(Instrument instrument, CancellationToken ct = default);
    Task UpdateAsync(Instrument instrument, CancellationToken ct = default);
}
