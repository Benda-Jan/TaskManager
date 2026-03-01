# TaskManager_BE — Claude Code Guide

This file is the authoritative reference for how Claude Code should generate, modify, and reason about code in this repository. Follow every convention here exactly. When in doubt, ask before deviating.

---

## 1. Project Overview

**TaskManager_BE** is a RESTful backend API for managing tasks, projects, and users. It is a greenfield .NET 10 / C# 13 application following Clean Architecture principles. Authentication is delegated to Keycloak; the API acts as a JWT Bearer resource server only.

---

## 2. Tech Stack & Versions

| Concern | Package / Tool | Version |
|---|---|---|
| Runtime | .NET | 10 |
| Language | C# | 13 |
| Web framework | ASP.NET Core (controller-based) | 10 |
| ORM | Microsoft.EntityFrameworkCore | 10.x |
| DB driver | Npgsql.EntityFrameworkCore.PostgreSQL | 10.x |
| Mediator | MediatR | 12.x |
| Validation | FluentValidation | 11.x |
| Auth | Microsoft.AspNetCore.Authentication.JwtBearer | 10.x |
| Health checks | AspNetCore.HealthChecks.Npgsql | latest |
| Testing | xUnit | 2.x |
| Mocking | Moq | 4.x |
| Assertions | FluentAssertions | 7.x |
| Containerisation | Docker + docker-compose | latest |
| Identity provider | Keycloak | 26.x |

Do not introduce packages outside this list without explicit user approval.

---

## 3. Architecture

This project uses **Clean Architecture** with a strict dependency rule: inner layers never reference outer layers.

```
┌─────────────────────────────────────────┐
│                   API                   │  ← Presentation layer
│  Controllers, Middleware, DI wiring     │
├─────────────────────────────────────────┤
│             Application                 │  ← Use-case layer
│  Commands, Queries, Handlers,           │
│  Validators, DTOs, Interfaces           │
├─────────────────────────────────────────┤
│             Infrastructure              │  ← Adapter layer
│  EF Core, Repositories, Keycloak,      │
│  External services                      │
├─────────────────────────────────────────┤
│               Domain                    │  ← Core layer
│  Entities, Value Objects, Enums,        │
│  Domain events, Domain exceptions       │
└─────────────────────────────────────────┘
```

**Dependency directions:**
- `API` → `Application`, `Infrastructure`
- `Application` → `Domain`
- `Infrastructure` → `Application`, `Domain`
- `Domain` → nothing

**Layer responsibilities:**

| Layer | Allowed | Forbidden |
|---|---|---|
| Domain | Entities, value objects, domain exceptions, domain events | EF Core, MediatR, HTTP, anything external |
| Application | Use-case logic, CQRS handlers, interfaces, DTOs, validators | EF Core, infrastructure concerns, HTTP |
| Infrastructure | EF DbContext, repository implementations, Keycloak client | Business logic, HTTP controllers |
| API | Controllers, middleware, program.cs wiring | Direct DB access, business logic |

---

## 4. Solution Structure

```
TaskManager_BE/
├── CLAUDE.md
├── docker-compose.yml
├── .env                          # local secrets (never committed)
├── .env.example                  # committed template
├── src/
│   ├── TaskManager.Domain/
│   │   ├── TaskManager.Domain.csproj
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Enums/
│   │   ├── Events/
│   │   └── Exceptions/
│   ├── TaskManager.Application/
│   │   ├── TaskManager.Application.csproj
│   │   ├── Common/
│   │   │   ├── Interfaces/       # IRepository<T>, IUnitOfWork, ICurrentUserService
│   │   │   ├── Behaviours/       # ValidationBehaviour, LoggingBehaviour
│   │   │   └── Models/           # PagedResult<T>, Result<T>
│   │   └── Features/
│   │       └── Tasks/
│   │           ├── Commands/
│   │           │   └── CreateTask/
│   │           │       ├── CreateTaskCommand.cs
│   │           │       ├── CreateTaskCommandHandler.cs
│   │           │       └── CreateTaskCommandValidator.cs
│   │           └── Queries/
│   │               └── GetTaskById/
│   │                   ├── GetTaskByIdQuery.cs
│   │                   ├── GetTaskByIdQueryHandler.cs
│   │                   └── TaskDto.cs
│   ├── TaskManager.Infrastructure/
│   │   ├── TaskManager.Infrastructure.csproj
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/   # IEntityTypeConfiguration<T> classes
│   │   │   ├── Repositories/
│   │   │   └── Migrations/
│   │   └── Services/
│   │       └── CurrentUserService.cs
│   └── TaskManager.API/
│       ├── TaskManager.API.csproj
│       ├── Program.cs
│       ├── Controllers/
│       ├── Middleware/
│       └── appsettings.json
└── tests/
    ├── TaskManager.Domain.Tests/
    ├── TaskManager.Application.Tests/
    └── TaskManager.Infrastructure.Tests/
```

