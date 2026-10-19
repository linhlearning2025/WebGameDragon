# REQUIREMENT_TRACEABILITY — PRD → Tickets

PRD: Dragon English Kids supplied by product owner; mappings below are intent/coverage, not proof of completeness.

| PRD Item | Ticket IDs | Verification target |
|---|---|---|
| §1 React ↔ Phaser Event Bridge | PLAT-002, ASSET-002, MAP-004 | E2E event payload, unmount cleanup |
| §1 Audio preload under 100 ms | CONTENT-004, QA-004 | cached audio latency on agreed devices |
| §2 Guest Trial / selection age | GUEST-001, GUEST-002 | init/placement E2E |
| §2 Adaptive Placement / hatch | GUEST-003, GUEST-004, GUEST-005 | threshold matrix and persistent dragon |
| §2 Guest Claim | GUEST-006, AUTH-001 | atomic/idempotent claim |
| §2 Core Game Loop | MAP-001..004, GAME-001..010, DRAGON-001..005 | complete one island end-to-end |
| §3 users | PLAT-003, AUTH-001/002/005, PARENT-003 | schema/auth/security |
| §3 guest_sessions | PLAT-003, GUEST-001..006 | expiry and claim |
| §3 child_profiles | AUTH-003/004, MAP-002 | 5 profiles / isolation |
| §3 dragon_states | DRAGON-001..005 | feed/evolution/inventory |
| §3 vocabularies | CONTENT-001..005 | validation and media publication |
| §3 child_vocab_srs | SRS-001/002 | transitions / review due |
| US-1.1 | AUTH-001, AUTH-002 | email/OAuth |
| US-1.2 | AUTH-003, AUTH-004 | max 5 profiles |
| US-1.3 | AUTH-005, AUTH-006, PARENT-003 | gate/time limits |
| US-1.4 | PARENT-001, PARENT-002 | report metrics |
| US-1.5 | PARENT-004, PARENT-005, DRAGON-005 | QR/entitlement |
| US-2.1 | GUEST-001 | guest init |
| US-2.2 | GUEST-002 | non-punitive egg scene |
| US-2.3 | GUEST-003, GUEST-004 | adaptive rules |
| US-2.4 | GUEST-005, GUEST-006 | egg hatch and claim |
| US-3.1 | DRAGON-001..005 | lair/feed/evolution/pet/skins |
| US-3.2 | GAME-005 | listening |
| US-3.3 | GAME-006 | reading |
| US-3.4 | GAME-007 | spelling |
| US-3.5 | GAME-008, GAME-009 | speaking/mic/effort-pass |
| US-4.1 | CONTENT-003/004/005 | AI/TTS/R2/review |
| US-4.2 | GAME-001, SRS-001/002 | 70/30 selection |
| §5 API contract | API_CONTRACT_INDEX.md; owner tickets | OpenAPI/consumer contract |
| Supplemental child safety / ops | QA-001..005, PLAT-004/005, AUTH-007 | security and production readiness |
