# StructuredNoteLifecycle

A .NET solution demonstrating a Domain-Driven Design (DDD) approach to building a small bounded domain around "structured notes" and related instruments. This README explains how the repo maps common DDD concepts to the code so beginners can understand the architecture and where to look in the codebase.

## High-level overview

- `src/StructuredNoteLifecycle.API/` — Presentation layer (ASP.NET Core) that exposes HTTP endpoints and translates HTTP requests into application commands/queries. See [src/StructuredNoteLifecycle.API/Controllers/InstrumentsController.cs](src/StructuredNoteLifecycle.API/Controllers/InstrumentsController.cs).
- `src/StructuredNoteLifecycle.Application/` — Application layer (use cases). Orchestrates commands and queries, calls domain logic, and handles transactions and messaging. Inspect `DependencyInjection.cs` and the `Commands`/`Queries` folders.
- `src/StructuredNoteLifecycle.Domain/` — Domain layer with aggregates, entities, value objects, domain events, and business rules. The core model lives here; start with [src/StructuredNoteLifecycle.Domain/Aggregates/Instruments/Instrument.cs](src/StructuredNoteLifecycle.Domain/Aggregates/Instruments/Instrument.cs).
- `src/StructuredNoteLifecycle.Infrastructure/` — Infrastructure layer providing persistence, idempotency, outbox pattern, and other external concerns. Look at `Persistence`, `Outbox`, and `Idempotency` folders.

## DDD concepts (beginner-friendly)

This project applies common DDD ideas. Below are the concepts and where they appear in this codebase.

- **Bounded Contexts**: A bounded context is a logical boundary where a particular model applies. This repo keeps a single bounded context split into layers: API, Application, Domain, and Infrastructure. Think of each folder as a role rather than a separate microservice.

- **Ubiquitous Language**: The domain uses domain-specific terms (e.g., "Instrument", "EventEnvelope"). Those terms are used across `Domain` and `Application` so the code matches the language you use when reasoning about the problem.

- **Entities and Aggregates**: An entity has identity and lifecycle. An aggregate is a cluster of objects treated as a single unit for consistency. The `Instrument` aggregate (see [src/StructuredNoteLifecycle.Domain/Aggregates/Instruments/Instrument.cs](src/StructuredNoteLifecycle.Domain/Aggregates/Instruments/Instrument.cs)) contains the domain rules for creating and mutating instruments.

- **Value Objects**: Immutable types that represent a concept (e.g., money, identifiers). They are stored and passed by value. If you find small immutable helper types in `Domain`, they are likely value objects.

- **Domain Events**: Represent things that have happened in the domain. Events are used to notify other parts of the system that the state has changed. See `EventEnvelope` in [src/StructuredNoteLifecycle.API/Models/EventEnvelope.cs](src/StructuredNoteLifecycle.API/Models/EventEnvelope.cs) and domain event interfaces in `StructuredNoteLifecycle.Domain/Common`.

- **Repositories**: Abstractions that provide collection-like access to aggregates (fetch/save). Concrete implementations live in `Infrastructure/Persistence` and are registered in `StructuredNoteLifecycle.Infrastructure/DependencyInjection.cs`.

- **Application Layer (Use Cases)**: Coordinates domain operations and side effects. Application services translate input into domain commands, handle transactions, and publish events. See `StructuredNoteLifecycle.Application/Instruments/Commands` and `Queries` for examples.

- **Infrastructure**: Contains technical concerns (database, outbox, idempotency). Important files and folders:
  - `src/StructuredNoteLifecycle.Infrastructure/Outbox/OutboxService.cs` — outbox pattern to reliably send messages.
  - `src/StructuredNoteLifecycle.Infrastructure/Idempotency/IdempotencyStore.cs` — prevents duplicate processing.

## How requests flow (simple call sequence)

1. HTTP request arrives at `API` controller (for example, `InstrumentsController`).
2. Controller constructs a command (or query) and sends it to the `Application` layer.
3. Application layer loads aggregates from a `Repository`, applies domain logic on the `Domain` model, and persists changes.
4. Domain layer emits domain events which the Application/Infrastructure layers may publish via the outbox.

## Running the project

1. Build the solution:

```bash
dotnet build StructuredNoteLifecycle.slnx
```

2. Run the API project (development environment enables OpenAPI):

```bash
dotnet run --project src/StructuredNoteLifecycle.API/StructuredNoteLifecycle.API.csproj
# then open http://localhost:5119/swagger
```

## Where to start reading the code (recommended path)

1. `src/StructuredNoteLifecycle.API/Controllers/InstrumentsController.cs` — how the HTTP API maps to use cases.
2. `src/StructuredNoteLifecycle.Application/` — see command/handler structure and DI registrations.
3. `src/StructuredNoteLifecycle.Domain/Aggregates/Instruments/Instrument.cs` — core business rules and aggregate lifecycle.
4. `src/StructuredNoteLifecycle.Infrastructure/` — persistence and messaging concerns.

## Tips for beginners

- Focus first on a single use case (one controller endpoint) and follow it through the Application -> Domain -> Infrastructure path.
- Keep the Ubiquitous Language consistent: rename variables or methods so they match domain terms used by product experts.
- Avoid putting business logic in controllers — keep it in aggregates or domain services.

If you want, I can also add a simple walkthrough example to this README that follows a request from controller to database with annotated links to the exact lines of code. Would you like that? 
