# EnTrackBag — Master Prompt

Updated 23 September 2026. This is the single maintained project prompt, consolidating the former baseline, final-project and handover prompts. Latest explicit user decisions override older proposals; implementation status below is not a fresh code audit.

Continue work on the existing EnTrackBag application. Do not recreate the application or assume all historical changes have been tested. Inspect the relevant local code before modifying it, preserve existing user edits, and distinguish completed source changes from pending integrations.

## Project context

- Workspace: `C:\Users\Manauwar\Documents\ChatGPT\EnTrackBag`
- Main repository (use only this remote): https://github.com/aerotrackit-spec/EnTrackBag_codex.git
- Legacy reference: https://github.com/manauwartrackit-wq/BLTS_DB_Init.git
- Frontend: Angular 19 under `UI`.
- Backend: .NET 10 APIs under `Identity.Api` and `EnTrackBag.Api`.
- Existing SQL Server database: `BLTSMFT`.
- Existing live-update hub: `/hubs/monitoring`.
- Preserve the EnTrackBag light theme, branding, and approved Bag Journey Configuration layout.

## Working constraints

- Make small, scoped changes; inspect only relevant files.
- Preserve unrelated local changes and existing operational data.
- Do not build, test, run or restart without renewed authorization. Follow the Git workflow below when delivery is authorized; never merge. The latest layout changes have not been built or run.
- Inspect actual database schema before changing database-related code. Do not invent columns or tables.
- Do not use EF migrations, `EnsureCreated()`, or `Database.Migrate()`.
- Maintain only the consolidated application SQL script described below; obtain clear authorization before executing database changes.
- Never add fake/demo operational records or hardcoded example counts.
- Report changed files, a short diff summary, and verification limitations accurately.

## Git workflow

- Baseline: master/main/current default. Integration branch: `dev`.
- Start each task from latest `dev`, then create `feature/<short-task-name>`.
- Never commit work directly to master or dev; never directly push task changes there.
- Stage relevant files only; use `type(scope): clear description` commits.
- When authorized to deliver, push the feature branch and open one PR into `dev`; stop without merging and remain on the feature branch.
- Preserve existing staged/unstaged work. Do not mix unrelated tasks into a commit.
- Current SQL consolidation branch: `feature/consolidate-development-sql`, created from local dev. Remote branch discovery returned no heads, so latest remote dev could not be fetched. Baseline/integration setup and PR remain pending; do not claim they exist remotely.

## Architecture rules retained from earlier prompts

- Preserve the existing three-application solution: Identity.Api, EnTrackBag.Api and Angular UI. Do not rebuild it as a new greenfield project or move identity to a separate database based on superseded proposals.
- Use database-first EF Core mappings reflecting actual keys, types, nullability and relationships.
- Dependency flow: Controller → feature-specific DomainComponent interface/implementation → feature-specific Repository interface/implementation → DbContext → BLTSMFT.
- Controllers handle HTTP, domain components orchestrate business behavior, repositories handle queries. Avoid generic repositories or one wrapper per table without a feature need.
- Explicit suffixes: Entity, Dto, Repository, DomainComponent, Controller; Angular services use Service.
- Use ordinary constructor injection, not primary constructors, for controllers/repositories/domain components/DbContexts. Default to normal classes; do not add sealed without a reason.
- Keep EF configuration separate where practical; scoped DbContexts, repositories and dependent domain components. Keep Angular code feature-focused.
- Identity.Api issues JWTs; EnTrackBag.Api validates tokens locally. Secure HTTP and SignalR with the same identity and permission checks. Hub subscriptions must not bypass feature authorization.
- Both APIs require safe global exception handling, correlation IDs, structured logging and EnTrackBagExceptions persistence; never expose stack traces or let logging mask the original exception.
- Development endpoints historically use Identity API port 5200 and operational API/hub port 5100. Verify current configuration; use environment-based URLs, not hardcoded production values.
- Retry only appropriate transient HTTP failures, at most three total attempts; never blindly retry authentication failures or non-idempotent operations.
- Existing Tagging Station, DataProcessor_S1 and CS_ADG2 operational processing remains unchanged. Do not invent power, camera, PROFINET, queue or restart telemetry.
- Reader ports are not physical antenna counts; use verified configured antenna records and ReaderID relationships. Tagging Station has no applicable RFID antenna values.
- Preserve prominent Summary/SLA operational maps, agreed KPI cards and right-side bag history. SLA Monitoring should use full available width. No Exception Dashboard UI is required.
- On-prem deployment remains the target; deployment proposals are not evidence that installation packages or production verification are complete.

## Single application database deployment script

