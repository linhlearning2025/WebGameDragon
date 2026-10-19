# Implementation Plan: QA-002 — Child Data Privacy, Consent & Data Protection

**Branch**: Not created | **Date**: 2026-10-19 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/002-child-data-privacy/spec.md` (source: QA-002)

## Summary

Make the backend the single authority for parent consent, guest-data expiry, child-data access, and
audit/redaction. Add a consent ledger (append-only), a backend-issued speech-session check that fails
closed, an idempotent guest-data retention worker, minimized pseudonymous audit events, privacy-request
tracking, a versioned data inventory enforced by tests, and log redaction. React owns the parent consent
notice and privacy-request screens; Phaser only reacts to a backend-confirmed "speech allowed" result.
Microphone speaking stays disabled by configuration (FR-026) until OD-3 is approved. Guests never receive speech sessions (FR-027); speech sessions are short leases
(proposed 15 s) renewed by heartbeat, so a withdrawal on any device stops capture within the lease. Policy values
(24 h guest lifetime and deletion, 30 d request and backup purge) are configuration, flagged proposed,
and never hard-coded as legal facts.

## Technical Context

**Language/Version**: C# on .NET SDK 10.0.402 / ASP.NET Core 10.0.13; TypeScript 7.0.2 on Node.js 24.22.0
(unchanged from PLAT-001).

**Primary Dependencies**: EF Core 10.0.13 with Npgsql 10.0.3 (migrations); ASP.NET Core hosted services
for the retention worker; built-in structured logging with a redaction policy; React 19.3.1 and Phaser
3.90.0 for UI. No new third-party runtime dependency is proposed.

**Storage**: PostgreSQL 17.11 (local or hosted). New tables: consent records, privacy requests, audit
events, retention-run records. Encryption at rest is a hosting responsibility confirmed by Security;
no application-level field encryption is introduced.

**Testing**: MSTest 4.5.1 (unit, endpoint, retention, redaction tests with fake clock and fake log
sink; PostgreSQL integration tests gated on `ConnectionStrings__Default`); Vitest 5.0.5 for consent UI
and bridge handling; Gitleaks 8.30.1 already in CI.

**Target Platform**: Browser clients on desktop/tablet/mobile; ASP.NET Core API on native hosts; no
containers.

**Project Type**: Web application (React/Phaser frontend, ASP.NET Core backend) in the existing monorepo.

**Performance Goals**: Consent evaluation adds no user-perceivable delay to starting a speaking activity;
the numeric target is deferred to DEC-16 (NFR-008) and is not invented here.

**Constraints**: Backend enforces authorization and ownership; client assertions never trusted; fail
closed when consent state is unknown; no raw audio, transcripts, tokens, PINs, or direct child
identifiers in logs or audit events; no legal-compliance claim; no Docker. Audit pseudonym key comes from
environment (`Privacy__AuditPseudonymKey`) and startup fails fast if it is missing.

**Scale/Scope**: Per-family volumes (up to 5 children per parent); one retention run per interval over
expired guest sessions; scale limits beyond that are out of scope.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle / gate | Status | Plan evidence |
|---|---|---|
| I. Child-first UX and privacy by design | Pass | No raw audio/transcripts stored; consent-gated microphone; friendly non-microphone fallback; no sensitive data in logs. |
| II. Backend as source of truth | Pass | Consent state, ownership, expiry, and speech-session grants are decided only by the API. |
| III. Domain ownership | Pass with dependency note | Privacy owns consent, audit, privacy requests; it consumes Identity ownership checks (PLAT-005) and Guest tables (PLAT-003/GUEST-001) through interfaces, not direct access. |
| IV. API versioning, idempotency, atomicity | Pass | `/api/v1` contract; consent writes and cleanup are idempotent; consent change plus audit event commit in one transaction. |
| V. Spec-before-code and testing | Pass with gate | Spec exists and is clarified; OD-1/OD-2/OD-3 remain approval gates, so no production enablement before sign-off. |
| VI. React–Phaser separation | Pass | React hosts notice/consent UI; Phaser receives only a typed bridge event with a backend result. |
| Stack and storage guardrails | Pass | React, Phaser 3, ASP.NET Core, PostgreSQL/EF Core only; no JSONB for relational facts. |
| Approval boundary | Gate retained | Privacy and parental-consent controls MUST be approved before a production pilot (constitution); recorded as release gate in quickstart. |

**Post-design recheck**: Pass. The design adds no external processor, stores no speech content, and
keeps every unresolved policy value behind configuration and a disabled-by-default feature switch.

## Project Structure

### Documentation (this feature)

```text
specs/002-child-data-privacy/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── privacy.openapi.yaml
│   └── consent-events.md
├── checklists/requirements.md
└── tasks.md                 # Produced by /speckit-tasks
```

### Source Code (repository root)

```text
apps/
├── api/
│   ├── Privacy/             # Consent, speech-session check, privacy requests, audit, redaction
│   ├── Retention/           # Guest-data retention worker and run records
│   ├── Data/                # New entities and migration (extends ApplicationDbContext)
│   └── tests/DragonEnglishKids.Api.Tests/   # Privacy, retention, redaction, authz tests
└── web/
    └── src/
        └── privacy/         # Parent consent notice, consent history, privacy request UI, bridge events
docs/
└── privacy/
    ├── data-inventory.yaml  # Versioned inventory (FR-001), checked by a test
    └── processors.md        # External processor and vendor DPA checklist (OD-3)
```

**Structure Decision**: Extend the existing `apps/api` and `apps/web` layout from PLAT-001. Privacy code
lives in focused folders so other domains consume it through interfaces. The inventory lives under
`docs/privacy` so reviewers and a test can read the same file.

## Phase 0/1 Outputs

- [research.md](research.md): decisions and rationale; how each open decision is handled without
  inventing policy.
- [data-model.md](data-model.md): entities, rules, and state transitions.
- [contracts/privacy.openapi.yaml](contracts/privacy.openapi.yaml): proposed (DRAFT) API contract.
- [contracts/consent-events.md](contracts/consent-events.md): proposed typed bridge events.
- [quickstart.md](quickstart.md): validation scenarios mapped to AS-01–AS-10.

## Dependencies and Risks

- **PLAT-001**: not fully closed (database acceptance and CI evidence pending); QA-002 implementation
  must not close before it.
- **PLAT-003 / AUTH-003**: relational tables for guest sessions and child profiles do not exist yet.
  This feature stores a child profile identifier without a foreign key until AUTH-003 lands, then adds
  the key in a migration. Retention targets the guest-session abstraction, wired when GUEST-001 exists.
- **PLAT-005**: ownership policy is consumed through `IChildAccessAuthorizer`; until it lands, tests use
  a fake and the endpoints stay disabled in non-test environments.
- **OD-1/OD-2/OD-3**: open policy decisions; proposed values are configuration only and remain gated.

## Complexity Tracking

No constitution violations. The only added moving part, a hosted retention worker, is required by the
guest-deletion and retry scenarios (AS-05, AS-10) and replaces nothing simpler.
