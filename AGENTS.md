# AGENTS.md

## Purpose
- This repository contains a .NET directory service for organizational reference data: departments, positions, and locations.
- The codebase is structured as a layered backend under `backend/DirectoryService`.

## Repository Layout
- `backend/backend.slnx` - solution entry point.
- `backend/DirectoryService/src/DirectoryService.Presentation` - ASP.NET Core API host, controllers, middleware, OpenAPI, startup.
- `backend/DirectoryService/src/DirectoryService.Application` - commands, queries, validators, repository abstractions, DI.
- `backend/DirectoryService/src/DirectoryService.Domain` - domain entities, value objects, shared errors.
- `backend/DirectoryService/src/DirectoryService.Infrastructure.Postgres` - EF Core/Postgres persistence, migrations, repositories, seeding.
- `backend/DirectoryService/src/DirectoryService.Contracts` - transport/request-response DTOs.
- `backend/DirectoryService/tests/DirectoryService.IntegrationTests` - xUnit integration tests using `WebApplicationFactory`, Testcontainers Postgres, and Respawn.
- `docker-compose.yaml` - local Postgres, pgAdmin, and Seq.

## Runtime and Tooling
- Target framework: `net10.0`.
- Central package management is enabled in `backend/Directory.Packages.props`.
- StyleCop and .NET analyzers are enabled via `backend/Directory.Build.props`.
- Prefer solution-level commands from `backend/`.

## Common Commands
- Restore: `dotnet restore backend/backend.slnx`
- Build: `dotnet build backend/backend.slnx`
- Run API: `dotnet run --project backend/DirectoryService/src/DirectoryService.Presentation`
- Run API with seeding in Development: `dotnet run --project backend/DirectoryService/src/DirectoryService.Presentation -- --seeding`
- Run tests: `dotnet test backend/backend.slnx`

## Local Dependencies
- The API config expects a Seq connection string at startup. Local Seq is defined in `docker-compose.yaml`.
- Start local infrastructure when working on the API outside tests: `docker compose up -d`
- Compose ports:
  - Postgres: `5434`
  - pgAdmin: `5050`
  - Seq UI: `8081`
  - Seq ingest: `5341`
- Integration tests provision their own Postgres container and do not rely on the compose database.

## Architecture Notes
- Keep the existing layer boundaries intact:
  - `Presentation` depends on `Application` and infrastructure registration.
  - `Application` owns use cases, validation, and repository interfaces.
  - `Domain` stays free of infrastructure concerns.
  - `Infrastructure.Postgres` implements persistence and EF Core mappings.
  - `Contracts` contains DTOs only.
- New endpoint work should usually touch:
  - contract DTOs,
  - application command/query + handler + validator,
  - repository abstraction and implementation if persistence changes,
  - controller mapping in `Presentation`,
  - integration tests.

## Testing Expectations
- Prefer integration tests for user-visible API behavior and repository-backed workflows.
- Follow the existing test pattern:
  - inherit from `BaseIntegrationTest`,
  - use `DirectoryServiceTestsWebFactory`,
  - use faker helpers from `Fakers/`,
  - let Respawn reset DB state between tests.
- If changing EF mappings or schema, check whether a new migration is required in `Infrastructure.Postgres/Migrations`.

## Conventions
- Match the existing CQRS-style naming:
  - commands: `CreateXCommand`, `UpdateXCommand`
  - queries: `GetXQuery`
  - handlers: `CreateXHandler`, `GetXHandler`
  - validators: `CreateXCommandValidator`, `GetXQueryValidator`
- Reuse existing result and error patterns from `DirectoryService.Domain.Shared` and `DirectoryService.Presentation/Response`.
- Keep edits scoped; do not introduce new architectural layers or alternative patterns unless the existing structure cannot support the change cleanly.

## When Editing
- Check for existing validators, repository interfaces, and contract DTOs before adding new abstractions.
- Prefer extending current modules over creating parallel code paths.
- Keep migrations, DI registration, and tests in sync with behavior changes.
