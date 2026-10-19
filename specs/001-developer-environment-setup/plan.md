# Implementation Plan: PLAT-001 — Developer Environment Setup

**Branch**: `001-developer-environment-setup` | **Date**: 2026-10-19 | **Spec**: [spec.md](spec.md)

**Input**: PLAT-001, "Thiết lập monorepo và môi trường phát triển"

## Summary

Establish a reproducible local development environment for the React/Phaser web application,
ASP.NET Core API, and content-authoring tools in the approved single repository. Pin the approved
Node.js 24, .NET 10, React/Vite/Phaser, PostgreSQL 17, and pnpm versions in configuration and lock
files. Use locally installed or hosted PostgreSQL and mock/local media; implement the approved
liveness/readiness contract; and expand GitHub Actions with build, lint, test, and Gitleaks gates.

## Technical Context

**Language/Version**: TypeScript 7.0.2 / JavaScript on Node.js 24.22.0; C# on .NET SDK 10.0.402
with ASP.NET Core runtime 10.0.13.

**Primary Dependencies**: React/React DOM 19.3.1, Phaser 3.90.0, Vite 8.3.4,
`@vitejs/plugin-react` 6.1.2, pnpm 12.14.0, ASP.NET Core/EF Core 10.0.13,
Npgsql EF provider 10.0.3, PostgreSQL 17.11, dotnet-ef 10.0.13, and Gitleaks 8.30.1.
Local media use MUST NOT require production R2 credentials.

**Storage**: PostgreSQL 17.11, installed locally or hosted externally; mock or local filesystem media for development.
Production R2 remains outside this feature's runtime setup.

**Testing**: Vitest 5.0.5 frontend tests; MSTest 4.5.1 with Microsoft.NET.Test.Sdk 18.11.0 for API
tests; readiness integration check with PostgreSQL available and unavailable; Gitleaks 8.30.1;
documented clean-checkout smoke test.

**Target Platform**: Developer workstations and pull-request CI. Supported workstation operating
systems and their setup commands are to be documented in the approved toolchain matrix.

**Project Type**: Single-repository web application, backend API, and content-authoring tools.

**Performance Goals**: No throughput or latency target is defined for this foundation ticket.
Readiness MUST reflect database availability and startup MUST fail promptly on missing required
configuration; numeric timing targets are not introduced by this plan.

**Constraints**: No committed secrets or production credentials; PostgreSQL settings come from
environment variables; required missing settings fail
fast without value disclosure; `/health/live` returns 200 while the process is alive;
`/health/ready` returns 200 only when PostgreSQL is reachable and 503 otherwise. GitHub Actions is
the approved CI provider; checks fail closed.

**Scale/Scope**: One developer's local environment and pull-request validation for the initial
repository foundation; production deployment and production-scale capacity are out of scope.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle / gate | Status | Plan evidence |
|---|---|---|
| Child-first UX and privacy by design | Pass | Local setup uses no child production data or credentials; secrets are excluded from logs and check output. |
| Backend as source of truth | Pass | Readiness is reported by the API; the web client does not declare persistence successful without backend confirmation. |
| Domain ownership | Pass | Platform setup owns toolchain and local orchestration; it does not move product rules or data between domains. |
| API versioning, idempotency, and atomicity | Pass with scope note | The approved OpenAPI document is versioned; liveness/readiness routes are read-only operational endpoints and introduce no mutation or reward behavior. |
| Spec-before-code and automated testing | Pass | PLAT-001 scope, technical baseline, and readiness behavior are approved; implementation and verification tasks remain open. |
| React-Phaser separation | Pass | Repository layout keeps the React DOM host and Phaser presentation package distinct; this feature does not implement their event bridge. |
| Stack and storage guardrails | Pass | Uses the approved React, Phaser 3, ASP.NET Core, PostgreSQL/EF Core, and R2 stack; JSONB and production R2 changes are not introduced. |

**Post-design recheck**: Pass. The approved versioned readiness contract is read-only and contains
no child data or secrets; the operational data model introduces no product persistence; the pinned
stack preserves React and Phaser separation.

No constitution violation is required. PLAT-001 approval does not imply that any implementation
task is complete; each remains open until implemented and verified.

## Project Structure

### Documentation (this feature)

```text
specs/001-developer-environment-setup/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── health-readiness.md
│   └── health.openapi.yaml
└── tasks.md                 # Produced by /speckit-tasks
```

### Source Code (repository root)

```text
apps/
├── web/                     # React DOM host and Phaser integration boundary
└── api/                     # ASP.NET Core API and EF Core persistence
tools/
└── content-pipeline/        # Content-authoring and validation tools
tests/
├── web/
├── api/
└── integration/
docs/
├── eng/
│   └── toolchain-matrix.md  # Approved tool versions and clean setup instructions
└── contracts/
    └── api.md               # Optional reviewed API contract index
scripts/
└── dev/                     # Cross-platform local setup and verification scripts
.github/workflows/ci.yml     # Existing GitHub Actions workflow, expanded from placeholder
README.md                    # Native local setup, migrations, and application startup
```

**Structure Decision**: Use the single-repository layout accepted in clarification and proposed
by PLAT-001. Keep browser UI, API, and content tools in distinct directories; keep cross-project
smoke tests in `tests/integration`. The proposed directories align with the ticket's proposed
`apps/web`, `apps/api`, and `tools/content-pipeline` paths. Exact runtime and package pins are
recorded in `docs/eng/toolchain-matrix.md`; script names are established by the workspace setup.

## Complexity Tracking

No constitution violations or additional architectural layers are proposed. The `tests/` and
`scripts/` directories are justified by the ticket's cross-project verification and repeatable
setup requirements.
