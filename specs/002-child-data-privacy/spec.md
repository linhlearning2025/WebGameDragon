# Feature Specification: Child Data Privacy, Consent & Data Protection

**Feature ID**: QA-002 · **Project**: Dragon English Kids · **Feature Directory**: `specs/002-child-data-privacy`

**Created**: 2026-10-19

**Status**: Draft — requires Product, Privacy, Security, and Legal approval before implementation or closure

**Input**: [QA-002 ticket](../../Dragon_English_Kids_SDD_Backlog/tickets/quality/QA-002-chinh-sach-du-lieu-tre-em-consent-va-bao-ve-thong-tin.md) (P0, M0, hard dependency PLAT-001), constitution v1.1.0, DOMAIN_MAP, OPEN_DECISIONS (DEC-06, DEC-08, DEC-15).

## Clarifications

- Retention periods, consent requirements, jurisdictions, and legal obligations are NOT invented here. Every unresolved value is marked `[NEEDS CLARIFICATION]` or listed under Open Decisions, and this spec makes no COPPA, GDPR-K, or other compliance claim.
- Technology (React/Phaser/ASP.NET Core/PostgreSQL/R2, native environment, no Docker) is fixed by the constitution and PLAT-001; this spec states outcomes, not designs.

### Session 2026-10-19

- Q: Should every speaking activity require recorded parent consent, even if the browser handles speech recognition and no audio reaches our servers? → A: Yes. Always require parent consent before any microphone use, regardless of where speech is processed.
- Q: When consent is withdrawn or a guest session expires mid-activity, should the activity end immediately or let the child finish the current question? → A: Microphone capture and processing stop immediately; the child sees a friendly message and may finish the current question without the microphone.
- Q: When microphone consent is absent or denied, should the child see the speaking activity, or be given a non-microphone alternative automatically? → A: The child is automatically given the non-microphone alternative, with no consent prompt shown to the child.
- Q: Which events must be audit-logged: only consent and privacy actions, or also parent views of child data? → A: Consent and privacy actions plus successful access to sensitive child data (such as data exports and consent records), but not routine reads.
- Q: Who may see audit records of consent and child-data access? → A: Designated security/privacy staff see all audit records; a parent sees only the consent history for their own children.
- Q: How long should a guest trial session stay valid before it expires? → A: 24 hours from creation (proposed; requires Product Owner and Security approval under DEC-08).
- Q: How soon after a guest session expires must its temporary placement-test data be permanently deleted? → A: Within 24 hours after expiry (proposed; requires Product Owner, Security, and Legal approval).
- Q: After a guest session expires, can the child start a new guest session immediately? → A: Yes; a new guest session can start immediately, is empty, and never restores expired data. Abuse limits stay with GUEST-001 rate limiting.
- Q: What completion time should the product promise for a parent's data-access or deletion request? → A: Within 30 days (proposed; Legal must confirm per market under OD-1).
- Q: In which countries will the first public release be offered? → A: Vietnam only (proposed; Legal must confirm, and no legal compliance is claimed).
- Q: How long may backup copies of deleted data remain before permanent purge? → A: Up to 30 days (proposed; Legal and Security must approve, and the notice must not promise instant removal from backups).
- Q: If the consent notice wording changes after a parent has consented, must the parent consent again? → A: Only for material changes (what is collected, who receives it, or how long it is kept); Legal must confirm under OD-1.
- Q: Besides the microphone, will the first release send any child information to an external AI or content service? → A: No; no child information is sent to any external AI or content service in the first release.
- Decision (analysis remediation): guests never get microphone use; speech sessions are short leases (proposed 15 seconds) renewed by heartbeat so withdrawal on any device stops capture within the lease.
- Q: How should the product confirm a data-deletion request really comes from the parent? → A: The parent must be signed in, pass the parental gate, and confirm through the account email; detailed flow stays with AUTH-007.
- Q: How should speech recognition be handled while no speech approach or vendor is approved? → A: Microphone-based speaking stays disabled for all children, who get the non-microphone alternative, until an approach is approved.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Parent Controls Microphone and External Processing Consent (Priority: P1)

