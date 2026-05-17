using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructuredNoteLifecycle.Application.Common.Interfaces;
using StructuredNoteLifecycle.Infrastructure.Idempotency;
using StructuredNoteLifecycle.Infrastructure.Outbox;
using StructuredNoteLifecycle.Infrastructure.Persistence;
using StructuredNoteLifecycle.Infrastructure.Persistence.Repositories;

namespace StructuredNoteLifecycle.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(DependencyInjection).Assembly.GetName().Name)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IInstrumentRepository, InstrumentRepository>();
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IIdempotencyStore, IdempotencyStore>();

        return services;
    }
}
