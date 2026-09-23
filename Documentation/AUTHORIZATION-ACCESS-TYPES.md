# Authorization: Permission + AccessType

The authorization model is intentionally two-dimensional:

- **Permission** = what page/function the user can access.
- **AccessType** = what operation the user can perform.

## Access types

| Code | Meaning |
|---|---|
| VIEW | View/read |
| CREATE | Add/create |
| EDIT | Modify/update |
| DELETE | Remove/delete |
| EXPORT | Export/download |

## Database relationship

```text
Users
  -> UserRoles
      -> Roles
          -> RolePermissions
              -> Permissions
              -> AccessTypes
```

`RolePermissions` primary key is `(RoleId, PermissionId, AccessTypeId)`.

## Role matrix

| Page / Function | Ground Floor | Supervisor | Site Manager | Admin |
|---|:---:|:---:|:---:|:---:|
| Summary Dashboard | VIEW | VIEW | VIEW | VIEW |
| SLA Dashboard | — | VIEW | VIEW | VIEW |
| Device & System Status | — | — | VIEW | VIEW |
| Device Details | — | — | VIEW | VIEW |
| Bag Journey | — | — | VIEW | VIEW |
| Tag Report | — | — | VIEW | VIEW |
| Administration | — | — | — | VIEW |
| Users | — | — | — | VIEW / CREATE / EDIT / DELETE |
| Roles | — | — | — | VIEW / EDIT |
| Sessions | — | — | — | VIEW |
| Audit Log | — | — | — | VIEW |

JWT contains claims such as `permission_access = TagReport.View:VIEW`. The Angular UI uses the same data for navigation/guards, while the .NET API validates the permission/access combination for protected endpoints and SignalR.