A parent sees a plain-language explanation of what microphone-related and external-service processing involves, then grants or denies it per child. Denial never blocks core learning; a non-microphone alternative is offered. The parent can withdraw consent at any time.

**Why this priority**: Child voice is the most sensitive input; no processing may happen without parent control.

**Independent Test**: Grant, deny, and withdraw consent for a test child and verify speaking behavior and recorded consent state each time.

**Acceptance Scenarios**:

1. **AS-01 — Parent grants required consent**: **Given** a signed-in parent who has viewed the notice, **When** they grant consent for a specific child and purpose, **Then** the consent is recorded with notice version and time, and speaking becomes available for that child only for that purpose.
2. **AS-02 — Parent denies consent**: **Given** the notice is shown, **When** the parent denies consent, **Then** the denial is recorded, no capture or external processing occurs, and the child is automatically given a non-microphone alternative with no penalty and no consent prompt.
3. **AS-03 — Speaking starts without required consent**: **Given** no valid consent exists (absent, denied, or withdrawn), **When** a speaking activity is started by any means, **Then** the backend-enforced check blocks capture and external processing, no speech data leaves the device, and the child is automatically given the alternative without any consent prompt.
4. **AS-08 — Consent is withdrawn**: **Given** consent was granted, **When** the parent withdraws it (including mid-activity), **Then** active capture and further processing stop immediately, the child sees a friendly message and may finish the current question without the microphone, subsequent speaking attempts follow AS-03, and the withdrawal is recorded and auditable.
5. **AS-11 — Guest asks for speech**: **Given** a guest session, **When** speaking is requested, **Then** no speech session is granted and the child is automatically given the non-microphone alternative.

---

### User Story 2 - Guest and Temporary Data Is Minimal and Expires (Priority: P1)

A guest child can try the product with only the minimum data needed. When the guest session expires, temporary placement-test data is deleted on the proposed schedule (pending approval) and expired credentials stop working.

**Why this priority**: Guests have no parent relationship yet, so the safest default is least data and automatic removal.

**Independent Test**: Create a guest session with temporary results, expire it, run cleanup, and verify inaccessibility and deletion.

**Acceptance Scenarios**:

1. **AS-04 — Guest session expires**: **Given** a guest session past its 24-hour lifetime, **When** its token is presented, **Then** access is refused with a clear re-start message, the child can start a new empty guest session immediately, and the token cannot read or write any data.
2. **AS-05 — Temporary guest data is deleted**: **Given** an expired guest session with temporary placement-test data, **When** retention cleanup runs, **Then** that data is deleted within 24 hours after expiry, and a claimed guest's data is handled per the claim rules rather than deleted as temporary.
3. **AS-10 — Retention cleanup fails and retries**: **Given** a cleanup run fails partway, **When** the next scheduled attempt runs, **Then** it resumes or repeats safely without duplicating or skipping deletions, expired data stays inaccessible meanwhile, and the failure is visible to operators.

---

### User Story 3 - Child Data Is Access-Controlled and Redacted (Priority: P1)

Only authorized actors can read or change a child's data, and audit/diagnostic records record what happened without exposing unnecessary child information.

**Why this priority**: Consent and minimization fail if data is readable by the wrong person or leaks into logs.

**Independent Test**: Attempt cross-parent and unauthenticated access; inspect logs from consent, access, and error paths.

**Acceptance Scenarios**:

