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

### Restore and Migrate

From the repository root, restore the pinned dependencies and EF migration tool:

```powershell
pnpm install --frozen-lockfile
dotnet tool restore
dotnet restore apps/api/DragonEnglishKids.Api.csproj --locked-mode
```

After the API's `DbContext` and migrations are implemented, apply migrations to the configured
development database:

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