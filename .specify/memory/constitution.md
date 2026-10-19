<!--
Sync Impact Report
- Version change: 1.0.0 -> 1.1.0
- Modified principles:
  - I. Child-First Learning -> I. Child-First UX and Privacy by Design
  - II. Single Source of Truth for State and Rewards -> II. Backend as the Source of Truth
  - III. Domain Ownership and Data Minimalism -> III. Domain Ownership
  - IV. Spec-First Delivery -> V. Spec-Before-Code and Automated Testing
  - V. Quality, Safety, and Observability -> IV. API Versioning, Idempotency, and Atomicity
- Added principle: VI. React-Phaser Separation
- Added sections: Technology and Architecture Constraints; Approval Boundary and Quality Gates
- Removed sections: none
- Deferred items: approval of DEC-01 through DEC-17; backlog tickets and proposed contracts remain DRAFT
-->

# Dragon English Kids Constitution

## Core Principles

### I. Child-First UX and Privacy by Design
The product MUST provide safe, encouraging learning for children ages 5–8. It MUST NOT use
shame-based feedback, pressure tactics, or paywalls for core learning content. Collection and
retention of child data MUST be limited to what the product needs; sensitive data handling MUST
have parent consent and defined safeguards. Raw audio and child transcripts MUST NOT be stored by
default, and sensitive child data, tokens, PINs, and voice data MUST NOT appear in logs.

### II. Backend as the Source of Truth
ASP.NET Core MUST be authoritative for authentication, child ownership, evaluated learning
outcomes, progression, unlocks, SRS state, scores, rewards, inventory, EXP, and supporter status.
Clients MUST NOT be trusted to assert correctness, mastery, unlocks, rewards, or payment status.
The backend MUST validate authorization and domain invariants before persisting outcomes.

### III. Domain Ownership
Each bounded context MUST own its data, rules, and contracts. Identity owns parent and child
profiles; Guest owns trial sessions; Content owns vocabulary and media; World Map and Progress own
stage configuration and unlock state; Learning and SRS own attempts and review state; Rewards and
Dragon own food, EXP, evolution, mood, and cosmetics; Parent Dashboard owns reporting projections
and parent settings. Cross-domain interaction MUST use reviewed APIs or events, not direct access
to another domain's private state.

### IV. API Versioning, Idempotency, and Atomicity
APIs, schemas, and integration events MUST be versioned and backward-compatible within a release.
Breaking changes MUST include an approved migration or compatibility plan. Claim, stage completion,
feeding, and donation-confirmation operations MUST be idempotent under retries and duplicate
submissions; related state changes MUST be atomic. A stage session MUST receive its completion
reward at most once per `stageSessionId`.

### V. Spec-Before-Code and Automated Testing
Every feature, gameplay change, or contract change MUST have an approved specification before
implementation. It MUST state the user need, acceptance criteria, scenarios, contracts, dependencies,
and unresolved decisions. Ambiguous business rules MUST be resolved before contracts or behavior are
frozen. Changes MUST pass relevant automated tests and CI checks, with acceptance evidence recorded
before the related ticket is closed.

### VI. React-Phaser Separation
React MUST own DOM-based authentication, profiles, parent dashboard, modals, and HUD surfaces.
Phaser MUST own world-map presentation, animation, and mini-game scenes. Their integration MUST use
a typed event bridge with explicit subscription and cleanup. Neither frontend layer may become the
authoritative store for persistent progress, rewards, or authorization.

## Technology and Architecture Constraints

- The application stack is React, Phaser 3, ASP.NET Core, PostgreSQL with EF Core, and Cloudflare R2.
- PostgreSQL with EF Core MUST enforce relational integrity and use reviewed, migration-safe schema
  changes. JSONB MUST be limited to metadata or configuration that requires flexible shapes.
- R2 assets MUST be immutable and versioned, with explicit CDN and CORS controls for media delivery.
- The experience MUST support desktop, tablet, and mobile, child-appropriate touch targets,
  reduced motion, and audio or microphone fallback paths.
- The backend MUST enforce child-profile ownership and other domain invariants on every applicable
  request; a client-reported status MUST NOT bypass those checks.

## Approval Boundary and Quality Gates

The SDD backlog's tickets, proposed contracts, and proposed policy values remain DRAFT until
approved by the relevant product, domain, technical, privacy, or legal owner. The decision owners
listed in the backlog are indicative. No implementation may freeze a related business rule or
production contract before its required approval. The following open decisions require resolution:

| ID | Approval-required decision |
|---|---|
| DEC-01 | Placement thresholds, scoring, escalation, and overrides |
| DEC-02 | Stage and island catalog, stage count, dependencies, and skill mapping |
| DEC-03 | Star scoring and unlock thresholds |
| DEC-04 | Session composition rounding and new/review shortage fallback |
| DEC-05 | SRS intervals, mastery transitions, and effort-pass treatment |
| DEC-06 | Speaking evaluation, transcript trust, thresholds, permissions, and fallback |
| DEC-07 | Daily playtime scope, reset timezone, idle handling, and grace period |
| DEC-08 | Guest lifetime, expired-session behavior, retention, and claim limits |
| DEC-09 | Authentication sessions, email verification, password reset, and OAuth linking |
| DEC-10 | Parental gate storage and grant duration; math-gate risk |
| DEC-11 | Food conversion, EXP thresholds, and evolution progression/assets |
| DEC-12 | Content licensing and provider terms for words, images, and audio |
| DEC-13 | Donation verification method, supporter scope, and revocation |
| DEC-14 | Production map assets, coordinates, and artwork dimensions |
| DEC-15 | Privacy jurisdictions, notice, consent, deletion, and speech vendor |
| DEC-16 | Performance SLA, benchmark devices, network, and percentile |
| DEC-17 | Stage-session integrity protocol and offline completion handling |

The backlog identifies DEC-01, DEC-02, DEC-03, DEC-04, DEC-05, DEC-06, DEC-07, DEC-08, DEC-10,
DEC-14, and DEC-17 as blockers to the corresponding M0/M1 P0 work. Privacy and parental-consent
controls MUST be approved before a production pilot. Proposed sample contracts, including the SRS
policy illustration, MUST NOT be treated as approved production contracts.

Delivery MUST follow this sequence: specification, ambiguity resolution, contract review, task
decomposition, implementation, automated verification, and convergence against acceptance criteria.
Release candidates MUST demonstrate compliance with approved ticket requirements and this
constitution. Required evidence and owner sign-off MUST be attached to each ticket before closure.

## Governance

This constitution supersedes conflicting local preferences and informal team practices. Amendments
require written rationale, documented review, and approval from the relevant product and engineering
owners; privacy or legal owners MUST also approve amendments affecting their responsibilities.
Conformance MUST be reviewed during specification, contract review, and release acceptance.

Versioning follows Semantic Versioning: MAJOR for incompatible governance or principle changes,
MINOR for new principles or materially expanded requirements, and PATCH for clarifications or
non-semantic edits. Amendments MUST update the version and last-amended date. The ratification date
records the original adoption and MUST remain unchanged; behavior-changing amendments MUST include
migration or compatibility notes.

**Version**: 1.1.0 | **Ratified**: 2026-10-19 | **Last Amended**: 2026-10-19