1. **AS-06 — Unauthorized user accesses child data**: **Given** a user who does not own the child profile (or is unauthenticated, or a guest), **When** they request or change that child's data or consent, **Then** the backend denies the request, discloses nothing about the child, and records a minimized security event.
2. **AS-07 — Sensitive information appears in logs**: **Given** representative flows (consent, speaking, guest expiry, errors), **When** logs and audit records are inspected or scanned, **Then** no raw audio, transcript, token, PIN, credential, or unnecessary direct child identifier appears; any violation fails verification.
3. **AS-09 — Privacy service becomes unavailable**: **Given** the consent/privacy capability cannot be reached or evaluated, **When** a consent-dependent action is attempted, **Then** the system fails closed (no capture, no external processing), shows a friendly retry/alternative message, and never treats consent as granted by default.

---

### User Story 4 - Parent Makes Privacy Requests (Priority: P2)

A verified parent can request access to or deletion of their child's data and see request status; retention behavior is explained.

**Why this priority**: Required for trust and likely for legal readiness, but detailed export/delete mechanics belong to AUTH-007.

**Independent Test**: Submit access and deletion requests as owner and non-owner; verify authorization, status, and recorded outcome.

**Acceptance Scenarios**:

1. **Given** an authenticated owning parent, **When** they submit an access or deletion request, **Then** it is accepted only after they have passed the parental gate and confirmed through the account email, tracked with a status, and handled within the proposed 30-day target.
2. **Given** a non-owner submits the same request, **When** it is evaluated, **Then** it is rejected without changing or revealing any data.

### Edge Cases

- Consent granted for one child does not apply to siblings or to a different purpose.
- Browser-only speech recognition still requires parent consent; the browser's own microphone permission alone is never sufficient.
- Consent notice version changes: prior consent remains valid for non-material wording changes; a material change (what is collected, who receives it, or how long it is kept) requires new parent consent before microphone use continues.
- Browser speech recognition may send audio to a browser/OS vendor outside this product's control; the notice must disclose this and the product must not claim otherwise.
- Guest session claimed by a parent while cleanup is running: claim wins; claimed data is not deleted as temporary.
- Withdrawal arrives while a result is being saved: no speech-derived data is persisted after withdrawal takes effect.
- Client reports "consent granted" without backend record: ignored; backend state is authoritative.
- Deletion requested for data held in backups or an external vendor: handled per the approved retention/vendor process; no promise of instant removal.
- Clock skew or timezone differences must not extend a guest lifetime beyond its approved expiry.
- Unsupported or unapproved jurisdiction: affected collection/processing stays disabled and no compliance is claimed.

## Requirements *(mandatory)*

### Functional Requirements

**Data Classification (FR-01)**
- **FR-001**: The product MUST maintain a data inventory classifying every collected field as personal, sensitive, temporary, or non-sensitive (see Data Classification), with its purpose, collection point, recipients, retention, and deletion method.
- **FR-002**: Introducing a new collected field MUST require an inventory entry and recorded purpose before release.

**Data Minimization (FR-02)**
- **FR-003**: Child profiles MUST use a nickname rather than a real name, and MUST NOT collect precise date of birth, address, or contact details for children unless an approved need is documented.
- **FR-004**: Every collected field MUST have a documented purpose; fields without one MUST NOT be collected.

**Parental Consent (FR-03)**
- **FR-005**: Any microphone use, including browser-only speech recognition with no audio sent to the product's servers, MUST require recorded parent consent for that child. Before such use, a parent MUST be shown a plain-language notice stating what is processed, why, by whom (including browser or external vendors), and for how long.
- **FR-006**: Consent MUST be recorded per parent, child, purpose, notice version, decision (granted/denied/withdrawn), and timestamp, and MUST be changeable by the parent at any time. A material notice change (what is collected, who receives it, or how long it is kept) MUST require new consent; non-material wording changes MUST NOT.
- **FR-007**: External AI or speech service processing of child data MUST require a reviewed vendor, a disclosed purpose, and parent consent. Microphone use is always consent-gated (see FR-005 and FR-026). In the first release, no child information MAY be sent to any external AI or content service; introducing one requires a new approved specification.
- **FR-008**: Denying or withdrawing consent MUST NOT block core learning; a non-microphone alternative MUST be provided automatically, without prompting the child for consent, where the activity supports one, and fallback MUST NOT record pronunciation mastery as correct.

