# Tasks: QA-002 — Child Data Privacy, Consent & Data Protection

**Input**: Design documents from `specs/002-child-data-privacy/`

**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/](contracts/), [quickstart.md](quickstart.md)

**Tests**: Included. QA-002 requires automated evidence for consent denial, retention cleanup, redaction, and access (ticket §6).

**Organization**: Grouped by user story so each can be implemented and verified independently. No Docker or container configuration is created by any task.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependency on incomplete tasks)
- **[Story]**: User story from spec.md (US1–US4)
- Policy values (guest lifetime and deletion 24 h, request target and backup purge 30 d, speech lease 15 s) are configuration only and marked proposed (OD-1/OD-2); production enablement stays gated.
- Tasks T053–T059 were added after analysis and sit in their phases; they are not in numeric order.

## Path Conventions

API: `apps/api/` · API tests: `apps/api/tests/DragonEnglishKids.Api.Tests/` · Web: `apps/web/src/` · Docs: `docs/privacy/`

---

## Phase 1: Setup

**Purpose**: Configuration and documentation scaffolding

- [X] T001 Add `PrivacyOptions` (speech-enabled switch default off; speech lease proposed 15 s; guest lifetime, deletion delay, request target, and backup purge proposed 24 h / 24 h / 30 d / 30 d; values are development-only defaults and MUST be set explicitly outside Development; all pending approval) in apps/api/Configuration/PrivacyOptions.cs and bind it in apps/api/Program.cs
- [X] T002 Add fail-fast validation for the required `Privacy__AuditPseudonymKey` environment setting (no value disclosure) in apps/api/Configuration/PrivacyConfiguration.cs and document the name in .env.example
- [X] T003 [P] Create the versioned data inventory skeleton listing every persisted field with class, purpose, recipients, retention key, and deletion method in docs/privacy/data-inventory.yaml
- [X] T004 [P] Create the external-processor and vendor DPA checklist template (no processor approved) in docs/privacy/processors.md

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared persistence, authorization seam, audit, and redaction used by every story

**CRITICAL**: No user story work can begin until this phase is complete

- [X] T005 [P] Create entities `ConsentRecord`, `SpeechSession`, `PrivacyRequest`, `AuditEvent`, `RetentionRun` per data-model.md in apps/api/Data/Privacy/
- [X] T006 Register the entities and constraints (append-only consent and audit, unique keys for idempotency) in apps/api/Data/ApplicationDbContext.cs and add the EF Core migration in apps/api/Data/Migrations/
- [X] T007 [P] Register `TimeProvider` for testable time in apps/api/Program.cs
- [X] T008 [P] Define `IChildAccessAuthorizer` with a deny-by-default implementation (replaced when PLAT-005 lands) in apps/api/Privacy/IChildAccessAuthorizer.cs
- [X] T009 [P] Implement `SubjectPseudonymizer` (HMAC of child id using the environment key) in apps/api/Privacy/Audit/SubjectPseudonymizer.cs
- [X] T010 Implement `AuditWriter` that stores minimized events with correlation id in the same transaction as the action in apps/api/Privacy/Audit/AuditWriter.cs (depends on T005, T006, T009)
- [X] T011 [P] Implement the central log and error redaction policy and disable request-body logging in apps/api/Privacy/Redaction/RedactionPolicy.cs and apps/api/Program.cs (applied through the redacting console formatter; verified by T035)
- [X] T012 [P] Implement standard problem responses (`consent_required`, `speech_disabled`, `privacy_unavailable`, `guest_not_allowed`, forbidden) without sensitive data in apps/api/Privacy/PrivacyProblems.cs
- [X] T013 [P] Add tests for key validation, pseudonymization stability, and audit minimization in apps/api/tests/DragonEnglishKids.Api.Tests/AuditTests.cs and apps/api/tests/DragonEnglishKids.Api.Tests/StartupConfigurationTests.cs

**Checkpoint**: Foundation ready — user stories can begin

---

## Phase 3: User Story 1 - Parent Controls Microphone Consent (Priority: P1) 🎯 MVP

**Goal**: Parent grants, denies, and withdraws consent; the backend decides whether speech may start and fails closed.

**Independent Test**: Grant, deny, and withdraw consent for a test child; verify speech-session responses, revocation, audit events, and the child fallback (AS-01, AS-02, AS-03, AS-08, AS-09).

### Tests for User Story 1

> Write these first and confirm they fail before implementation.