- Only SQL entry point: `Database/Install-EnTrackBag-New-Tables-And-Admin.sql`.
- Never edit, replace or rerun the original **MFT Script Latest** as part of application deployment; the operational master schema already exists in staging/production.
- Nine application-owned tables: Roles, Permissions, AccessTypes, Users, UserRoles, RolePermissions, UserSessions, AuditEvents, EnTrackBagExceptions.
- The consolidated script creates missing tables, reconciles profile/session columns, adds indexes, normalizes legacy permission codes, seeds Admin and relationships, expires stale sessions and revokes duplicate active sessions without deleting history.
- Six superseded application SQL scripts were removed and remain recoverable from Git. Add future approved application schema changes to this single script, not another deployment patch.
- Existing user passwords/profiles and operational records are preserved. New Admin creation requires a generated PBKDF2-HMAC-SHA512 hash; never embed plaintext credentials.
- Run manually in SSMS SQLCMD Mode (`:ON ERROR EXIT`), with API stopped and a database backup. Multiple committed phases are rerunnable, not one atomic deployment.
- The consolidated script has not been executed or database-tested. Validate in staging before production; do not claim production readiness based on static review alone.

## Earlier work and requirements to preserve

These areas were addressed earlier in the task history; verify their current implementation before extending them. This document is not a fresh code audit or proof of deployment.

### Administration and identity

- Existing identity/access schema integrates with BLTSMFT; do not replace operational tables.
- User form requires User Name, First Name, Last Name, Email, Password and Confirm Password when creating a user.
- Employee Code, Passport details, Nationality and Designation were subsequently made optional in the form requirements; Designation must remain optional.
- Password minimum length: 8 characters. Use password confirmation cross-field validation.
- Do not show mismatch errors while confirmation is empty; use required validation when appropriate. When confirmation contains a value and differs, show `Passwords do not match.`
- Invalid forms must block both Submit clicks and Enter submission, without an API call.
- Email uses required and email validators; show errors after touched/dirty or submitted.
- Keep Active account; remove Temporary Password wording and Require password change from the requested form.
- Passport privacy direction: retain/display only the last four characters as agreed; never expose a full passport number in normal UI. Inspect the actual schema before further changes.
- Every user assigned the Admin role is protected, regardless of username/display name: View and Change Password only; no Edit, Delete, Disable or role changes. Admin role permissions are read-only. Enforce in Angular and .NET using UserRoles/Role relationships, not username matching. Do not rename any accounts.
- A later request mentioned Husain Ragib as an Admin; verify actual account/role data instead of assuming it was seeded.
- Password hashing requirement: PBKDF2-HMAC-SHA512; never plaintext or reversible password storage. Do not put credentials in this handover.
- The requested development deployment script must match the existing nine application-owned identity/access tables and seed relationships idempotently. Verify the current script and execution status before claiming completion.

### Access control

- Use exact permission code `Dashboard.SLA.View` and AccessType `VIEW`.
- Do not use `Dashboard.SLA` or `Dashboard.SLA.View.View`.
- Admin should have Summary and SLA access through correctly loaded permissions.
- Floor Operator/Rishi must not see, navigate to, or call SLA without permission.
- Enforce authorization in .NET, returning 403 for unauthorized SLA requests.
- Load SLA only when its page is selected, not during login or Summary initialization.
- Site Manager/Murli requires Bag Journey Configuration access according to approved role permissions; verify persisted grants.

### Sessions and performance

- One active session per user; revoke/expire previous active sessions at a new login, preserving history and duration.
- Expired, logged-out and revoked sessions must not contribute to active counts.
- Session creation belongs to login only, not normal requests, dashboards, polling, or SignalR.
- Five-minute inactivity timeout must be enforced on both client and server.
- Track meaningful activity locally; activity reporting at most once every 60 seconds.
- Scroll, mouse movement, background polling, automatic refresh, and SignalR keep-alives must not extend inactivity.
- On expiry: expire the session, reject further requests, clear Angular authentication, disconnect SignalR and navigate to Login.
- Avoid duplicate subscriptions, repeated activity/session calls and unnecessary polling.
- Lazy-load feature pages. Reuse one monitoring connection and one set of subscriptions.

### Branding and shared UI

- User supplied the EnTrackBag luggage/radio logo.
- Notification icon should be neutral with no notifications and colored when notifications exist.
- Settings/profile menu replaces direct logout and offers account details and logout.
- Preserve the user's `.topbar { min-height: 80px; }` change.
- Remove duplicate page headings/subtitles where applicable.
- The Device Status footer is shared across all signed-in pages: browser-local-time note and EnTrackBag copyright year. Avoid a duplicate Device Status footer.

### Bag Journey Configuration

- Preserve the client-approved layout, stages, arrows and input positions.
- CSS-only responsive/zoom fixes were requested; do not redesign the page.
- Preserve the user's `.bhs-branch .pair-box { top: 0px; }`.
- First threshold inputs use the pale yellow legend color; second threshold inputs use pale pink, overriding global white input backgrounds as necessary.