**Microphone Privacy (FR-04)**
- **FR-009**: The product MUST NOT record, persist, or upload raw child audio by default; any exception requires separate approved specification.
- **FR-010**: Capture MUST start only after a user gesture and valid consent, MUST be visibly indicated, and MUST stop when the activity ends, the scene is left, or consent is withdrawn. On withdrawal, capture and processing MUST stop immediately and the child MUST be shown a friendly, non-shaming message and allowed to finish the current question without the microphone. If the withdrawal is made on another device, capture MUST stop within the speech-session lease (proposed 15 seconds, pending Security approval).
- **FR-011**: The documentation MUST describe how browser speech recognition handles audio and list applicable vendor dependencies, including whether audio leaves the device `[NEEDS CLARIFICATION: see OD-3 — approved speech approach and vendor; DEC-06]`.
- **FR-012**: Child speech transcripts MUST NOT be stored by default.
- **FR-026**: Until Privacy, Security, and Legal approve a speech approach and any vendor (OD-3), microphone-based speaking MUST remain disabled for all children, who MUST be given the non-microphone alternative; approval of an approach does not remove the consent requirement in FR-005.
- **FR-027**: Guest sessions MUST NOT be granted microphone use; guests receive the non-microphone alternative because no parent consent can be recorded for them.

**Guest Session Retention (FR-05)**
- **FR-013**: Guest tokens MUST expire 24 hours after creation (proposed value, pending Product Owner and Security approval under DEC-08); expired tokens MUST be refused. A child MAY start a new guest session immediately after expiry; the new session MUST be empty and MUST NOT restore any expired data. Abuse limits are owned by GUEST-001.
- **FR-014**: Temporary placement-test data MUST be deleted within 24 hours after expiry (proposed value, pending Product Owner, Security, and Legal approval), and cleanup failures MUST be retryable, idempotent, and observable.

**Data Protection (FR-06)**
- **FR-015**: All child-data and consent operations MUST require authentication appropriate to the actor and be authorized by the backend against ownership; client assertions MUST NOT be trusted.
- **FR-016**: Child data MUST be protected in transit and at rest to the standard defined by the deployment environment and approved by Security.
- **FR-017**: Secrets, tokens, PINs, and child-sensitive values MUST be redacted from logs, errors, and telemetry.
- **FR-018**: Child-profile access MUST be restricted to the owning parent and authorized system processes; no child may access another child's data.

**Privacy Requests (FR-07)**
- **FR-019**: Verified owning parents MUST be able to request access to and deletion of their child's data and see request status; a request MUST be accepted only after the parent is signed in, passes the parental gate, and confirms through the account email; the target completion time is 30 days (proposed value, pending Legal confirmation per market under OD-1) and retention follows FR-020. Detailed export/deletion mechanics are owned by AUTH-007.
- **FR-020**: Retention rules MUST be documented per data category and applied consistently, including backups and vendor-held data. Backup copies of deleted data MUST be purged within 30 days (proposed value, pending Legal and Security approval), and parent-facing notices MUST NOT promise immediate removal from backups.

**Audit Logging (FR-08)**
- **FR-021**: The product MUST record consent decisions, withdrawals, privacy requests, denied access, successful access to sensitive child data (such as data exports and consent records), and retention-cleanup outcomes as audit events containing actor, action, time, and outcome.
- **FR-022**: Audit events MUST use minimal, pseudonymous child references and MUST NOT contain raw audio, transcripts, credentials, or unnecessary child identifiers.
- **FR-023**: Audit records MUST be access-restricted to designated security/privacy staff and tamper-evident (append-only storage is the minimum; any stronger level is defined with Security); a parent MUST be able to view only the consent history for their own children.

