using Microsoft.EntityFrameworkCore;
using StructuredNoteLifecycle.Application.Common.Interfaces;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;

namespace StructuredNoteLifecycle.Infrastructure.Persistence.Repositories;

public sealed class InstrumentRepository(ApplicationDbContext db) : IInstrumentRepository
{
    public Task<Instrument?> GetByIdAsync(InstrumentId id, CancellationToken ct = default) =>
        db.Instruments.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<Instrument?> GetByOrderIdAsync(string orderId, CancellationToken ct = default) =>
        await db.Instruments.FirstOrDefaultAsync(i => i.Terms.OrderId == orderId, ct);

    public async Task AddAsync(Instrument instrument, CancellationToken ct = default) =>
        await db.Instruments.AddAsync(instrument, ct);

    public Task UpdateAsync(Instrument instrument, CancellationToken ct = default)
    {
        db.Instruments.Update(instrument);
        return Task.CompletedTask;
    }
}