- [X] T014 [P] [US1] Unit tests for consent evaluation (latest decision wins, per child and purpose, material notice change invalidates, unknown state is not consent) in apps/api/tests/DragonEnglishKids.Api.Tests/ConsentEvaluatorTests.cs
- [X] T015 [P] [US1] Endpoint tests for grant, deny, withdraw, idempotent replay, owner-only access with a fake authorizer, denial for non-owners, and a sensitive-read audit event for consent history (AS-01, AS-02) in apps/api/tests/DragonEnglishKids.Api.Tests/ConsentEndpointTests.cs
- [X] T016 [P] [US1] Endpoint tests for speech sessions: no consent, denied, withdrawn, speech disabled, privacy lookup failure returns 503 and fails closed, guests never receive a speech session (AS-03, AS-09, AS-11, FR-026, FR-027) in apps/api/tests/DragonEnglishKids.Api.Tests/SpeechSessionEndpointTests.cs
- [X] T017 [P] [US1] Test that withdrawal revokes active speech sessions and records audit events, and that no speech-derived data is persisted after withdrawal takes effect (AS-08) in apps/api/tests/DragonEnglishKids.Api.Tests/ConsentWithdrawalTests.cs
- [X] T018 [P] [US1] Web tests for consent notice, history view, fallback with no consent prompt to the child, and withdrawal stopping capture in apps/web/src/privacy/consent.test.tsx
- [X] T053 [P] [US1] Endpoint test for retrieving the current notice and its version in apps/api/tests/DragonEnglishKids.Api.Tests/NoticeEndpointTests.cs
- [X] T054 [P] [US1] Test that a withdrawal made on another device stops speech within the lease bound because the heartbeat is refused (FR-010) in apps/api/tests/DragonEnglishKids.Api.Tests/SpeechLeaseTests.cs
- [X] T055 [P] [US1] Accessibility test for the consent notice and controls (touch targets, reduced motion, non-audio fallback) in apps/web/src/privacy/consent.a11y.test.tsx

### Implementation for User Story 1

- [X] T019 [P] [US1] Implement versioned notice metadata with a materiality flag (wording itself pending Legal, OD-1) in apps/api/Privacy/Consent/NoticeVersions.cs
- [X] T020 [US1] Implement `ConsentEvaluator` in apps/api/Privacy/Consent/ConsentEvaluator.cs (depends on T005, T019)
- [X] T021 [US1] Implement `ConsentService` (record decision, idempotency, ownership check via `IChildAccessAuthorizer`, revoke speech sessions on withdrawal, write audit event atomically) in apps/api/Privacy/Consent/ConsentService.cs (depends on T008, T010, T020)
- [X] T022 [US1] Implement `SpeechSessionService` (checks speech switch, rejects guests, consent validity, ownership; 15 s lease renewed by heartbeat; fails closed) in apps/api/Privacy/Consent/SpeechSessionService.cs (depends on T008, T020, T021)
- [X] T023 [US1] Map the consent and speech-session endpoints per contracts/privacy.openapi.yaml in apps/api/Privacy/Consent/ConsentEndpoints.cs, applying the ownership check and a denied-access audit event on every endpoint, writing a sensitive-read audit event when a parent views consent history, and register them in apps/api/Program.cs
- [X] T024 [P] [US1] Implement the typed bridge events from contracts/consent-events.md with subscribe and cleanup in apps/web/src/privacy/consentBridge.ts
- [X] T025 [P] [US1] Implement the parent consent notice and consent history screens, showing success only after backend confirmation and behind the parental gate once AUTH-005 exists (placeholder until then), in apps/web/src/privacy/ConsentNotice.tsx and apps/web/src/privacy/ConsentHistory.tsx
- [X] T026 [US1] Implement the speech-start flow that asks the backend first and routes to the non-microphone alternative without a child consent prompt, renewing the lease by heartbeat and stopping capture when renewal is refused, in apps/web/src/privacy/speechSession.ts (depends on T024)
- [X] T056 [P] [US1] Implement placeholder notice content (what, why, by whom, how long; final wording pending Legal, OD-1) in apps/api/Privacy/Consent/NoticeContent.cs
- [X] T057 [US1] Implement notice retrieval and speech-session heartbeat endpoints per contracts/privacy.openapi.yaml in apps/api/Privacy/Consent/NoticeEndpoints.cs and apps/api/Privacy/Consent/SpeechSessionEndpoints.cs, apply the `IChildAccessAuthorizer` ownership check and a denied-access audit event to both endpoints, and register them in apps/api/Program.cs (depends on T022, T056)
- [X] T058 [P] [US1] Document how browser speech recognition handles audio, vendor dependencies, and whether audio leaves the device (pending OD-3) in docs/privacy/speech-processing.md

**Checkpoint**: US1 verifiable on its own; microphone speaking remains disabled by default (FR-026)

---

## Phase 4: User Story 2 - Guest Data Expires and Is Deleted (Priority: P1)