**Resilience and Failure**
- **FR-024**: If consent state cannot be determined, consent-dependent processing MUST fail closed.
- **FR-025**: The UI MUST NOT show consent or deletion as successful before the backend confirms it.

### Non-Functional Requirements

- **NFR-001 (Privacy by default)**: Default configuration collects the least data and processes nothing externally.
- **NFR-002 (Child-friendly UX)**: Child-facing messages are encouraging and non-shaming; parent-facing notices use plain language and are shown behind the parental gate where applicable.
- **NFR-003 (Accessibility)**: Notices and consent controls work on desktop, tablet, and mobile with accessible touch targets and non-audio fallbacks.
- **NFR-004 (Reliability)**: Cleanup and consent evaluation are idempotent and safe under retries and concurrent requests.
- **NFR-005 (Observability)**: Failures surface with correlation identifiers and no sensitive content.
- **NFR-006 (Compatibility)**: Changes to schemas and contracts are versioned and backward-compatible within a release.
- **NFR-007 (Environment)**: Works in the approved native development and CI environment; no container configuration is introduced.
- **NFR-008 (Performance of privacy checks)**: Consent evaluation adds no user-perceivable delay to starting a speaking activity; the numeric threshold is set with DEC-16.

### Data Classification

| Class | Meaning | Examples (to be confirmed against the inventory) | Handling |
|-------|---------|--------------------------------------------------|----------|
| Personal | Identifies or can be linked to a person | Parent email, child nickname, child profile identifier, age band | Minimize; access-controlled; deletable on request |
| Sensitive | Higher harm if exposed | Child voice/audio, speech transcripts, credentials, tokens, PIN hashes, consent records | Not stored by default (voice); redacted from logs; encrypted; restricted access |
| Temporary | Short-lived by design | Guest token, placement-test results, in-flight speech state | Expires and is deleted per approved schedule |
| Non-sensitive | Not linked to an individual | Published vocabulary, aggregate non-identifying statistics | Standard handling |

### Key Entities

- **Data Inventory Entry**: Field, class, purpose, collection point, recipients, retention, deletion method.
- **Consent Record**: Parent, child, purpose, notice version, decision, timestamps, withdrawal.
- **Guest Session Data**: Token, expiry, temporary placement results, claim state.
- **Privacy Request**: Requester, child, type (access/deletion), verification, status, outcome.
- **External Processor**: Vendor, purpose, data shared, review/approval status.
- **Audit Event**: Actor, action, pseudonymous subject, outcome, time, correlation identifier.
- **Speech Session**: A short backend-issued lease allowing speech for one consented child, renewed by heartbeat and revoked on withdrawal.
- **Retention Run**: A record of one cleanup attempt with outcome and counts, used to retry and to make failures visible.

## Privacy and Security Requirements Summary

- Backend is authoritative for authentication, ownership, consent state, and expiry (constitution II).
- No raw audio or transcripts by default; no sensitive data in logs (constitution I).
- Fail closed when privacy state is unavailable; never default to consent.
- No legal-compliance claims without documented legal review.
- Domain boundaries: Identity owns profiles and the consent subject; Guest owns guest tokens and placement data; Parent Dashboard owns consent/settings projections; Learning owns attempts; Content and Phaser own no privacy state.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of collected child-related fields in a release appear in the inventory with class, purpose, retention, and deletion method.
- **SC-002**: 0 microphone-capture or external-processing attempts succeed without valid consent across the consent test suite, including denied, withdrawn, and privacy-unavailable cases.
- **SC-003**: 100% of expired guest test sessions are inaccessible immediately at expiry and deleted within the proposed 24-hour schedule.
- **SC-004**: 0 raw audio, transcripts, tokens, PINs, credentials, or unnecessary child identifiers found in sampled logs and audit records.
- **SC-005**: 100% of cross-owner and unauthenticated access attempts in tests are denied without data disclosure.
- **SC-006**: Failed retention cleanups succeed on retry with no duplicate or missed deletions in 100% of test runs.
- **SC-007**: At least 90% of parents in usability testing correctly state what is collected, why, and whether microphone or external processing occurs.
- **SC-008**: 100% of in-scope markets and external processors have documented approval before the affected processing is enabled.
- **SC-009**: After a parent withdraws consent on any device, 100% of active speech sessions stop within the speech-session lease (proposed 15 seconds).

