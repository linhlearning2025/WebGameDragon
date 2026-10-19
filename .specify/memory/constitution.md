<!--
Sync Impact Report
- Version change: 0.0.0 -> 1.0.0
- Modified principles:
  - [PRINCIPLE_1_NAME] -> I. Child-First Learning
  - [PRINCIPLE_2_NAME] -> II. Single Source of Truth for State and Rewards
  - [PRINCIPLE_3_NAME] -> III. Domain Ownership and Data Minimalism
  - [PRINCIPLE_4_NAME] -> IV. Spec-First Delivery
  - [PRINCIPLE_5_NAME] -> V. Quality, Safety, and Observability
- Added sections:
  - Architecture and Guardrails
  - Quality Gates
- Removed sections: none
- Deferred items: none
-->

# WebGameDragon Constitution

## Core Principles

### I. Child-First Learning
The product MUST prioritize safe, encouraging learning for children ages 5–8. We MUST avoid shame-based feedback, pressure tactics, or paywalls for core learning content. Reward loops MUST be positive, transparent, and designed to support persistence without creating fear or exploitation.

### II. Single Source of Truth for State and Rewards
Game state and reward decisions MUST be governed by approved domain owners and server-side authority. .NET Core is the authoritative system for authentication, unlocks, scores, SRS progression, inventory, EXP, and parent reporting. Phaser remains a client-rendering and interaction layer and MUST NOT independently authorize rewards, mastery claims, unlocks, or supporter status.

### III. Domain Ownership and Data Minimalism
Each domain MUST own its contracts and data boundaries. Identity owns parent and child identity flow; Guest owns trial behavior; Content owns words and media; Learning owns attempts and SRS state; Progress owns unlock state; Dragon owns inventory and EXP; Parent Reporting owns aggregate reporting. We MUST minimize collection of personal data, require parent consent for sensitive data handling, and avoid storing raw audio or child transcripts by default.

### IV. Spec-First Delivery
Every feature, gameplay change, and contract update MUST start from an approved ticket or specification that states the user need, acceptance criteria, scenarios, and required contracts. If a requirement is ambiguous, the team MUST resolve it before implementation. No engineering work may proceed without reviewed acceptance criteria and the relevant domain owner approval.

### V. Quality, Safety, and Observability
All changes MUST pass validation through automated tests, CI checks, and reproducible acceptance verification. We MUST record operational context with trace IDs, and we MUST NOT log tokens, PINs, raw voice data, or sensitive child data. Schema or API changes MUST be versioned, reviewed for backward compatibility, and accompanied by explicit migration or compatibility handling when required.

## Architecture and Guardrails

- React DOM MUST handle auth, profile, parent dashboard, modals, and HUD surfaces.
- Phaser MUST handle the world map, animations, and mini-games through a typed event bridge that is explicitly subscribed and unsubscribed.
- PostgreSQL with EF Core is the system of record for schema, migration safety, integrity, and relational data access; JSONB MUST be limited to metadata and configuration that specifically require flexible shapes.
- Cloudflare R2 storage MUST use immutable, versioned assets with explicit CDN and CORS policy controls for audio and media delivery.
- Idempotency is mandatory for claim, completion, feeding, and donation-confirmation flows so retries and double-clicks cannot create duplicate outcomes.
- Accessibility is mandatory: touch targets must be large enough for children, reduced-motion support must be preserved, audio fallback paths must exist, and the experience must work across desktop, tablet, and mobile platforms.

## Quality Gates

The project MUST follow this delivery sequence: specification draft, ambiguity resolution, contract review, task decomposition, implementation, verification, and convergence against acceptance criteria. Every release candidate MUST demonstrate that the implementation meets the approved ticket requirements and does not violate this constitution.

## Governance

This constitution supersedes conflicting local preferences and informal team habits. Amendments require written rationale, explicit documentation of the change, and approval from the relevant product and engineering owners before the new rule is enforced. Any change to the core principles or governance procedures MUST update the version, ratification date, and last amended date and include migration notes when behavior changes.

**Version**: 1.0.0 | **Ratified**: 2026-10-19 | **Last Amended**: 2026-10-19
