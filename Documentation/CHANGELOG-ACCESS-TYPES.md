# Access Type Authorization Change

This version updates the EnTrackBag source, database script, and prompt to use the approved four user types and Permission + AccessType authorization model.

## User types
- Ground Floor: Summary Dashboard only
- Supervisor: Summary Dashboard + SLA Dashboard
- Site Manager: everything except Administration
- Admin: everything including Administration

## Access types
- VIEW
- CREATE
- EDIT
- DELETE
- EXPORT

## Source changes
- Added `AccessTypeEntity` and EF configuration.
- Changed identity IDs to match the final INT/BIGINT SQL schema.
- Changed `RolePermissions` key to `(RoleId, PermissionId, AccessTypeId)`.
- Login response now returns structured permission/access pairs.
- JWT contains `permission_access` claims such as `TagReport.View:VIEW`.
- .NET API authorization validates Permission + AccessType.
- Angular navigation and route guards use the same permission/access data.
- Updated role names from Floor Operator/Super User to Ground Floor/Site Manager.

## Database changes
- Added `dbo.AccessTypes`.
- Updated `dbo.RolePermissions` to include `AccessTypeId`.
- Seeded the four approved roles.
- Seeded VIEW permissions for page access and administration management access types.

No existing BLTSMFT operational tables are recreated or modified.