## Device & System Status — latest saved implementation

Relevant frontend files:

- `UI/src/app/device-status/device-status.component.ts`
- `UI/src/app/device-status/device-status.component.html`
- `UI/src/app/device-status/device-status.component.scss`

Relevant backend files:

- `EnTrackBag.Api/DomainComponents/DeviceStatusDomainComponent.cs`
- `EnTrackBag.Api/Data/Repositories/ReaderRepository.cs`
- `EnTrackBag.Api/Data/Repositories/IReaderRepository.cs`
- `EnTrackBag.Api/DTOs/DeviceDetailDto.cs`

### Data mapping already investigated

- Legacy logical-device types: 1 Tagging Station, 2 Tagging Read Point, 3 Dog House Airside, 4 Dog House Landside, 5 Exit Gate, 6 Inside Lounge, 7 Exit Lounge, 8 Recheck Station, 9 BHS Return Feed.
- Legacy device heartbeat threshold is one minute.
- Types 1 and 8 use LogicalDevice.LastConnected; other logical devices use the mapped reader heartbeat.
- Distinct logical-device/reader mappings prevent repeated antenna-map rows from duplicating devices.
- Logical device and reader joins, antenna port indicators and extra Device Details metadata were added using inspected schema.
- Device statuses are strictly Online or Offline. Error text remains separate and must not become a third status.

### Current charts and layout

- Keep four top KPI cards: Tagging Stations, Readers, Antennas and Controllers.
- Individual category donut cards were replaced with ONE grouped column chart.
- X-axis shows the nine existing logical-device categories.
- Green columns = Online; red columns = Offline; show values above columns.
- A category group remains clickable/keyboard accessible to filter Device Details.
- Use a shared vertical scale and horizontal chart scrolling where needed.
- Add ONE overall System Overview donut using the four KPI data groups:
  - Total = stations total + readers total + antennas total + controllers total.
  - Online = sum of the four online counts.
  - Offline = Total − Online.
  - Percentages are calculated dynamically.
- Do not calculate the overall donut from the nine logical categories, which can have a different grouping/denominator.
- Missing/incomplete totals display an availability message rather than fabricated zero counts. Zero devices is not a third status.
- Latest saved layout places Device Category and System Overview in one CSS grid row, approximately 80% / 20% (`4fr 1fr`), with a 16px gap.
- At widths below 1100px the two panels stack vertically.
- Overall donut is compact, approximately 140px maximum width.
- Device Details remains full width below both panels.
- Initial summary/details requests and existing SignalR updates remain; no new polling was introduced.

### Verification status

- Earlier API/Angular builds and device-status checks were reported successful before the instruction not to run code.
- Later binary-status, chart and responsive-layout changes were not built, tested or browser-verified.
- Do not claim the running API includes newer backend changes: it was not restarted as part of the latest work.

## Refresh controls

- Manual refresh/date controls were removed from Device Status and the Tag Report header.
- The shared Administration refresh button was removed.
- Search/Clear behavior remains; not every existing background refresh mechanism was removed.
- Do not assume that removing a button proves a page has a completed live-data integration.

## Tag Report — important pending integration

Files:

- `UI/src/app/tag-report/tag-report.component.ts`
- `UI/src/app/tag-report/tag-report.component.html`
- `UI/src/app/tag-report/tag-report.component.scss`

The modern Tag Report UI shell was created from the supplied screenshot:

- Tag ID, prohibited/normal choices, suspect alarm options, date range, Clear/Search.
- Results panel, empty state, export/pagination placeholders and history sidebar structure.
- Date/filter validation and honest API-not-connected messaging.

Search, real report results, export and full legacy history are NOT yet integrated. Do not represent these as completed or invent report data.

Legacy references still needed:

- `BLTS_DB_Init/frmTagReport.vb`
- `BLTS_DB_Init/frmTagHistory.vb`

Their paths were discovered, but contents were not successfully retrieved in the prior attempt. A download was blocked by the approval service. Respect that denial; ask for local source files or use a newly authorized access path rather than bypassing it.

- There was no dedicated Tag Report controller in the inspected backend.
- Existing dashboard bag history is not a verified equivalent of legacy report history and has separate authorization.
- Existing mapped `vwTags` and SuspectBag entities may support the report; inspect their relevant mappings and legacy logic before implementing classification/filtering.

## How to continue

1. Follow the user's next specific request; do not automatically resume all historical tasks.
2. Read only the relevant files and inspect their local diffs to preserve user edits.
3. Keep real-data calculations, binary statuses, permissions and existing theme intact.
4. Do not execute builds/tests/application code without renewed authorization.
5. Leave changes saved and summarize exactly what changed and what remains unverified.