---

## 5. C# Naming Conventions

| Construct | Convention | Example |
|---|---|---|
| Classes, interfaces, records | PascalCase | `CreateTaskCommand`, `ITaskRepository` |
| Methods | PascalCase | `HandleAsync` |
| Private fields | `_camelCase` | `_repository` |
| Parameters, locals | camelCase | `cancellationToken` |
| Constants | PascalCase | `DefaultPageSize` |
| Interfaces | Prefix with `I` | `ICurrentUserService` |

**Always use:**
- File-scoped namespaces (`namespace TaskManager.Domain.Entities;`)
- `#nullable enable` globally via `<Nullable>enable</Nullable>` in all csproj files
- Records for DTOs and value objects where appropriate
- Primary constructors for simple dependency injection (C# 12+)

**Lifetimes:**
- Repositories → `Scoped`
- DbContext → `Scoped`
- HttpClient-based services → `Transient` or `Singleton` (use `IHttpClientFactory`)
- Stateless helpers → `Singleton`

---

## 6. CQRS Pattern

All use cases are modelled as Commands (mutations) or Queries (reads) handled by MediatR.

### Command

```csharp
// src/TaskManager.Application/Features/Tasks/Commands/CreateTask/CreateTaskCommand.cs
namespace TaskManager.Application.Features.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(string Title, string? Description, Guid ProjectId) : IRequest<Guid>;
```

### Validator

```csharp
// src/TaskManager.Application/Features/Tasks/Commands/CreateTask/CreateTaskCommandValidator.cs
namespace TaskManager.Application.Features.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
```

### Handler

```csharp
// src/TaskManager.Application/Features/Tasks/Commands/CreateTask/CreateTaskCommandHandler.cs
namespace TaskManager.Application.Features.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandHandler(ITaskRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateTaskCommand, Guid>
{
    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskItem(request.Title, request.Description, request.ProjectId);
        await repository.AddAsync(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return task.Id;
    }
}
```

### Query + DTO

```csharp
// src/TaskManager.Application/Features/Tasks/Queries/GetTaskById/GetTaskByIdQuery.cs
namespace TaskManager.Application.Features.Tasks.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(Guid Id) : IRequest<TaskDto?>;

// TaskDto.cs
public sealed record TaskDto(Guid Id, string Title, string? Description, Guid ProjectId);
```

### ValidationBehaviour (registers once in Application DI)

```csharp
// src/TaskManager.Application/Common/Behaviours/ValidationBehaviour.cs
namespace TaskManager.Application.Common.Behaviours;

public sealed class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
```

---

## 7. Controller Pattern

**Rules:**
- Inherit from `ControllerBase` (never `Controller`)
- Inject `ISender` (from MediatR), not `IMediator`
- Decorate with `[ApiController]`, `[Route("api/[controller]")]`
- Use `[Authorize]` at class level; use `[AllowAnonymous]` on exceptions
- Return `ActionResult<T>` or `IActionResult`
- Map domain/application exceptions to HTTP responses in middleware, not in controllers
- No business logic in controllers

```csharp
// src/TaskManager.API/Controllers/TasksController.cs
namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TasksController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType<TaskDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTaskByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }
}
```

---

## 8. Entity Framework Core

### DbContext

```csharp
// src/TaskManager.Infrastructure/Persistence/AppDbContext.cs
namespace TaskManager.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
```

### Fluent API Configuration (never data annotations on entities)

```csharp
// src/TaskManager.Infrastructure/Persistence/Configurations/TaskItemConfiguration.cs
namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(2000);
    }
}
```

### Repository Interface (Application layer)

```csharp
// src/TaskManager.Application/Common/Interfaces/ITaskRepository.cs
namespace TaskManager.Application.Common.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    void Remove(TaskItem task);
}
```

### Repository Implementation (Infrastructure layer)

```csharp
// src/TaskManager.Infrastructure/Persistence/Repositories/TaskRepository.cs
namespace TaskManager.Infrastructure.Persistence.Repositories;

public sealed class TaskRepository(AppDbContext context) : ITaskRepository
{
    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
        => await context.Tasks.AddAsync(task, cancellationToken);

    public void Remove(TaskItem task)
        => context.Tasks.Remove(task);
}
```

### Migration Commands

```bash
# Add migration (run from repo root)
dotnet ef migrations add <MigrationName> \
  --project src/TaskManager.Infrastructure \
  --startup-project src/TaskManager.API

# Apply migrations
dotnet ef database update \
  --project src/TaskManager.Infrastructure \
  --startup-project src/TaskManager.API

# Revert last migration
dotnet ef migrations remove \
  --project src/TaskManager.Infrastructure \
  --startup-project src/TaskManager.API
```

---

## 9. Authentication

The API is a **JWT Bearer resource server only**. It does not issue tokens; Keycloak does.

### Program.cs wiring

```csharp
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience  = builder.Configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });

builder.Services.AddAuthorization();
```

### appsettings.json keys

```json
{
  "Keycloak": {
    "Authority": "http://keycloak:8080/realms/taskmanager",
    "Audience": "taskmanager-api"
  }
}
```

### ICurrentUserService

```csharp
// src/TaskManager.Application/Common/Interfaces/ICurrentUserService.cs
namespace TaskManager.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
```

```csharp
// src/TaskManager.Infrastructure/Services/CurrentUserService.cs
namespace TaskManager.Infrastructure.Services;

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid UserId =>
        Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

    public string? Email => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated => accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
```

---

## 10. Docker Setup

### docker-compose.yml

```yaml
services:
  postgres:
    image: postgres:17-alpine
    environment:
      POSTGRES_DB: ${POSTGRES_DB}
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER} -d ${POSTGRES_DB}"]
      interval: 10s
      timeout: 5s
      retries: 5

  keycloak:
    image: quay.io/keycloak/keycloak:26.0
    command: start-dev
    environment:
      KEYCLOAK_ADMIN: ${KEYCLOAK_ADMIN}
      KEYCLOAK_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD}
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://postgres:5432/${POSTGRES_DB}
      KC_DB_USERNAME: ${POSTGRES_USER}
      KC_DB_PASSWORD: ${POSTGRES_PASSWORD}
    ports:
      - "8080:8080"
    depends_on:
      postgres:
        condition: service_healthy

  api:
    build:
      context: .
      dockerfile: src/TaskManager.API/Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__Default=Host=postgres;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}
      - Keycloak__Authority=http://keycloak:8080/realms/taskmanager
      - Keycloak__Audience=taskmanager-api
    ports:
      - "5000:8080"
    depends_on:
      postgres:
        condition: service_healthy
      keycloak:
        condition: service_started
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

volumes:
  postgres_data:
```

### Dockerfile (multi-stage)

```dockerfile
# src/TaskManager.API/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/TaskManager.API/TaskManager.API.csproj", "src/TaskManager.API/"]
COPY ["src/TaskManager.Application/TaskManager.Application.csproj", "src/TaskManager.Application/"]
COPY ["src/TaskManager.Infrastructure/TaskManager.Infrastructure.csproj", "src/TaskManager.Infrastructure/"]
COPY ["src/TaskManager.Domain/TaskManager.Domain.csproj", "src/TaskManager.Domain/"]

RUN dotnet restore "src/TaskManager.API/TaskManager.API.csproj"

COPY . .
RUN dotnet publish "src/TaskManager.API/TaskManager.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.API.dll"]
```

### .env.example (commit this; never commit .env)

```
POSTGRES_DB=taskmanager
POSTGRES_USER=taskmanager
POSTGRES_PASSWORD=changeme
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=changeme
```

---

## 11. Common Commands

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run API locally
dotnet run --project src/TaskManager.API

# Run all tests
dotnet test

# Run a specific test project
dotnet test tests/TaskManager.Application.Tests

# Start all containers
docker-compose up -d

# Stop containers
docker-compose down

# Rebuild and start
docker-compose up -d --build

# View logs
docker-compose logs -f api

# Add EF migration
dotnet ef migrations add <Name> \
  --project src/TaskManager.Infrastructure \
  --startup-project src/TaskManager.API

# Apply migrations
dotnet ef database update \
  --project src/TaskManager.Infrastructure \
  --startup-project src/TaskManager.API
```

---

## 12. Testing Conventions

**Naming:** `MethodUnderTest_Scenario_ExpectedBehavior`

**Structure:** Arrange / Act / Assert with FluentAssertions

**Location:** Mirror the `src/` namespace in `tests/`. Each test class tests exactly one class.

```csharp
// tests/TaskManager.Application.Tests/Features/Tasks/Commands/CreateTaskCommandHandlerTests.cs
namespace TaskManager.Application.Tests.Features.Tasks.Commands;

public sealed class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task Handle_ValidCommand_ReturnsNewTaskId()
    {
        // Arrange
        var command = new CreateTaskCommand("Test task", null, Guid.NewGuid());
        var handler = new CreateTaskCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

- Use `Mock<T>` for all external dependencies
- Test one logical behaviour per `[Fact]`
- Use `[Theory]` + `[InlineData]` for parameterised cases
- Do not test EF Core directly — use repository interfaces and mock them

---

## 13. What NOT To Do

| Rule | Reason |
|---|---|
| Never use Minimal APIs | Project uses controller-based Web API |
| Never reference Infrastructure from Application or Domain | Violates Clean Architecture |
| Never reference API from any other project | Presentation must be outermost |
| Never put data annotations (`[Required]`, `[MaxLength]`) on Domain entities | Use Fluent API in configurations |
| Never put business logic in controllers | Controllers only dispatch to MediatR |
| Never inject `AppDbContext` outside Infrastructure | Use repository interfaces |
| Never inject `IMediator` — use `ISender` | Limits surface area to sending only |
| Never commit `.env` | Contains secrets |
| Never use `var` where the type is not obvious from the right-hand side | Prefer explicit types for clarity |
| Never catch and swallow exceptions silently | Log or re-throw |
| Never add NuGet packages not in the approved list without user approval | Keeps dependency footprint controlled |

---

## 14. Configuration & Secrets

**Hierarchy (later overrides earlier):**
1. `appsettings.json` — committed defaults (no secrets)
2. `appsettings.{Environment}.json` — environment-specific non-secrets
3. User Secrets (`dotnet user-secrets`) — local dev secrets
4. Environment variables — Docker / CI/CD secrets

**Connection string key:** `ConnectionStrings:Default`

**User Secrets setup (one-time per dev machine):**

```bash
dotnet user-secrets init --project src/TaskManager.API
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Database=taskmanager;Username=taskmanager;Password=changeme" --project src/TaskManager.API
```

**Access in code:** Always use `IConfiguration` or the Options pattern (`IOptions<T>`). Never hardcode connection strings or secrets.

---

## 15. Health Checks

Register in `Program.cs`:

```csharp
builder.Services
    .AddHealthChecks()
    .AddNpgsql(builder.Configuration.GetConnectionString("Default")!);

// ...

app.MapHealthChecks("/health");
```

The `GET /health` endpoint is unauthenticated (`[AllowAnonymous]` is implicit for `MapHealthChecks`). The Docker healthcheck in `docker-compose.yml` polls this endpoint.
