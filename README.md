# EnTrackBag — Database-First 3-Application Source

This source follows the agreed architecture:

1. `Identity.Api` — authentication, JWT issuance, sessions and identity domain components.
2. `EnTrackBag.Api` — operational APIs, SignalR hub and operational domain components.
3. `UI` — Angular 19 frontend.

## Naming convention

Database-first EF Core classes use the `Entity` suffix:
- `ReaderEntity`
- `AntennaEntity`
- `ControllerEntity`
- `UserEntity`

DTOs use the `Dto` suffix:
- `ReaderDto`
- `LoginRequestDto`
- `DashboardKpiDto`

Repositories use the entity/feature name + `Repository`:
- `ReaderRepository`
- `DashboardRepository`
- `SlaRepository`
- `IdentityRepository`

Business/use-case logic uses `DomainComponent`:
- `DashboardDomainComponent`
- `SlaDomainComponent`
- `DeviceStatusDomainComponent`
- `IdentityDomainComponent`

Controllers use explicit constructor injection:

```csharp
private readonly IIdentityDomainComponent _identityDomainComponent;

public AuthController(IIdentityDomainComponent identityDomainComponent)
{
    _identityDomainComponent = identityDomainComponent;
}
```

## Database-first rules

- Existing `BLTSMFT` is the source of truth.
- No EF migrations for the existing operational schema.
- No `EnsureCreated()`.
- No `Database.Migrate()`.
- No automatic schema creation/update at startup.
- Entities/configurations must be generated or aligned from the actual SQL Server schema.
- `EnTrackBagExceptions` and identity tables are application-owned additions to the existing database and should be installed by SQL script, then scaffolded/mapped from the resulting schema.

The checked-in entity properties are based only on the currently verified schema notes. Before production use, regenerate/verify them against the exact target `BLTSMFT` database.

See `Documentation/NAMING-AND-ARCHITECTURE.md` and `EnTrackBag-Final-Project-Prompt.md` for the mandatory naming, constructor injection and Database-First rules.


## Permission access types
Authorization is modeled as Permission + AccessType. AccessTypes are VIEW, CREATE, EDIT, DELETE, and EXPORT. RolePermissions uses (RoleId, PermissionId, AccessTypeId). JWT contains permission_access claims such as `TagReport.View:VIEW`. Angular receives the same structured permission/access list and uses it for navigation and route guards; the .NET API enforces the same permission/access combination.
