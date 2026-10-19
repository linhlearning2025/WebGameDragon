# PLAT-001 Quickstart Validation

This guide defines the clean-checkout and failure checks used to verify PLAT-001 after
implementation. Exact runtime versions and command prerequisites come from the owner-approved
`docs/eng/toolchain-matrix.md`; no version numbers are selected here.

## Prerequisites

- Git and the approved Node.js and .NET SDK versions from the toolchain matrix.
- PostgreSQL 17.11 installed locally or a hosted PostgreSQL 17 endpoint and authorized credentials.
- No production database or Cloudflare R2 credentials. Local media mode is used by default.

## Clean Local Setup

1. Check out the repository and read `docs/eng/toolchain-matrix.md`. Confirm Node.js `24.22.0`,
   pnpm `12.14.0`, and .NET SDK `10.0.402` are selected.
2. Create the local environment file from `.env.example` using the command appropriate to the
   workstation. Do not put production secrets in the file.
3. Set `ConnectionStrings__Default` in the current shell or a local untracked environment file.
   Keep credentials out of committed files and logs.
4. Install the root JavaScript workspace dependencies with `pnpm install --frozen-lockfile`.
5. Restore the local migration tool with `dotnet tool restore`; apply migrations with the
   documented EF Core command.
6. Start the web application with the documented root development script.
7. Start the API using the documented API development script.
8. Request `GET /health/live` and confirm HTTP 200 while the API process is alive, including while
   PostgreSQL is unavailable. Request
   `GET /health/ready`; it must return HTTP 200 while PostgreSQL is available and HTTP 503 when it
   is unavailable. See [health-readiness.md](contracts/health-readiness.md) and
   [health.openapi.yaml](contracts/health.openapi.yaml).
9. Follow the documented content-tool usage path without configuring production media credentials.

**Expected result**: the documented setup reaches a usable local web/API environment, API
readiness reports PostgreSQL availability accurately, and content-tool setup is documented.

## Failure Checks

### Required configuration is missing

1. Stop the API and remove the required local database setting from the local environment file.
2. Start the API using the documented command.

**Expected result**: startup fails and names the missing setting without printing its value.

### Database is unavailable

1. Stop the local PostgreSQL service using the operating system's service manager, or make the
   hosted PostgreSQL endpoint temporarily unreachable in a test environment.
2. Request `GET /health/ready`.

**Expected result**: readiness returns not-ready with a service-unavailable status; no connection
string or secret appears in the response or logs.

### Pull-request checks

1. Open a pull request with a change that passes the required checks.
2. Verify that build, lint, automated tests, readiness smoke verification, and secret scanning
   report explicit outcomes.
3. In a controlled test fixture, introduce a known failing check and a recognized dummy secret
   pattern; do not use a real credential.

**Expected result**: failed required checks block a passing result, and secret scan output does not
print the detected value.

## Commands to Run After Implementation

Use the exact commands recorded in the root setup guide and toolchain matrix. At minimum, run the
root build, lint, and test scripts; API tests; and liveness/readiness smoke checks. The PR workflow must
run the same required checks or an explicitly documented equivalent.