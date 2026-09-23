# DomainComponent Architecture

The solution remains exactly three applications: `Identity.Api`, `EnTrackBag.Api`, and `UI`.

API feature flow:

`Controller -> IDomainComponent -> DomainComponent -> EF Core DbContext -> BLTSMFT`

Use feature-specific pairs such as:
- `IDashboardDomainComponent` / `DashboardDomainComponent`
- `ISlaDomainComponent` / `SlaDomainComponent`
- `IDeviceStatusDomainComponent` / `DeviceStatusDomainComponent`
- `IIdentityDomainComponent` / `IdentityDomainComponent`

Controllers use constructor injection. Domain components are registered as `Scoped` by default when they use EF Core `DbContext` or other scoped dependencies.

Use `Transient` only for lightweight stateless components where appropriate. Use `Singleton` only for truly application-wide components whose dependencies are also safe for singleton lifetime.

Do not create a generic god `IDomainComponent`, repository-per-table layer, or a separate Domain project.


## Permission access types
Authorization is modeled as Permission + AccessType. AccessTypes are VIEW, CREATE, EDIT, DELETE, and EXPORT. RolePermissions uses (RoleId, PermissionId, AccessTypeId). JWT contains permission_access claims such as `TagReport.View:VIEW`. Angular receives the same structured permission/access list and uses it for navigation and route guards; the .NET API enforces the same permission/access combination.
