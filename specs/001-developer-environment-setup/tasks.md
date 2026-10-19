# Tasks: PLAT-001 — Developer Environment Setup

**Input**: Design documents from `specs/001-developer-environment-setup/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`, and
`quickstart.md`

**Approval record**: The technical owner approved PLAT-001 scope, exact technical versions, and
health behavior on 2026-10-19. This approval does not complete implementation or verification work.

**Tests**: Included because the specification requires automated build, lint, and test checks plus
acceptance evidence for each scenario.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Tasks can run in parallel because they edit different files and have no dependency on
  unfinished tasks.
- **[Story]**: User-story label matching `spec.md`.
- Every task names the exact file path or paths to create, edit, or validate.

## Phase 1: Setup

**Purpose**: Establish approvals, pinned toolchain guidance, and the agreed repository skeleton.

- [X] T001 Create `docs/eng/toolchain-matrix.md` listing required runtimes, SDKs, package managers,
  test runners, and secret scanner with version/status columns; leave unapproved values marked
  pending and link the file from `Dragon_English_Kids_SDD_Backlog/tickets/platform/PLAT-001-thiet-lap-monorepo-va-moi-truong-phat-trien.md`.
- [X] T002 Record technical-owner approval of exact versions in `docs/eng/toolchain-matrix.md` and
  health behavior in `specs/001-developer-environment-setup/contracts/health-readiness.md` in the
  PLAT-001 ticket and technical artifacts. Approval recorded 2026-10-19.
- [X] T003 [P] Initialize the approved JavaScript workspace and project manifests in `package.json`,
  `pnpm-workspace.yaml`, `.node-version`, `global.json`, `apps/web/package.json`,
  `tools/content-pipeline/package.json`, `apps/api/DragonEnglishKids.Api.csproj`, and
  `apps/api/tests/DragonEnglishKids.Api.Tests/DragonEnglishKids.Api.Tests.csproj`; generate
  `pnpm-lock.yaml` and NuGet `packages.lock.json` files using only approved versions from
  `docs/eng/toolchain-matrix.md`.
- [X] T004 [P] Add local environment examples and secret exclusions in `.env.example` and
  `.gitignore`; include setting names only, with no credential values.

---

## Phase 2: Foundational

**Purpose**: Provide shared setup commands and database migration guidance; PostgreSQL is installed
locally or hosted outside the repository.

- [X] T005 Add a local .NET tool manifest at `.config/dotnet-tools.json` pinning `dotnet-ef` 10.0.13,
  and document database creation, connection-string environment setup, and migration commands for
  locally installed or hosted PostgreSQL in `README.md` and `docs/eng/dev-setup.md`.
- [X] T006 Add cross-platform local setup commands in `scripts/dev/setup.ps1` and
  `scripts/dev/setup.sh` to validate pinned runtimes, install locked dependencies, restore .NET
  tools, validate required database environment variables, and print startup/migration commands;
  these scripts MUST NOT install or start a database service.
- [X] T007 Add root development scripts for build, lint, test, web startup, API startup, and content
  tool startup in `package.json`; ensure each script delegates to the corresponding workspace path.

**Checkpoint**: Owner approvals are recorded, toolchain versions are pinned, and local setup
commands accept a configured PostgreSQL connection. User-story work may now proceed.

---

## Phase 3: User Story 1 — Start the Development Environment (Priority: P1) — MVP

**Goal**: A developer can prepare the web app, API, and content-authoring tools from a clean
checkout, use local PostgreSQL and mock/local media, and observe accurate API readiness.

**Independent Test**: Follow `specs/001-developer-environment-setup/quickstart.md` from a clean
checkout using PostgreSQL installed locally or hosted, without production media credentials; verify
setup, migration, missing-configuration, and readiness behavior.

### Tests for User Story 1

- [X] T008 [P] [US1] Add API startup tests for missing required configuration and safe error output in
  `apps/api/tests/DragonEnglishKids.Api.Tests/StartupConfigurationTests.cs`.
- [ ] T009 [P] [US1] Add health contract tests for `/health/live` plus ready/unready PostgreSQL
  probe outcomes in `apps/api/tests/DragonEnglishKids.Api.Tests/ReadinessEndpointTests.cs` and an
  environment-gated real PostgreSQL probe test in
  `apps/api/tests/DragonEnglishKids.Api.Tests/PostgresReadinessIntegrationTests.cs`; verify using
  the approved contract at `specs/001-developer-environment-setup/contracts/health-readiness.md`.

### Implementation for User Story 1

- [X] T010 [US1] Implement API startup configuration validation and fail-fast diagnostics in
  `apps/api/Program.cs` and `apps/api/appsettings.Development.json`; report missing setting names
  without values.
- [X] T011 [US1] Implement the approved read-only readiness operation in
  `apps/api/Program.cs` and `apps/api/Health/HealthResponses.cs`; return ready only when
  PostgreSQL is reachable and never include connection details.
- [X] T012 [P] [US1] Add the React development entry point and isolated Phaser scene mount/unmount
  boundary in `apps/web/src/main.tsx`, `apps/web/src/App.tsx`, and
  `apps/web/src/game/PhaserHost.tsx`; add the backend-confirmation request helper and tests in
  `apps/web/src/api/request.ts` and `apps/web/src/api/request.test.ts`; configure the workspace in
  `apps/web/index.html`, `apps/web/tsconfig.json`, and `apps/web/vite.config.ts`. Do not move
  persistent state or authorization into the client.
