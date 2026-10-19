# PLAT-001 Toolchain Matrix

**Status**: APPROVED by the technical owner on 2026-10-19.
**Ticket**: [PLAT-001](../../Dragon_English_Kids_SDD_Backlog/tickets/platform/PLAT-001-thiet-lap-monorepo-va-moi-truong-phat-trien.md)

| Category | Tool or dependency | Approved version | Status / approval |
|---|---|---|---|
| Runtime | Node.js LTS | 24.22.0 | APPROVED; Node 24 LTS |
| Runtime | .NET SDK | 10.0.402 | APPROVED; includes .NET and ASP.NET Core runtime 10.0.13 |
| Frontend framework | React | 19.3.1 | APPROVED |
| Frontend renderer | React DOM | 19.3.1 | APPROVED |
| Game framework | Phaser 3 | 3.90.0 | APPROVED; stable Phaser 3 line, not Phaser 4 |
| Frontend compiler | TypeScript | 7.0.2 | APPROVED |
| Frontend build tool | Vite | 8.3.4 | APPROVED; Node 24 meets engine requirement |
| Vite React integration | @vitejs/plugin-react | 6.1.2 | APPROVED; peer supports Vite 8 |
| Frontend test runner | Vitest | 5.0.5 | APPROVED; supports Vite 8 and Node 24 |
| JavaScript/TypeScript linter and formatter | Biome | 2.5.15 | Stable; Node 24 supported; TypeScript/TSX supported |
| React type definitions | @types/react / @types/react-dom | 19.3.0 / 19.3.0 | APPROVED for React 19.3.1 |
| Node type definitions | @types/node | 24.19.2 | APPROVED for Node 24 |
| JavaScript package manager | pnpm | 12.14.0 | APPROVED; exact packageManager pin required |
| Backend framework | ASP.NET Core | 10.0.13 | APPROVED; bundled with .NET SDK 10.0.402 |
| Data access | Entity Framework Core | 10.0.13 | APPROVED |
| PostgreSQL provider | Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.3 | APPROVED; supports EF Core >=10.0.4 and <11.0.0 |
| API documentation package | Microsoft.AspNetCore.OpenApi | 10.0.13 | APPROVED |
| API test framework | MSTest | 4.5.1 | APPROVED |
| API test adapter | MSTest.TestAdapter | 4.5.1 | APPROVED |
| .NET test host | Microsoft.NET.Test.Sdk | 18.11.0 | APPROVED |
| Database | PostgreSQL | 17.11 | APPROVED; current PostgreSQL 17 minor release |
| Database migration tool | dotnet-ef | 10.0.13 | APPROVED; local tool manifest |
| Secret scanner | Gitleaks | 8.30.1 | APPROVED; pin exact release in CI |
| OpenAPI specification | OpenAPI | 3.1.0; contract 1.0.0 | APPROVED; see `contracts/health.openapi.yaml` |
| CI provider | GitHub Actions | Existing repository baseline | APPROVED; required checks still need implementation |

## Approval Record

The technical owner approved the baseline and exact versions in this matrix on 2026-10-19. The
readiness behavior is approved in [health-readiness.md](../../specs/001-developer-environment-setup/contracts/health-readiness.md).
Do not replace these pins with floating tags or ranges. Refresh patch versions only through an
explicit technical review and update associated lock files in the same change.