# PLAT-001 Research

**Date**: 2026-10-19
**Status**: Technical baseline approved 2026-10-19; implementation and verification are in progress.

## Repository Layout

**Decision**: Use one repository with `apps/web`, `apps/api`, and `tools/content-pipeline`, plus
shared `tests`, `docs`, and `scripts` directories.

**Rationale**: The user selected one repository during clarification. The ticket proposes these
three component paths, and one clean-checkout setup can coordinate the browser app, API, and
content tools without introducing cross-repository setup dependencies.

**Alternatives considered**: Separate repositories were rejected by the accepted clarification.
No package-by-package repository split is proposed.

## Toolchain Version Policy

**Decision**: Use the exact technical-owner-approved versions in
`docs/eng/toolchain-matrix.md`, pin runtimes in `.node-version` and `global.json`, JavaScript
dependencies in `package.json`/`pnpm-lock.yaml`, and NuGet dependencies in project files and
`packages.lock.json`.

**Rationale**: Exact pins make clean checkouts reproducible. Node 24.22.0 satisfies the declared
engines for Vite 8.3.4, `@vitejs/plugin-react` 6.1.2, and Vitest 5.0.5. Npgsql EF provider 10.0.3
requires EF Core >=10.0.4 and <11.0.0; EF Core 10.0.13 satisfies that range. SDK 10.0.402 includes
ASP.NET Core runtime 10.0.13.

**Alternatives considered**: Floating tags and semver ranges in the root version-selection surface
were rejected because they do not make the selected baseline reproducible. Phaser 4 was not selected;
Phaser 3.90.0 is the latest stable release in the requested Phaser 3 line.

## Continuous Integration

**Decision**: Use the approved GitHub Actions baseline and expand the existing
`.github/workflows/ci.yml` placeholder to run the approved checks, with Gitleaks 8.30.1 for secret
scanning.

**Rationale**: The repository already contains a GitHub Actions workflow triggered on push and
pull request. PLAT-001 expands its placeholder with pinned toolchain setup, native PostgreSQL,
build/lint/test checks, required setup/OpenAPI document guards, and secret scanning; there are no
competing CI definitions.

**Alternatives considered**: Introducing another CI provider is not justified by current repository
evidence and would add setup work without meeting an additional ticket requirement.

## PostgreSQL and Secrets

**Decision**: Support PostgreSQL 17 installed locally or hosted externally. Read the connection
string from `ConnectionStrings__Default` (or the equivalent secret environment injection) and use
mock or local media for development; production R2 credentials are not needed for the default path.
Keep secret values out of tracked files and redact configuration values from startup and scan output.

**Rationale**: This directly follows PLAT-001's local database/mock-storage requirement and the
constitution's privacy and secret-handling rules.

**Alternatives considered**: Requiring a developer's production R2 account was rejected because it
would expose unnecessary credentials and make clean setup depend on production access. Local and
hosted PostgreSQL use the same environment-variable contract.

## Health Contract

**Decision**: Implement the approved read-only `GET /health/live` and `GET /health/ready`
operations. `/health/live` returns 200 while the process is alive. `/health/ready` returns 200 when
PostgreSQL is reachable and 503 otherwise. The versioned OpenAPI 3.1 document is
`contracts/health.openapi.yaml`, contract version 1.0.0.

**Rationale**: This follows the technical-owner-approved contract. Health responses disclose no
secrets or child data, and required environment variables fail fast at startup.

**Alternatives considered**: Making liveness depend on PostgreSQL was rejected because it cannot
distinguish a stopped process from a running process with an unavailable dependency. No product
endpoint or product data contract is introduced.

## Verification Strategy

**Decision**: Use project-level scripts to expose consistent setup, lint, test, and build operations;
run frontend and API checks in CI, add a database-backed readiness smoke check, and run secret
scanning on proposed changes.

**Rationale**: The ticket requires these checks and evidence for setup from a clean checkout. The
CI workflow already acts as the natural pull-request gate.

**Alternatives considered**: Manual-only verification was rejected because it cannot reliably
prevent a failed build/test or committed secret from appearing as a passing pull request.

## Approval Record

- PLAT-001 WHAT/scope, exact versions, and health contract approved by the technical owner on
	2026-10-19.
- The lockfiles and runtime configuration must be generated from these exact pins and verified
	before dependent implementation tasks are marked complete.