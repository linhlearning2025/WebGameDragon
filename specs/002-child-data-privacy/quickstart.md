# Quickstart: QA-002 Validation Guide

Run after implementation. Prerequisites: PLAT-001 environment, `ConnectionStrings__Default`, and
`Privacy__AuditPseudonymKey` set in the environment (no secrets in files). No Docker is used.

## Setup

```powershell
pnpm install --frozen-lockfile
dotnet restore apps/api --locked-mode
dotnet ef database update --project apps/api
```

## Automated checks

```powershell
dotnet test apps/api/tests/DragonEnglishKids.Api.Tests --no-restore
pnpm test
pnpm lint
```

Expected: all privacy, retention, redaction, authorization, and inventory tests pass; the PostgreSQL
integration tests run (not skipped) when the connection setting is present.

## Scenario validation

| Scenario | How to verify | Expected outcome |
|---|---|---|
| AS-01 consent granted | Grant consent for a test child, request a speech session | Consent recorded with notice version; speech allowed for that child and purpose only |
| AS-02 consent denied | Deny consent, start speaking | Denial recorded; no capture; child gets non-microphone alternative with no prompt |
| AS-03 no consent | Request speech session without valid consent | 403 `consent_required`; no audio capture |
| AS-04 guest expiry | Use a fake clock past 24 hours | Token refused with re-start message; new empty session can start |
| AS-05 guest deletion | Run retention after expiry | Expired temporary data removed within the configured window |
| AS-06 unauthorized access | Request another parent's child consent as non-owner | 403, no data disclosed, minimized audit event |
| AS-07 log redaction | Run flows, scan captured logs and audit rows | No raw audio, transcript, token, PIN, credential, or child identifier |
| AS-08 withdrawal | Withdraw consent mid-activity | Speech session revoked; capture stops immediately; child continues without microphone; a withdrawal from another device stops capture within the lease |
| AS-09 privacy unavailable | Make consent lookup fail | 503, fails closed, never treated as consent |
| AS-10 cleanup retry | Fail a retention run midway, rerun | Idempotent retry; no duplicates or misses; failure visible to operators |
| AS-11 guest asks for speech | Request a speech session as a guest | 403 `guest_not_allowed`; no session; child gets the non-microphone alternative |

## Evidence (2026-10-19)

Run against a local native PostgreSQL 17.11 (official portable binaries, no Docker), database
`dragon_english_kids`, with the `PrivacyFoundation` migration applied:

```powershell
$env:ConnectionStrings__Default = 'Host=127.0.0.1;Port=54329;Database=dragon_english_kids;Username=dragon_dev;Password=<set-locally>'
$env:Privacy__AuditPseudonymKey = '<set-locally-random-secret>'
dotnet ef database update --project apps/api/DragonEnglishKids.Api.csproj --startup-project apps/api/DragonEnglishKids.Api.csproj
dotnet test apps/api/tests/DragonEnglishKids.Api.Tests/DragonEnglishKids.Api.Tests.csproj --no-restore
```

Result: 94 passed, 0 failed, 0 skipped (the PostgreSQL integration tests ran, none were skipped).

| Scenario | Real-database evidence (PostgresPrivacyIntegrationTests) |
|---|---|
| AS-01, AS-08 | `GrantStartSpeechAndWithdrawArePersistedAndRevokeTheSession`, `SpeechLeaseIsRenewedAndThenRefusedAfterWithdrawal` |
| AS-02, AS-03 | `DeniedOrMissingConsentRefusesSpeechAndStoresNoSession` |
| AS-06, AS-11 | `NonOwnerAndGuestAreRefusedAndAuditedWithoutAnyStoredData` |
| AS-07 | `AuditRowsInTheDatabaseNeverContainTheChildIdentifier` (plus log and response scans in `RedactionTests`) |
| AS-09 | `UnreachableDatabaseFailsClosedWithServiceUnavailable` (a genuinely unreachable database) |
| AS-10, AS-05 | `RetentionRunsAreRecordedAndFailuresCanBeRetried` |
| FR-020 idempotency, append-only | `IdempotentReplayAndUniqueKeyAreEnforcedByTheDatabase`, `ConsentRecordsAreAppendOnlyInTheDatabase` |
| Privacy requests | `PrivacyRequestsArePersistedWithTheirDueDate` |

Limits: AS-04 and the deletion part of AS-05 and AS-10 use a fake guest store and the expiry policy,
because the guest tables do not exist until GUEST-001 and PLAT-003. Ownership, parental-gate
verification, and sign-in use test fakes until PLAT-005, AUTH-001, and AUTH-005.

## Release gates (not testable in CI)

- Legal approval of notice wording and the Vietnam-only market (OD-1).
- Approval of proposed retention values and backup purge (OD-2).
- Approval of a speech approach and vendor before enabling microphone speaking (OD-3).
- No legal-compliance claim is made by this feature.

Contracts: [privacy.openapi.yaml](contracts/privacy.openapi.yaml), [consent-events.md](contracts/consent-events.md).
Entities: [data-model.md](data-model.md).
