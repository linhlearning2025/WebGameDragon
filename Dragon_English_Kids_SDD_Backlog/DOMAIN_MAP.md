# DOMAIN_MAP — Bounded contexts & ownership

| Bounded context | Owns | Does not own | Event / integration |
|---|---|---|---|
| Identity & Child Profiles | users, auth sessions, children, parental gate | learning score | `ProfileChanged`, authorization policy |
| Guest Trial | guest token, placement attempts, hatch claim | permanent child auth | `GuestClaimed`, `PlacementCompleted` |
| Content & Media | vocabulary, metadata, AI generation, TTS, R2 URLs, publication | child mastery | published content manifest/version |
| World Map & Progress | island/stage config, prerequisites, child stage progress | correctness of each attempt | `StageCompleted` → unlock calculation |
| Learning & SRS | session attempts/outcomes, spaced repetition | child rewards/inventory | `AttemptEvaluated`, `ReviewDue` |
| Rewards & Dragon | food ledger, EXP, evolution, mood, cosmetics | authentication, SRS calculation | `StageRewardGranted`, `DragonFed` |
| Parent Dashboard | reporting projections, limits/consent/settings, donation entitlement | core learning state | query/read models, payment verification |
| Phaser Presentation | animated UI, hit areas, scene navigation | persistent source of truth | typed event bridge |

## Domain invariants

- One active authenticated parent may own up to five active child profiles (enforced transactionally).
- A child cannot access another child's progress; backend ownership checks are mandatory.
- Guest claim cannot be reused across parents and should be idempotent within same principal/operation.
- A stage completion earns rewards **at most once per stageSessionId**.
- `bestStars` is monotonic nondecreasing for repeated attempts unless an authorized reset operation is specified separately.
- `evolution_level ∈ [1,5]`, `mastery_level ∈ [0,5]`, inventory never negative.
- `EFFORT_PASS` allows flow but **does not equal phonetic correctness or mastery gain**.
- Only PUBLISHED content may enter stage generation.
- Client-provided unlock/status/EXP/accuracy/claimed donation must not be trusted.

## Terminology

- **Tier:** Cambridge STARTERS / MOVERS. **Topic:** Animals/Food/Colors/etc. A **stage** is a playable learning unit for one **skill**.
- **Skill:** listening, reading, spelling, speaking. **Island:** a visual/topic grouping containing stages.
- **Guest Session:** limited-scope temporary trial; **Child Profile:** claimed persistent profile.
- **SRS Due:** vocabulary item with `next_review_at <= nowUTC` and eligible for recall.
- **Effort pass:** motivational continuation, not evidence of a correct answer.
- **Spec status DRAFT:** not yet approved business rules; dev can spike but should not freeze contracts.