**Goal**: Guest tokens expire at the configured lifetime, expired data is inaccessible, and temporary data is deleted by an idempotent, retryable worker.

**Independent Test**: Expire a guest session with a fake clock, confirm refusal and a fresh empty session, run cleanup, and verify deletion and retry (AS-04, AS-05, AS-10).

### Tests for User Story 2

- [X] T027 [P] [US2] Tests for expiry refusal and immediate empty new session that never restores expired data and that clock skew does not extend the lifetime (AS-04) in apps/api/tests/DragonEnglishKids.Api.Tests/GuestExpiryTests.cs
- [X] T028 [P] [US2] Tests for deletion within the configured window and for sparing claimed guest data, using a fake guest store that models the claimed state (AS-05) in apps/api/tests/DragonEnglishKids.Api.Tests/RetentionWorkerTests.cs
- [X] T029 [P] [US2] Tests for a failed run, safe rerun without duplicates or misses, and operator-visible failure (AS-10) in apps/api/tests/DragonEnglishKids.Api.Tests/RetentionRetryTests.cs

### Implementation for User Story 2

- [X] T030 [P] [US2] Define `IExpiredGuestDataStore` as the seam to the guest tables (wired when GUEST-001 and PLAT-003 exist; in-memory fake for tests) in apps/api/Retention/IExpiredGuestDataStore.cs
- [X] T031 [P] [US2] Implement `GuestExpiryPolicy` using the configured lifetime and `TimeProvider` in apps/api/Retention/GuestExpiryPolicy.cs
- [X] T032 [US2] Implement the idempotent batch `RetentionWorker` hosted service that records `RetentionRun` results and audit outcomes in apps/api/Retention/RetentionWorker.cs (depends on T010, T030, T031)
- [X] T033 [US2] Emit sanitized failure signals (code and counts only) so operators can see failed or partial runs in apps/api/Retention/RetentionWorker.cs and register the service in apps/api/Program.cs

**Checkpoint**: US2 verifiable with the fake store; real wiring follows GUEST-001

---

## Phase 5: User Story 3 - Access Control and Redaction (Priority: P1)

**Goal**: Only owners and authorized processes can reach child data; logs and audit records contain nothing sensitive; the inventory is enforced.

**Independent Test**: Run cross-owner, unauthenticated, and guest access attempts and scan captured logs and audit rows (AS-06, AS-07).

### Tests for User Story 3

- [X] T034 [P] [US3] Tests that non-owners, unauthenticated users, and guests are denied without disclosure and produce minimized audit events (AS-06) in apps/api/tests/DragonEnglishKids.Api.Tests/ChildAccessTests.cs
- [X] T035 [P] [US3] Tests that scan captured logs, error responses, and audit rows for audio, transcripts, tokens, PINs, credentials, and child identifiers (AS-07) in apps/api/tests/DragonEnglishKids.Api.Tests/RedactionTests.cs
- [X] T036 [P] [US3] Test that every persisted field appears in docs/privacy/data-inventory.yaml (SC-001) and that no precise date of birth, address, or real-name field is persisted (FR-003) in apps/api/tests/DragonEnglishKids.Api.Tests/DataInventoryTests.cs
- [X] T037 [P] [US3] Test that no public endpoint exposes audit events and parents see only consent history for their own children in apps/api/tests/DragonEnglishKids.Api.Tests/AuditVisibilityTests.cs

### Implementation for User Story 3

- [X] T038 [US3] (verified by the access sweep in ChildAccessTests, which now includes the privacy-request routes) Verify every privacy endpoint, including privacy-request endpoints, applies `IChildAccessAuthorizer` and writes denied-access audit events (consent endpoints get this in T021–T023) in apps/api/Privacy/Consent/ConsentEndpoints.cs and apps/api/Privacy/Requests/PrivacyRequestEndpoints.cs
- [X] T039 [P] [US3] Require HTTPS redirection and HSTS outside Development in apps/api/Program.cs
- [X] T040 [P] [US3] Complete docs/privacy/data-inventory.yaml for all fields persisted by this feature and verify classification against the spec's data classification table
- [ ] T041 [P] [US3] (docs/privacy/audit-access.md drafted; open until Security agrees the tamper-evidence level and key versioning) Document the staff-only audit-access procedure and key-management expectations (including pseudonym key versioning) and the tamper-evidence level agreed with Security in docs/privacy/audit-access.md

**Checkpoint**: US3 verifiable independently via the access and redaction suites

---

## Phase 6: User Story 4 - Privacy Requests (Priority: P2)

**Goal**: A verified parent can submit access or deletion requests and track status; execution stays with AUTH-007.

