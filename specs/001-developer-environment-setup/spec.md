# Feature Specification: PLAT-001 — Developer Environment Setup

**Feature Branch**: Not assigned

**Created**: 2026-10-19

**Status**: WHAT/scope and technical baseline approved by the technical owner on 2026-10-19; implementation pending

**Input**: PLAT-001, "Thiết lập monorepo và môi trường phát triển"

## Clarifications

### Session 2026-10-19

- Q: Should PLAT-001 organize the web app, API, and content-authoring tools in one repository or separate repositories? → A: One repository for the web app, API, and content-authoring tools.
- Q: Should the PLAT-001 ticket be treated as the complete scope, or must PRD §1 be reviewed before the spec is approved? → A: Treat the ticket as the complete PLAT-001 scope.

## Business Goals

- Let developers reproduce a working local development environment from documented steps.
- Reduce setup failures caused by missing configuration or unavailable dependencies.
- Ensure changes are checked consistently and development secrets are not exposed.

## User Scenarios & Testing

### User Story 1 - Start the Development Environment (Priority: P1)

As a developer, I need to prepare and start the project locally so I can work on the web
application, API, and content-authoring tools using a repeatable process.

**Why this priority**: A reproducible local environment is the foundation for all subsequent
project work.

**Independent Test**: A developer with the documented prerequisites can follow the setup guide
and confirm that the development services start and report their readiness accurately.

**Acceptance Scenarios**:

1. **AC-01 — Clean setup**: **Given** a clean project checkout and the prerequisites named by the
  approved setup guide, **When** a developer follows that guide, **Then** the web application and
  API start with development configuration, their readiness can be checked, and the guide explains
  how the content-authoring tools are used in the local workflow.
2. **AC-02 — Native database configuration**: **Given** PostgreSQL 17 is installed locally or
  available from an authorized hosted database, **When** the developer configures the documented
  environment variables and starts the environment, **Then** the API connects to that database
  without requiring production media credentials.
3. **AC-03 — Missing configuration**: **Given** a required configuration value is missing,
   **When** the API starts, **Then** it fails fast and identifies the missing setting by name
   without displaying its value.
4. **AC-04 — Database unavailable**: **Given** the database is unavailable, **When** a developer
   checks API readiness, **Then** readiness reports failure rather than reporting the API as ready.
5. **AC-08 — Liveness independent of database**: **Given** the API process is alive, **When** a
  developer requests `GET /health/live`, **Then** the API returns HTTP 200 regardless of database
  availability.

### User Story 2 - Verify a Change (Priority: P2)

As a developer or reviewer, I need each proposed change to receive consistent automated checks so
that build, quality, and secret-related failures are visible before merge.

**Why this priority**: Consistent checks reduce regressions and help prevent accidental exposure
of development credentials.

**Independent Test**: Submit a change that passes and one that fails a required check; verify that
the results clearly distinguish success from failure.

**Acceptance Scenarios**:

1. **AC-05 — Pull request checks**: **Given** a pull request, **When** the required automated
   checks run, **Then** build, lint, and automated test results are reported and any failed check
   prevents the check set from reporting success.
2. **AC-06 — Secret detection**: **Given** a proposed change containing a detected secret,
   **When** secret scanning runs, **Then** the check reports failure without exposing the secret
   value.
3. **AC-07 — Unconfirmed persistence**: **Given** a user action that writes data, **When** the
   backend has not confirmed persistence, **Then** the user interface does not report the write as
   successful.

### Edge Cases

- A required database configuration value is absent: startup fails fast and names the setting without
  revealing its value.
- The database cannot be reached: API readiness reports failure.
- A build, lint, test, or secret scan fails: the corresponding check reports failure rather than a
  successful result.
- A write request has not been confirmed by the backend: the interface does not display success.
- PostgreSQL is installed locally or hosted elsewhere: the same environment-based connection
  configuration supports either target.
- PostgreSQL credentials are unavailable: startup fails safely without logging or displaying the
  supplied value.

## Requirements

### Functional Requirements

- **FR-001**: The development setup MUST provide a documented way to prepare and start the web
  application, API, and content-authoring tools from a clean project checkout. (AC-01)
- **FR-002**: Local development MUST connect to PostgreSQL 17 installed locally or hosted elsewhere
  through environment-based configuration, and MUST support mock or local media without requiring
  production media credentials. (AC-02)
- **FR-003**: The setup guide MUST identify prerequisites, required configuration names, setup
  steps, and how to verify that the development services are ready. (AC-01, AC-03, AC-04)
- **FR-004**: The API MUST fail fast when required configuration is missing and identify the
  missing setting by name without logging or displaying its value. (AC-03)
- **FR-005**: API readiness MUST report failure when the required database is unavailable.
  (AC-04)
- **FR-006**: Pull request checks MUST report build, lint, and automated test results; a failed
  required check MUST NOT be reported as success. (AC-05)
- **FR-007**: The project MUST prevent committed secrets and MUST include secret scanning in the
  change-verification process. Detected secret values MUST NOT be exposed in check output. (AC-06)
- **FR-008**: The user interface MUST NOT report a write as successful before the backend confirms
  persistence. (AC-07)
- **FR-009**: The setup guide MUST document database migration and application startup steps for a
  local or hosted PostgreSQL connection supplied through environment variables. (AC-01, AC-02)
- **FR-010**: The API MUST return HTTP 200 from `GET /health/live` while its process is alive,
  independent of PostgreSQL availability. (AC-08)

### Key Entities

- **Development Setup Guide**: The prerequisites, configuration names, setup steps, and readiness
  checks needed to reproduce the local environment.
- **Local Development Configuration**: Non-production settings that connect local services and
  select mock or local media storage without embedding secrets in the repository.
- **Change Check Result**: The visible success or failure result of required build, lint, test, and
  secret-scanning checks for a proposed change.

## Success Criteria

### Measurable Outcomes

- **SC-001**: All eight acceptance scenarios pass when evaluated against the approved setup and
  verification process.
- **SC-002**: A developer starting from a clean checkout can complete setup using the guide without
  undocumented manual steps.
- **SC-003**: Each required change check produces an unambiguous pass or fail result, and no failed
  required check is presented as a successful change.

## Error Cases

- Missing required configuration: stop startup, identify the missing setting by name, and do not
  reveal its value.
- Unavailable database: report API readiness failure.
- Failed automated check or detected secret: report the failure and do not mark the check set as
  successful.
- Backend has not confirmed a write: do not show a success state for that write.

## Out of Scope

- Implementing product features for identity, guest sessions, learning, progression, rewards,
  parent reporting, or content authoring.
- Production deployment, production credentials, and production media delivery.

## Assumptions

- The intended users are project developers preparing a local environment and reviewers checking
  proposed changes.
- The approved project constitution remains the source for the selected technology stack and
  architecture constraints; this spec does not propose alternatives.
- The PLAT-001 ticket is the complete scope source for this feature; no additional PRD requirements
  are needed for PLAT-001.
- The technical owner approved the exact baseline and readiness behavior on 2026-10-19; pins are
  recorded in `docs/eng/toolchain-matrix.md` and the versioned OpenAPI contract.
- Local development may use a database and mock or local media storage, as stated by PLAT-001.