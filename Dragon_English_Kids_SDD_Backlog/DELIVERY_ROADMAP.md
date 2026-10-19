# DELIVERY_ROADMAP — Corrected dependency-first execution plan

**Baseline:** 2026-10-19 · **Total: 58 tickets** · **Milestone counts:** M0=7, M1=19, M2=23, M3=9.

> Execution order is in [`TICKET_INDEX.md`](TICKET_INDEX.md). Milestones are **release/functional gates**, not a claim every feature can ship independently. All tickets remain DRAFT pending spec approval.

## M0 — Foundation & child-safety prerequisites (7 tickets)

Order: PLAT-001 → QA-002 → PLAT-003 → PLAT-005 → PLAT-002 → CONTENT-001 → PLAT-004

**Exit gate:** React+Phaser/API build and local setup work; database schema migrated; policy for child data/guest and access boundaries reviewed; base vocabulary metadata model available. `PLAT-004` P1 can be scheduled in parallel but remains on the proposed serial checklist.

## M1 — Guest-to-Map demo (19 tickets)

Order: QA-001 → ASSET-001 → CONTENT-002 → GUEST-001 → AUTH-001 → AUTH-003 → AUTH-005 → AUTH-004 → GUEST-003 → GUEST-004 → GUEST-002 → GUEST-005 → GUEST-006 → MAP-001 → MAP-002 → MAP-003 → ASSET-002 → ASSET-003 → DRAGON-001

**Exit gate:** guest session → placement → hatch → parent claim → select child → server-managed World Map and basic Dragon Lair. `ASSET-003` displays dynamic nodes from `MAP-003`. **Do not claim playable 4 mini-games at M1**: navigation integration requiring real Stage Data API is `MAP-004` in M2.

**Content note:** Placement demo can use reviewed fixture media when automated AI/TTS pipeline is unfinished; production placement requires appropriately licensed, approved, reachable image/audio before public release.

## M2 — Learning core + content & play loop (23 tickets)

Order: SRS-001 → SRS-002 → PARENT-003 → CONTENT-003 → CONTENT-004 → CONTENT-005 → GAME-001 → MAP-004 → GAME-002 → GAME-003 → GAME-004 → GAME-005 → GAME-006 → GAME-007 → GAME-008 → GAME-009 → GAME-010 → DRAGON-002 → ASSET-004 → DRAGON-003 → QA-004 → QA-003 → AUTH-002

**Exit gate:** SRS policy, content generation/TTS/R2/publishing or approved ready-to-use equivalent, stage-data API, dynamic stage routing, completion idempotency, scoring, 4 mini-games, feed and evolution, golden-path E2E, cached audio benchmark. Google OAuth (`AUTH-002`) is P1 and is listed at end so it cannot delay the P0 learning path.

**Important release gates (not all hard code dependencies):** Approved content for every promoted stage (`CONTENT-005`); confirmed 70/30 shortage rules (`DEC-04`); rewards/unlock scoring (`DEC-02/03/11`); consent and microphone fallback (`DEC-06/15`); stage session integrity (`DEC-17`). Never pass a public learning milestone on demo fixtures alone.

**E2E scope:** M2 `QA-003` covers guest→placement→claim→map→4 skills→reward→lair/feed; parent report is validated in M3 with `PARENT-001/002`.

## M3 — Parent, release & optional features (9 tickets)

Order: AUTH-007 → PARENT-001 → PARENT-002 → PARENT-004 → DRAGON-005 → PARENT-005 → AUTH-006 → DRAGON-004 → QA-005

**Exit gate:** account deletion/export, report/dashboard behind Parental Gate, restore/rollback runbook and deployment checks. Donate/cosmetics/math alternate gate/pet animation are optional P2 features; they are listed before `QA-005` for a full-scope serial execution but may be deferred by explicit PO decision if release scope excludes them.

## Cross-cutting validation

- Before each ticket: all declared hard dependencies must be DONE; check pending decisions in `OPEN_DECISIONS.md`.
- Before publishing child-facing features: legal/privacy sign-off, licensed media and positive-error UX remain mandatory regardless of scheduling.
- Keep API authorization server-side, use idempotent completion/reward/claim, and do not expose correct answers in stage-data responses.
- For one-person/one-agent delivery: use the numbered list in `TICKET_INDEX.md`, mark progress after each ticket, and test the full core flow after every milestone.

## Changes from the original delivery plan

| Change | Rationale |
|---|---|
| `MAP-004` moved from M1 to M2, depends on `GAME-001` | Its AC requires fetching actual stage-data before Scene start |
| `ASSET-003` depends on `MAP-003` | Dynamic level state/stars should render from the real map DTO |
| `QA-003` gains GUEST-006, GAME-010, DRAGON-002 | Golden-path E2E must have guest claim, summary and feeding |
| M0/M1/M2/M3 counts = 7/19/23/9 | Corrected artifact metadata and release gates |