- [X] T013 [P] [US1] Add a runnable content-authoring tool entry point and local usage command in
  `tools/content-pipeline/src/index.ts` and `tools/content-pipeline/package.json`.
- [X] T014 [US1] Document Node.js, pnpm, .NET SDK, local/hosted PostgreSQL, environment variables,
  EF Core migrations, and web/API/content-tool startup in the root `README.md` and
  `docs/eng/dev-setup.md`; link to `docs/eng/toolchain-matrix.md`.
- [ ] T015 [US1] Execute AC-01 through AC-04 and AC-08 from a clean checkout and record commands, expected
  outcomes, and evidence in `specs/001-developer-environment-setup/quickstart.md`.

**Checkpoint**: User Story 1 passes independently without production credentials.

---

## Phase 4: User Story 2 — Verify a Change (Priority: P2)

**Goal**: Pull requests report build, lint, test, readiness, and secret-scan outcomes; any failed
required check prevents the workflow from passing.

**Independent Test**: Submit a change that passes, then use a controlled failing check and a
synthetic secret fixture to verify that required checks fail closed and scanner output redacts the
synthetic value.

### Tests for User Story 2

- [X] T016 [US2] Document the passing workflow case, intentionally failing required-check case, and
  synthetic secret-scan/redaction case in `tests/ci/README.md`; ensure the procedure never uses a
  real credential and can be run without committing the synthetic fixture.

### Implementation for User Story 2

- [X] T017 [US2] Expand `.github/workflows/ci.yml` to run the approved toolchain checks, web/API
  build, lint and tests, native PostgreSQL readiness integration check, and approved secret scanner;
  make every required job fail the workflow when it fails and prevent secret values from appearing
  in logs.
- [ ] T018 [US2] Run the passing, failing-check, and synthetic secret-scan scenarios from
  `tests/ci/README.md`; record workflow run references and results in
  `specs/001-developer-environment-setup/quickstart.md`.

**Checkpoint**: User Story 2 independently demonstrates clear pass/fail results for all required
checks without exposing a detected secret.


## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Verify end-to-end acceptance and keep the contributor entry points consistent.

- [X] T019 [P] Add links to the native setup guide, toolchain matrix, and CI verification guide in
  `README.md`.
- [ ] T020 Run all eight acceptance scenarios and the final clean-checkout quickstart; update the
  evidence and any deviations in `specs/001-developer-environment-setup/quickstart.md` and confirm
  `Dragon_English_Kids_SDD_Backlog/tickets/platform/PLAT-001-thiet-lap-monorepo-va-moi-truong-phat-trien.md`
  has the required approval and completion evidence before closing it.


## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: T001 creates the approval matrix; T002 is a hard gate for all implementation.
  T003 and T004 can proceed in parallel after T002.
- **Foundational (Phase 2)**: Depends on T002; T005 documents the native PostgreSQL/migration
  setup, T006 depends on T005, and T007 depends on the approved manifests from T003.
- **User Story 1 (Phase 3)**: Depends on Phases 1 and 2. T008 and T009 are test-first and can run
  in parallel; T010/T011 follow the API tests; T012 and T013 can run in parallel with API work;
  T014 documents the resulting commands; T015 validates the complete story.
- **User Story 2 (Phase 4)**: Depends on shared setup. T016 defines the validation procedure before
  T017 implements the workflow; T018 runs the scenarios after T017.
- **Polish (Phase 5)**: Depends on both desired user stories; T019 can run in parallel with the
  final acceptance run T020 if edits do not overlap.

### User Story Dependencies

- **US1 (P1)**: Can start after Setup and Foundational; it is the recommended MVP.
- **US2 (P2)**: Can start after Setup and Foundational; it may proceed in parallel with US1 after
  T016, but final workflow validation depends on the checks and scripts delivered by US1.

### Parallel Opportunities

- After T002: T003 and T004 edit separate files and can run in parallel.
- In Phase 3: T008 and T009 are separate test files; T012 and T013 are separate workspace projects.
- In Phase 4: T016 is a separate validation document and can be prepared while US1 implementation
  proceeds, provided shared toolchain approvals are complete.
- During Polish: T019 can run in parallel with T020 if no one is editing `README.md` concurrently.

## Parallel Example: User Story 1

```text
After T002-T007 are complete:
Task: T008 — API missing-configuration tests in apps/api/tests/DragonEnglishKids.Api.Tests/StartupConfigurationTests.cs
Task: T009 — API health tests in apps/api/tests/DragonEnglishKids.Api.Tests/ReadinessEndpointTests.cs
Task: T012 — React/Phaser startup boundary in apps/web/src/main.tsx and apps/web/src/game/PhaserHost.tsx
Task: T013 — Content tool startup in tools/content-pipeline/src/index.ts
```

## Implementation Strategy

### MVP First — User Story 1

1. Complete Setup and record all required approvals.
2. Complete Foundational tasks.
3. Implement and independently validate US1 against AC-01 through AC-04.
4. Stop for review; do not close PLAT-001 until acceptance evidence and required owner sign-off are recorded.

### Incremental Delivery

1. Deliver the reproducible local environment and readiness behavior (US1).
2. Add pull-request automation and secret scanning (US2).
3. Run all eight acceptance scenarios, attach evidence, and obtain owner sign-off before closing
   PLAT-001.

## Notes

- Every task uses the required checkbox, sequential ID, optional `[P]`, required story label for
  story-phase tasks, and explicit file path.
- Runtime/package versions and health behavior are approved and recorded; each implementation task
  still requires its artifacts and verification before being marked complete.