**Independent Test**: Submit requests as owner (verified) and non-owner; verify acceptance only after verification, status tracking, and rejection without disclosure.

### Tests for User Story 4

- [X] T042 [P] [US4] Endpoint tests for submission requiring parental gate and account email confirmation, due date, status, and non-owner rejection using a fake parent verifier in apps/api/tests/DragonEnglishKids.Api.Tests/PrivacyRequestEndpointTests.cs
- [X] T043 [P] [US4] Web tests for request submission and status display only after backend confirmation in apps/web/src/privacy/privacyRequest.test.tsx

### Implementation for User Story 4

- [X] T044 [P] [US4] Define `IParentVerifier` (signed-in parent, parental gate, email confirmation) with a deny-by-default implementation until AUTH-001 and AUTH-005 land in apps/api/Privacy/Requests/IParentVerifier.cs
- [X] T045 [US4] Implement `PrivacyRequestService` (idempotent create, status transitions, due date from options, audit events) in apps/api/Privacy/Requests/PrivacyRequestService.cs (depends on T010, T044)
- [X] T046 [US4] Map the privacy-request endpoints per contracts/privacy.openapi.yaml in apps/api/Privacy/Requests/PrivacyRequestEndpoints.cs and register them in apps/api/Program.cs
- [X] T047 [P] [US4] Implement the request submission and status screen in apps/web/src/privacy/PrivacyRequests.tsx

**Checkpoint**: US4 verifiable; export and deletion execution remain with AUTH-007

---

## Phase 7: Polish & Cross-Cutting Concerns

- [X] T048 [P] Add a contract test that validates the implemented endpoints against specs/002-child-data-privacy/contracts/privacy.openapi.yaml in apps/api/tests/DragonEnglishKids.Api.Tests/PrivacyContractTests.cs
- [X] T049 [P] Run the full API and web suites plus lint and Gitleaks, confirming no Docker artifacts and no raw audio or transcript persistence
- [ ] T059 [P] Record Security's confirmation of encryption in transit and at rest for the target environment (FR-016) in docs/privacy/encryption-evidence.md
- [X] T050 (evidence recorded in quickstart.md; AS-04, and the deletion part of AS-05 and AS-10, use a fake guest store until GUEST-001) Run the quickstart scenarios AS-01–AS-10 per specs/002-child-data-privacy/quickstart.md and record evidence in that file
- [ ] T051 Record the open release gates in specs/002-child-data-privacy/quickstart.md: Legal approval of notice wording and Vietnam-only market (OD-1), approval of retention values (OD-2), and approval of a speech approach before enabling speaking (OD-3), and record that backup-purge verification is tracked under QA-005 (FR-020)
- [ ] T052 (README.md, docs/eng/dev-setup.md, and the setup scripts are updated; open until the Product Owner confirms the ticket status and sign-off) Update README.md and docs/eng/dev-setup.md with the new environment setting and privacy docs; confirm the source ticket status and sign-off before closure

---

## Dependencies & Execution Order

- **Setup (T001–T004)** → **Foundational (T005–T013)** → user stories → **Polish (T048–T052 and T059)**.
- **External**: PLAT-001 is not fully closed; US2 real wiring needs GUEST-001/PLAT-003; US3 real ownership needs PLAT-005; US4 verification needs AUTH-001/AUTH-005. Placeholder interfaces (T008, T030, T044) keep this feature testable meanwhile.
- **Stories**: US1, US2, and US3 are independent after Phase 2; US3 verifies the endpoint ownership enforcement built in T021–T023 (T038). US4 depends only on Phase 2 and T044.
- Backup purge (FR-020) is verified under QA-005; this feature only supplies the configured value.
- Within a story: tests first, then models, services, endpoints, then web.

### Parallel Examples

- **Phase 1–2**: T003 ∥ T004; T005 ∥ T007 ∥ T008 ∥ T009 ∥ T011 ∥ T012.
- **US1**: T014–T018 and T053–T055 together; T019 ∥ T024 ∥ T025 ∥ T056 ∥ T058.
- **US2**: T027–T029 together; T030 ∥ T031.
- **US3**: T034–T037 together; T039 ∥ T040 ∥ T041.
- **US4**: T042 ∥ T043; T044 ∥ T047.

## Implementation Strategy

- **MVP**: Phase 1, Phase 2, and US1 (T001–T026 plus T053–T058). It delivers consent control and a fail-closed speech check, with microphone speaking disabled by default.
- **Next increments**: US2 and US3 (both P1), then US4 (P2).
- **Release**: Not production-ready until OD-1, OD-2, OD-3 are approved and PLAT-001 and the guest, identity, and access-control tickets are integrated.
