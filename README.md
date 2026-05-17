# StructuredNoteLifecycle

A .NET solution implementing a structured note lifecycle with API, application, domain, and infrastructure layers.

## Projects

- `src/StructuredNoteLifecycle.API/` - ASP.NET Core Web API entry point with controllers and request models.
- `src/StructuredNoteLifecycle.Application/` - Application layer containing dependency injection, CQRS commands and queries, and service behaviors.
- `src/StructuredNoteLifecycle.Domain/` - Domain layer with aggregates, common domain infrastructure, domain events, and custom exceptions.
- `src/StructuredNoteLifecycle.Infrastructure/` - Infrastructure layer for persistence, idempotency, and outbox support.

## Getting Started

1. Open the solution in Visual Studio or JetBrains Rider.
2. Build the solution:
   ```bash
   dotnet build StructuredNoteLifecycle.slnx
   ```
3. Run the API project:
   ```bash
   dotnet run --project src/StructuredNoteLifecycle.API/StructuredNoteLifecycle.API.csproj
   ```

## Notes

- The solution is split into clean layers following DDD and CQRS principles.
- The infrastructure includes outbox and idempotency support for reliable message processing.
- The API exposes instrument-related endpoints under `Controllers/InstrumentsController.cs`.