## Out of Scope

- Choosing or asserting legal compliance with COPPA, GDPR-K, or any regulation.
- Detailed account/profile deletion and export workflows (AUTH-007).
- Guest token issuance and claim mechanics (GUEST-001, GUEST-006); only expiry and deletion policy is defined here.
- Speech capture implementation, fuzzy matching, and fallback gameplay (GAME-008, GAME-009).
- Telemetry provider selection and backup/restore operations (PLAT-004, QA-005).
- Storing raw audio or transcripts for any purpose.
- Docker or container configuration.

## Open Decisions

| ID | Decision | Related | Owner (indicative) | Status |
|----|----------|---------|--------------------|--------|
| OD-1 | Legal basis and notice wording for the Vietnam-only first release (market proposed, pending Legal); Legal confirmation of the material-change re-consent rule | DEC-15 | Legal + Security | `[NEEDS CLARIFICATION]` — unresolved |
| OD-2 | Approval of retention, timeline, and lease values (proposed: 15-second speech lease, guest token lifetime and temporary-data deletion 24 hours each, privacy-request completion 30 days, backup purge 30 days; all pending PO/Security/Legal approval) | DEC-08, DEC-15 | PO + Security + Legal | `[NEEDS CLARIFICATION]` — unresolved |
| OD-3 | Speech approach and vendor, whether audio leaves the device, vendor DPA/terms (microphone speaking stays disabled until approved, FR-026) | DEC-06, DEC-12, DEC-15 | Privacy + Learning + FE | `[NEEDS CLARIFICATION]` — unresolved; safe default applied |

## Assumptions

- Children are ages 5–8; a parent or guardian makes privacy decisions requiring adult authority.
- The first release targets Vietnam only (proposed, pending Legal confirmation); other markets stay disabled until reviewed under OD-1.
- Nickname and age band satisfy product needs without legal names or exact birth dates.
- Raw audio and transcripts are not stored by default; exceptions need a new approved spec.
- PLAT-001 (hard dependency) is not yet fully closed; this spec can be clarified now, but implementation and closure are gated by it and by the approvals above.
- Existing platform conventions apply: required configuration fails fast, no secrets in logs, UI never shows unconfirmed writes.

## Traceability to QA-002

| QA-002 source | Covered by |
|---------------|------------|
| §2 Data minimization, PII classification | FR-001–FR-004, Data Classification, SC-001 |
| §2 Guest token TTL/deletion | FR-013, FR-014, AS-04, AS-05, AS-10, SC-003, SC-006 |
| §2 Consent for mic and AI vendor; no audio upload by default | FR-005–FR-012, AS-01–AS-03, AS-08, SC-002 |
| §2 Encryption, retention/privacy requests, access logging | FR-015–FR-023, AS-06, AS-07, User Story 4, SC-004, SC-005 |
| AC-01 (no mic without consent) | AS-03, AS-02 |
| AC-02 (guest expiry cleanup) | AS-04, AS-05 |
| AC-03 (log redaction) | AS-07, FR-017, FR-022 |
| §4 Privacy inventory, consent schema, retention, vendor DPA | FR-001, FR-006, FR-020, OD-3, Key Entities |
| §5 No unreviewed COPPA/GDPR-K claim | Clarifications, FR-007, Out of Scope, OD-1 |
| §9 Open decisions | Open Decisions OD-1–OD-3, DEC-06/08/15 |
| Additional scenarios beyond ticket | AS-09 (privacy unavailable), AS-10 (cleanup retry) |
