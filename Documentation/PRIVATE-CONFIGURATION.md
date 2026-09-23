# Private deployment configuration

This repository starts with a clean snapshot history. Local `appsettings*.json` files, secrets, caches and generated build files are intentionally excluded. Obtain private configuration securely from the deployment owner; do not commit it.

Configure both APIs through local ignored settings files, development user-secrets or deployment environment variables. Required settings include `ConnectionStrings__BLTSMFT`, `Jwt__Key`, `Jwt__Issuer` and `Jwt__Audience`. Identity also requires `Security__EncryptionKey` for the existing passport-protection implementation. Keep token issuer/audience/signing configuration consistent across both APIs. Configure `Frontend__Origin` and hosting endpoints for each environment.

Do not reuse previously exposed credentials. Coordinate rotation, especially encryption keys: replacing an encryption key without an approved data migration can make existing encrypted data unreadable.

No application builds or tests were executed during publication. Tests that read local appsettings require a securely supplied local configuration.
