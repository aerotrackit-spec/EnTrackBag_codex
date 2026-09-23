# EnTrackBag Naming and Dependency Rules

## Naming convention

For every new backend feature, use explicit suffixes:

| Purpose | Convention | Example |
|---|---|---|
| EF database entity | `<Name>Entity` | `ReaderEntity` |
| DTO | `<Name>Dto` | `ReaderDto` |
| Repository interface | `I<Name>Repository` | `IReaderRepository` |
| Repository implementation | `<Name>Repository` | `ReaderRepository` |
| Domain component interface | `I<Name>DomainComponent` | `ISlaDomainComponent` |
| Domain component implementation | `<Name>DomainComponent` | `SlaDomainComponent` |
| Controller | `<Name>Controller` | `AuthController` |
| Angular service | `<Name>Service` | `AuthService` |

## Constructor injection

Use ordinary constructors. Do not use primary constructors for controllers, repositories, domain components, or DbContexts in this project.

```csharp
public class AuthController : ControllerBase
{
    private readonly IIdentityDomainComponent _identityDomainComponent;

    public AuthController(IIdentityDomainComponent identityDomainComponent)
    {
        _identityDomainComponent = identityDomainComponent;
    }
}
```

The same pattern applies to repositories and domain components.

## Class declaration

Use normal `class` declarations by default. `sealed` is not required by EF Core and should only be used when there is a deliberate reason to prohibit inheritance.

## Database-first

Entities represent the existing SQL Server schema. The database defines table names, columns, keys, nullability, relationships and generated values. Do not design the operational schema from C# classes.

## Dependency flow

```text
Controller
   ↓ constructor injection
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

Repositories are feature-focused and should contain database access/query code. Domain components contain business/use-case orchestration. Controllers handle HTTP concerns only.

## DI lifetime

- `DbContext`: Scoped.
- Repository using DbContext: Scoped.
- Domain component using repository/DbContext: Scoped.
- Transient only for lightweight stateless components where appropriate.
- Singleton only for genuinely application-wide stateless components whose dependencies are singleton-safe.


## Permission access types
Authorization is modeled as Permission + AccessType. AccessTypes are VIEW, CREATE, EDIT, DELETE, and EXPORT. RolePermissions uses (RoleId, PermissionId, AccessTypeId). JWT contains permission_access claims such as `TagReport.View:VIEW`. Angular receives the same structured permission/access list and uses it for navigation and route guards; the .NET API enforces the same permission/access combination.
