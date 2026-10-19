# PLAT-001 Native Development Setup

This guide configures a native development environment for the React/Phaser web app, ASP.NET Core
API, and content tools. PostgreSQL may run locally or be hosted. Production media credentials are
not required.

## Prerequisites

Install the exact versions in [toolchain-matrix.md](toolchain-matrix.md):

- Git
- Node.js 24.22.0
- pnpm 12.14.0
- .NET SDK 10.0.402
- PostgreSQL 17.11 locally, or access to a hosted PostgreSQL 17 development database

Install pnpm with the pinned version:

```powershell
npm install --global pnpm@12.14.0
```

The `.node-version` and `global.json` files select exact runtime versions. `global.json` requires
.NET SDK 10.0.402.

## PostgreSQL Configuration

Create a development database and a least-privileged development user using the PostgreSQL
administration method appropriate to the environment. Do not use a production account.

Set the ASP.NET Core connection string in the current shell or an untracked local environment
file. Never commit the real password:

```powershell
$env:ConnectionStrings__Default = 'Host=localhost;Port=5432;Database=dragon_english_kids;Username=dragon_dev;Password=<set-locally>'
```

For hosted PostgreSQL, use its hostname, port, database, username, and password instead. Restrict
network access to the development environment. The API MUST fail fast if this setting is missing;
neither logs nor health responses may disclose its value.

## Privacy Settings

The API also fails fast without `Privacy__AuditPseudonymKey`, the secret used to pseudonymize child
references in audit events. Set it in the shell or an untracked local environment file and never
commit it. `dotnet ef` starts the API host, so it needs this setting and the connection string.

```powershell
$env:Privacy__AuditPseudonymKey = '<set-locally-random-secret>'
```

| Setting | Default | Notes |
|---|---|---|
| `Privacy__SpeechEnabled` | `false` | Microphone speaking stays off until a speech approach is approved (QA-002 OD-3). |
| `Privacy__SpeechLeaseSeconds` | 15 (Development only) | Proposed; must be set explicitly outside Development. |
| `Privacy__GuestLifetimeHours` | 24 (Development only) | Proposed, pending approval; must be set explicitly outside Development. |
| `Privacy__GuestDeletionDelayHours` | 24 (Development only) | Proposed, pending approval; must be set explicitly outside Development. |
| `Privacy__PrivacyRequestTargetDays` | 30 (Development only) | Proposed, pending approval; must be set explicitly outside Development. |
| `Privacy__BackupPurgeDays` | 30 (Development only) | Proposed, pending approval; must be set explicitly outside Development. |

Outside Development the API also redirects HTTP to HTTPS and sends HSTS. Privacy documents are in
[docs/privacy](../privacy/).

## Restore Dependencies

From the repository root:

```powershell
pnpm install --frozen-lockfile
dotnet tool restore
dotnet restore apps/api/tests/DragonEnglishKids.Api.Tests/DragonEnglishKids.Api.Tests.csproj --locked-mode
```

The JavaScript graph is pinned by `pnpm-lock.yaml`; NuGet packages are pinned by central package
versions and each project's `packages.lock.json`.

## Database Migrations

After the API project defines its `DbContext` and migrations, apply migrations to the configured
development database with the pinned `dotnet-ef` tool:

```powershell
dotnet ef database update --project apps/api/DragonEnglishKids.Api.csproj --startup-project apps/api/DragonEnglishKids.Api.csproj
```

Generate a reviewed migration when the data model changes:

```powershell
dotnet ef migrations add <MigrationName> --project apps/api/DragonEnglishKids.Api.csproj --startup-project apps/api/DragonEnglishKids.Api.csproj
```

Never apply development migrations to production from this guide.

## Start the Applications

Once the corresponding application projects are implemented:

```powershell
pnpm --filter @dragon-english-kids/web dev
dotnet run --project apps/api/DragonEnglishKids.Api.csproj
pnpm --filter @dragon-english-kids/content-pipeline dev
```

The API health contract is documented in
[health-readiness.md](../../specs/001-developer-environment-setup/contracts/health-readiness.md)
and [health.openapi.yaml](../../specs/001-developer-environment-setup/contracts/health.openapi.yaml):

- `GET /health/live` returns HTTP 200 while the API process is alive.
- `GET /health/ready` returns HTTP 200 when PostgreSQL is reachable and HTTP 503 otherwise.
- Health responses contain no secrets.

API and UI startup becomes runnable after the implementation tasks add their application entry
points and readiness handlers.