# API_CONTRACT_INDEX — Existing PRD contracts vs Proposed additions

> **Existing** = endpoint explicitly given in user PRD; **Proposed** = engineering design suggestion only (subject to approval). Every endpoint with `childId` must check ownership server-side.

| Status | Method | Endpoint | Owner / Ticket |
|---|---|---|---|
| Existing | POST | `/api/v1/guest/init` | GUEST-001 |
| Existing | POST | `/api/v1/guest/submit-test` | GUEST-003/004 |
| Existing | POST | `/api/v1/auth/register-and-claim` | GUEST-006 |
| Existing | POST | `/api/v1/parent/verify-pin` | AUTH-005 |
| Existing | GET | `/api/v1/parent/children/{id}/report` | PARENT-001 |
| Existing | GET | `/api/v1/game/{childId}/stage-data` | GAME-001 |
| Existing | POST | `/api/v1/game/{childId}/stage-complete` | GAME-002 |
| Existing | POST | `/api/v1/game/{childId}/dragon/feed` | DRAGON-002 |
| Proposed | GET | `/api/v1/game/{childId}/world-map` | MAP-003 |
| Proposed | GET | `/api/v1/game/{childId}/dragon` | DRAGON-001 |
| Proposed | POST | `/api/v1/auth/register`, `/login`, `/logout` | AUTH-001 |
| Proposed | GET/POST/PATCH | `/api/v1/parent/children` | AUTH-003 |
| Proposed | POST/GET | `/api/v1/guest/{guestToken}/answers`, `/next-question` | GUEST-003 |
| Proposed | GET/PATCH | `/api/v1/parent/settings/playtime` | PARENT-003 |
| Proposed | POST | `/api/v1/game/{childId}/dragon/pet` | DRAGON-004 |
| Proposed | POST | `/api/v1/game/{childId}/dragon/equip` | DRAGON-005 |
| Proposed | GET | `/api/v1/parent/donate/qr` | PARENT-004 |

## API invariants

1. No client-supplied `isCorrect`, `earnedExp`, `stars` or `unlocked` is trusted directly.
2. Mutating `claim`, `stage-complete`, `feed`, donation-confirmation require idempotency semantics.
3. Unauthenticated protected routes return 401; unauthorized ownership policy uses 403/404 consistently.
4. Errors use stable `code` and `traceId` (ProblemDetails-style). HTTP status and schemas are refined during `/speckit.plan`.
5. Publishing content uses verified CDN URLs and versioned immutable asset identifiers.
