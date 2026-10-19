# Research: QA-002 — Child Data Privacy, Consent & Data Protection

No `NEEDS CLARIFICATION` remains in the Technical Context. Policy items OD-1, OD-2, and OD-3 are
legal/approval decisions, not design unknowns; each is handled below as configuration plus a release gate.

## Decisions

### R1 — Consent enforcement location
- **Decision**: The API decides whether speech may start. The client asks for a speech session; the API
  returns allowed or a refusal, and fails closed on any uncertainty.
- **Rationale**: Constitution II forbids trusting client assertions; AS-03 and AS-09 require fail-closed.
- **Alternatives**: Client-side check only (rejected: bypassable); cached consent flag in the client
  (rejected: stale after withdrawal).

### R2 — Consent record model
- **Decision**: Append-only consent decisions per parent, child, purpose, notice version; current state
  is the latest decision. Material notice changes invalidate older decisions (clarified rule).
- **Rationale**: Gives an audit trail, supports withdrawal history shown to parents, and makes
  re-consent a simple version comparison.
- **Alternatives**: Single mutable consent row (rejected: loses history); consent in JSONB (rejected:
  relational facts, constitution guardrail).

### R3 — Mid-activity withdrawal
- **Decision**: Speech sessions are short-lived server-side grants. Withdrawal revokes them and React
  emits a typed event so Phaser stops capture immediately; the child continues without the microphone.
  The lease (proposed 15 s) is renewed by a heartbeat that rechecks consent, so a withdrawal on another
  device stops capture within the lease. Guests never receive a session (FR-027).
- **Rationale**: Clarified requirement (FR-010). Lease length is configuration, tuned with DEC-16.
- **Alternatives**: Poll consent state (rejected: delay); end the whole activity (rejected by clarification).

### R4 — Speech disabled by default
- **Decision**: A configuration switch keeps microphone speaking off until OD-3 is approved; all children
  get the non-microphone alternative (FR-026).
- **Rationale**: Avoids exposing voice to an unapproved processor while keeping the consent flow testable.
- **Alternatives**: Allow browser-default recognition now (rejected: unreviewed vendor behavior).

### R5 — Guest retention worker
- **Decision**: A hosted background worker processes expired guest sessions in idempotent batches and
  records each run (success, failure, counts). Expired tokens are refused at request time regardless of
  worker state.
- **Rationale**: Expiry is enforced instantly; deletion can lag within the proposed window and retry
  safely (AS-04, AS-05, AS-10). No Docker or external scheduler is needed.
- **Alternatives**: Database-level scheduled jobs (rejected: extra operational dependency); delete only
  at request time (rejected: abandoned sessions never cleaned).

### R6 — Audit pseudonymization
- **Decision**: Audit events store an HMAC of the child identifier using an environment-supplied key, plus
  actor role, action, outcome, time, and correlation id. Key is required at startup.
- **Rationale**: Meets FR-022 (no unnecessary identifiers) while letting staff correlate events.
- **Alternatives**: Raw ids (rejected); no subject reference (rejected: events not traceable).

### R7 — Audit visibility
- **Decision**: Staff see all audit records; parents see only consent history for their own children.
- **Rationale**: Clarified answer; limits exposure of security events.
- **Alternatives**: Parent sees all events (rejected by clarification).

### R8 — Redaction
- **Decision**: Central redaction policy on logs and error responses, plus tests that scan captured logs
  and audit rows for raw audio, transcripts, tokens, PINs, and child identifiers. Request-body logging is off.
- **Rationale**: Satisfies FR-017/FR-022 and SC-004 with an automated gate.
- **Alternatives**: Manual review only (rejected: not repeatable).

### R9 — Data inventory enforcement
- **Decision**: `docs/privacy/data-inventory.yaml` lists every persisted field with class, purpose,
  recipients, retention, and deletion method; a test fails when a persisted field is missing.
- **Rationale**: Makes FR-001/FR-002 and SC-001 verifiable and keeps the inventory current.
- **Alternatives**: Wiki document (rejected: drifts from code).

### R10 — Encryption
- **Decision**: Require HTTPS and HSTS in non-development environments; rely on hosted PostgreSQL and disk
  encryption confirmed by Security; no application-level encryption added.
- **Rationale**: FR-016 asks for environment-approved safeguards; adding custom crypto raises risk.
- **Alternatives**: Column encryption (rejected for now; revisit if Security requires it).

### R11 — Privacy requests
- **Decision**: This feature tracks request status and verification (signed-in parent, parental gate,
  account email confirmation); AUTH-007 performs export and deletion.
- **Rationale**: Respects domain boundaries and the spec's out-of-scope list.

## Open Decisions as Release Gates

| ID | Handling in this plan |
|---|---|
| OD-1 (Vietnam-only legal basis, notice wording) | Notice text is versioned content; production enablement blocked until Legal approves; no compliance claim. |
| OD-2 (retention values, vendor-held data) | Values live in configuration marked proposed; no vendor receives child data in the first release (clarified). |
| OD-3 (speech approach/vendor) | Speaking stays disabled by configuration; processor checklist template provided. |
