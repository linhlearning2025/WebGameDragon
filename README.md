# Dragon English Kids

## Local Development

PLAT-001 uses a native development environment. PostgreSQL 17 can be installed on the workstation
or hosted on an authorized development database.

### Prerequisites

- Git
- Node.js 24.22.0
- pnpm 12.14.0
- .NET SDK 10.0.402
- PostgreSQL 17.11 installed locally, or credentials for a hosted PostgreSQL 17 development database

Exact approved versions are listed in [the toolchain matrix](docs/eng/toolchain-matrix.md).
CI verification scenarios are documented in [tests/ci/README.md](tests/ci/README.md).

Install pnpm with `npm install --global pnpm@12.14.0`. The root `.node-version` and `global.json`
select the approved Node.js and .NET SDK versions.

### PostgreSQL

Install PostgreSQL 17.11 using the workstation's package installer, or provision an authorized
development database. Create a development database and least-privileged user through the local
PostgreSQL administration tools or hosted provider console. Do not use production credentials.

Provide the connection string through the current shell or an untracked local environment file:

```powershell
$env:ConnectionStrings__Default = 'Host=localhost;Port=5432;Database=dragon_english_kids;Username=dragon_dev;Password=<set-locally>'
```

For hosted PostgreSQL, replace the host and database credentials with the development endpoint's
values. Never commit the real connection string.

### Privacy Settings

The API refuses to start without a secret key used to pseudonymize child references in audit
events. Generate a random value locally and never commit it:

```powershell
$env:Privacy__AuditPseudonymKey = '<set-locally-random-secret>'
```

Microphone speaking is off by default (`Privacy__SpeechEnabled=false`) and stays off until a speech
approach is approved. In Development the policy values have proposed defaults; in every other
environment these must be set explicitly because they are pending approval: `Privacy__SpeechLeaseSeconds`,
`Privacy__GuestLifetimeHours`, `Privacy__GuestDeletionDelayHours`, `Privacy__PrivacyRequestTargetDays`,
and `Privacy__BackupPurgeDays`. See [docs/privacy](docs/privacy/) for the data inventory, processor
checklist, speech-processing notes, and audit-access procedure.

### Restore and Migrate

From the repository root, restore the pinned dependencies and EF migration tool:

```powershell
pnpm install --frozen-lockfile
dotnet tool restore
dotnet restore apps/api/tests/DragonEnglishKids.Api.Tests/DragonEnglishKids.Api.Tests.csproj --locked-mode
```

After the API's `DbContext` and migrations are implemented, apply migrations to the configured
development database (the `PrivacyFoundation` migration creates the consent, speech session,
privacy request, audit, and retention tables):

```powershell
dotnet ef database update --project apps/api/DragonEnglishKids.Api.csproj --startup-project apps/api/DragonEnglishKids.Api.csproj
```

Migration details and hosted-database notes are in [the development setup guide](docs/eng/dev-setup.md).

### Start the Applications

Once each application entry point is implemented, run from the repository root:

```powershell
pnpm --filter @dragon-english-kids/web dev
dotnet run --project apps/api/DragonEnglishKids.Api.csproj
pnpm --filter @dragon-english-kids/content-pipeline dev
```

The API contract defines `GET /health/live` as HTTP 200 while the process is alive and
`GET /health/ready` as HTTP 200 when PostgreSQL is reachable, otherwise HTTP 503. Health responses
must never expose secrets. See
[health-readiness.md](specs/001-developer-environment-setup/contracts/health-readiness.md).