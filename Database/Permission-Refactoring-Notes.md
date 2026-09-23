Use only `Install-EnTrackBag-New-Tables-And-Admin.sql` following `README.md`. It includes permission normalization, preserves IDs and grants, and retains the exact SLA code `Dashboard.SLA.View`. Conflicting old/new codes stop deployment for review.

The single consolidated script supports fresh identity installation and existing identity upgrades. No separate refactoring script is needed.

Sign out and sign in after deployment to refresh permission claims in the token and browser storage. Previously issued tokens contain the old codes and will no longer authorize renamed features.

The consolidated script has not been executed or database-tested during consolidation. Existing permission IDs are preserved; seed IDs are not hardcoded. No operational MFT tables are changed. Verification below is historical, not verification of this consolidated version.

Local verification (2026-09-16): .NET Release build and Angular build passed. The migration was run twice inside a rolled-back transaction, then applied successfully. Admin login, users, roles and configuration GET returned HTTP 200; unauthenticated configuration access returned 401; VIEW-only and unrelated grants were rejected with 403 for configuration PUT. The UI responded on port 4200.

Separate existing configuration issue: SLA GET returns 500 because SystemSettings lacks a valid SlaThresholdMinutes value. The business-approved threshold must be supplied before SLA data can be verified. No default operational threshold was invented.
