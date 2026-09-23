# EnTrackBag Fresh Development Prompt — Database-First + Explicit Naming + Constructor Injection

Build the EnTrackBag greenfield replacement using exactly three applications in one solution/repository:

1. `Identity.Api`
2. `EnTrackBag.Api`
3. `UI` (Angular 19)

Backend stack: .NET 10 Web API, EF Core 10, SQL Server, SignalR. Frontend: Angular 19.

## 1. Database-first is mandatory

The existing SQL Server database `BLTSMFT` is the operational source of truth. Do not replace it and do not create a new operational database.

Use EF Core Database-First for existing tables and views. Entities, relationships, keys, nullability and generated-value behavior must be based on the actual SQL Server schema.

Never use:
- EF migrations for the existing database
- `Database.Migrate()`
- `Database.EnsureCreated()`
- startup schema creation/update

The application-owned identity/exception tables are installed by SQL script into the existing `BLTSMFT` database. After installation they are still treated as database-first objects: scaffold or verify their actual schema and map the generated entities to those SQL objects.

Do not invent operational columns, keys, relationships, antenna counts, device telemetry or SLA settings. If the existing database does not establish a fact, return Unknown/neutral or document the missing source.

## 2. Backend naming convention — mandatory

Every backend file/type must use explicit suffixes:

- Entity: `<Name>Entity` — `ReaderEntity`, `UserEntity`, `EnTrackBagExceptionEntity`
- DTO: `<Name>Dto` — `ReaderDto`, `LoginRequestDto`, `SlaBagDto`
- Repository interface: `I<Name>Repository` — `IReaderRepository`
- Repository implementation: `<Name>Repository` — `ReaderRepository`
- Domain component interface: `I<Name>DomainComponent` — `ISlaDomainComponent`
- Domain component implementation: `<Name>DomainComponent` — `SlaDomainComponent`
- Controller: `<Name>Controller` — `AuthController`
- Angular service: `<Name>Service` — `AuthService`

Do not use vague names such as `Model`, `Manager`, `Handler` or generic `Service` for backend business logic when a feature-specific DomainComponent is appropriate.

Use normal `class` by default. Do not add `sealed` unless there is a deliberate architectural reason to prohibit inheritance.

## 3. Constructor injection — mandatory

Use ordinary constructors for controllers, repositories, domain components and DbContexts. Do not use primary constructors for these classes.

Required controller style:

```csharp
public class AuthController : ControllerBase
{
    private readonly IIdentityDomainComponent _identityDomainComponent;

    public AuthController(IIdentityDomainComponent identityDomainComponent)
    {
        _identityDomainComponent = identityDomainComponent;
    }

    [HttpPost("login")]
    [ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequestDto request, CancellationToken ct)
    {
        var ua = Request.Headers["User-Agent"].ToString();
        var result = await _identityDomainComponent.LoginAsync(
            request,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            ua,
            null,
            ct);

        return result is null ? Unauthorized() : Ok(result);
    }
}
```

Apply the same explicit constructor pattern to `ReaderRepository`, `SlaRepository`, `IdentityDomainComponent`, `SlaDomainComponent`, etc.

## 4. Dependency flow

```text
Controller
  ↓
IDomainComponent
  ↓
DomainComponent
  ↓
IRepository
  ↓
Repository
  ↓
EF Core DbContext
  ↓
BLTSMFT
```

Controllers handle HTTP concerns only. Domain components contain business/use-case orchestration. Repositories contain database access/query logic. DbContexts represent the database schema.

Do not create a generic `IDomainComponent` or a generic repository. Use feature-specific interfaces.

Examples:
- `IDashboardDomainComponent` / `DashboardDomainComponent`
- `ISlaDomainComponent` / `SlaDomainComponent`
- `IDeviceStatusDomainComponent` / `DeviceStatusDomainComponent`
- `IBagJourneyDomainComponent` / `BagJourneyDomainComponent`
- `ITagReportDomainComponent` / `TagReportDomainComponent`
- `IIdentityDomainComponent` / `IdentityDomainComponent`
- `IUserDomainComponent` / `UserDomainComponent`
- `IRoleDomainComponent` / `RoleDomainComponent`
- `ISessionDomainComponent` / `SessionDomainComponent`

## 5. Repository convention

Repositories are feature-focused. Use names such as:
- `ReaderRepository`
- `DashboardRepository`
- `SlaRepository`
- `IdentityRepository`
- `UserRepository`
- `RoleRepository`
- `SessionRepository`

Do not create a repository for every table merely to wrap `DbSet<T>` if the feature does not need one. When a repository is created, it should expose meaningful feature queries.

## 6. DbContext and EF configuration

Use separate configuration files where practical:

```text
Data/
  BltsmftDbContext.cs
  Entities/
    ReaderEntity.cs
    AntennaEntity.cs
    ControllerEntity.cs
  Configurations/
    ReaderEntityConfiguration.cs
    AntennaEntityConfiguration.cs
    ControllerEntityConfiguration.cs
  Repositories/
    IReaderRepository.cs
    ReaderRepository.cs
```

The DbContext should load configurations using `ApplyConfigurationsFromAssembly(...)`.

Do not let configuration become a replacement for Database-First. Configuration must reflect the actual database schema.

## 7. Authentication and authorization

`Identity.Api` issues JWT access tokens. `EnTrackBag.Api` validates the same token locally. The same JWT secures HTTP APIs and SignalR.

Use roles plus permissions. Do not hardcode role names throughout controllers.

Roles:
1. Ground Floor — Summary Dashboard only
2. Supervisor — Summary Dashboard + SLA Dashboard
3. Site Manager — everything except Administration
4. Admin — Administration plus management permissions

## 7A. Permission + Access Type Model

Authorization is modeled as **Permission + AccessType**. Add an `AccessTypes` reference table with these codes: `VIEW`, `CREATE`, `EDIT`, `DELETE`, `EXPORT`. `Permissions` represent the page or function. `RolePermissions` assigns `RoleId + PermissionId + AccessTypeId`.

Role access matrix:
- Ground Floor: Summary Dashboard VIEW only.
- Supervisor: Summary Dashboard VIEW + SLA Dashboard VIEW.
- Site Manager: Summary Dashboard, SLA Dashboard, Device & System Status, Device Details, Tag Report, and Bag Journey VIEW; no Administration.
- Admin: all pages VIEW including Administration; Users may VIEW/CREATE/EDIT/DELETE; Roles may VIEW/EDIT; Sessions and Audit Log VIEW.

The JWT must carry permission/access claims such as `TagReport.View:VIEW`. Angular navigation/guards and .NET API authorization must enforce the same permission/access combination.

## 8. SignalR

Use `/hubs/monitoring` in `EnTrackBag.Api` with JWT authorization. Use SignalR for verified real-time operational events such as device status changes, alarms and dashboard updates. Do not fabricate events when no authoritative event source exists.

## 9. Global exception handling

Both APIs must have global exception handling. Include correlation ID, safe HTTP 500 response, structured logging and persistence to `dbo.EnTrackBagExceptions`. Never return stack traces to the browser.

Correct User-Agent access:

```csharp
var userAgent = context.Request.Headers["User-Agent"].ToString();
```

## 10. UI API endpoints

Development environment URLs:
- Identity API: `http://localhost:5200/api`
- EnTrackBag API: `http://localhost:5100/api`
- Monitoring hub: `http://localhost:5100/hubs/monitoring`

Production URLs are configured in Angular environment files. Do not hide development API URLs behind an Angular proxy.

## 11. Angular HTTP retry

Retry only transient HTTP failures: 0, 408, 429, 500, 502, 503, 504. Maximum three attempts total (initial request plus two retries). Do not retry 400, 401, 403, 404 or other permanent client errors.

## 12. UI requirements

Keep the fixed approved light dashboard theme.

Dashboard uses a right-side `Summary | SLA` toggle. Exception Dashboard is not a UI module. Exception logging remains required.

Map is mandatory and prominent on both Summary and SLA.

Summary cards:
- Bags Tagged Today
- Processed at Reclaim Today
- Processed at Exit Today
- Processed at Recheck Today

SLA cards:
- Active Bags Now
- Oldest Active Bag
- Bags Over SLA

SLA table ends with `History`, not `Action`.

Device page name is `Device & System Status`. Selecting a category shows `Device Details`. Do not label the section `Recent Device Event`.

Tagging Station is a Windows application/device, not an RFID reader; never show antenna A1-A8 for a Tagging Station. Reader antenna information is shown only where the database establishes a reader/antenna relationship.

An FX9600 has eight antenna ports, but port count must never be treated as physical antenna count. Count configured antenna records from the database. Never assign one antenna to multiple readers.

## 13. Operational source constraints

Known source architecture:
- Tagging Station Windows application.
- `DataProcessor_S1` processes Tagging Read Point, DogHouse Airside and DogHouse Landside readers.
- `CS_ADG2` processes Exit Gate reader and ADAM controller.

The new UI/API reads the operational database and does not replace these source applications/schedulers.

## 14. DI lifetimes

- DbContext: Scoped
- Repository using DbContext: Scoped
- Domain component using repository/DbContext: Scoped
- Transient only for lightweight stateless components where appropriate
- Singleton only for truly application-wide stateless/safe components

## 15. Deliverable

Generate a clean solution with the above architecture, explicit names, ordinary constructor injection, Database-First EF Core organization, no migrations/EnsureCreated, environment-based Angular endpoints, SignalR, global exception persistence, and documentation explaining how to scaffold/refresh the EF model from the existing `BLTSMFT` database.
