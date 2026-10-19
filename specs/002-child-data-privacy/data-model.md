# Data Model: QA-002 — Child Data Privacy, Consent & Data Protection

All entities are relational. Child identifiers are opaque until AUTH-003 adds a foreign key. Retention
values are configuration (proposed, pending approval), not model constants.

## ConsentRecord (append-only)

| Field | Type | Rules |
|---|---|---|
| Id | uuid | Primary key |
| ParentId | uuid | Required; must own the child (backend-checked) |
| ChildId | uuid | Required; consent never applies across children |
| Purpose | text | Enumerated (`microphone_speech` in the first release) |
| NoticeVersion | text | Required; version of the notice shown |
| Decision | text | `granted`, `denied`, `withdrawn` |
| DecidedAt | timestamptz | Server time |
| CorrelationId | text | For audit linkage |
| IdempotencyKey | text | Required; unique with (ParentId, ChildId) so retries replay the same decision |

**Rules**: Current state is the latest record per (ChildId, Purpose). A `granted` record is valid only
if its NoticeVersion is not superseded by a material change. Records are never updated or deleted by
normal flows. Unknown state evaluates as not consented.

**State transitions**: none → granted | denied; granted → withdrawn; denied → granted; withdrawn → granted.

## SpeechSession

| Field | Type | Rules |
|---|---|---|
| Id | uuid | Primary key |
| ChildId | uuid | Required |
| ConsentRecordId | uuid | Consent relied upon |
| StartedAt / ExpiresAt | timestamptz | Short lease (proposed 15 s), renewed by heartbeat while consent stays valid |
| RevokedAt | timestamptz? | Set on withdrawal |

**Rules**: Created only when speech is enabled, consent is valid, ownership passes, and the caller is not a guest. Revoked on
withdrawal. Contains no audio or transcript.

## PrivacyRequest

| Field | Type | Rules |
|---|---|---|
| Id | uuid | Primary key |
| ParentId / ChildId | uuid | Owner and subject |
| Type | text | `access`, `deletion` |
| Status | text | `pending_verification`, `accepted`, `in_progress`, `completed`, `rejected` |
| IdempotencyKey | text | Required; unique with (ParentId, ChildId) |
| CreatedAt | timestamptz | Server time |
| VerifiedAt | timestamptz? | Set after parental gate and email confirmation |
| DueAt | timestamptz | Target date from configuration (proposed 30 days) |
| CompletedAt | timestamptz? | Set on completion |

**Rules**: Accepted only after verification; non-owners are rejected without disclosure. Execution of
export/deletion belongs to AUTH-007.

## AuditEvent (append-only)

| Field | Type | Rules |
|---|---|---|
| Id | uuid | Primary key |
| OccurredAt | timestamptz | Server time |
| ActorRole | text | `parent`, `child`, `guest`, `staff`, `system` |
| Action | text | Consent decision, withdrawal, privacy request, denied access, sensitive read, cleanup outcome |
| SubjectRef | text | HMAC of child id (key from environment) |
| Outcome | text | `success`, `denied`, `failed` |
| CorrelationId | text | Required |

**Rules**: No raw audio, transcript, token, PIN, credential, or direct child identifier. Staff-only
read; parents see consent history for their own children via ConsentRecord, not AuditEvent.

## RetentionRun

| Field | Type | Rules |
|---|---|---|
| Id | uuid | Primary key |
| StartedAt / FinishedAt | timestamptz | Finish null while running |
| Outcome | text | `succeeded`, `failed`, `partial` |
| ItemsDeleted | integer | Count only |
| FailureCode | text? | No sensitive content |

**Rules**: Each run is idempotent; failed or partial runs are retried on the next interval; failures are
visible to operators.

## DataInventoryEntry (file: docs/privacy/data-inventory.yaml)

Fields: name, class (`personal`, `sensitive`, `temporary`, `non_sensitive`), purpose, collection point,
recipients, retention key, deletion method. Every persisted field must appear; a test enforces it.

## Relationships

- ConsentRecord, SpeechSession, PrivacyRequest reference ChildId and ParentId (FK to Identity tables after AUTH-003).
- SpeechSession → ConsentRecord.
- AuditEvent references subjects only through SubjectRef.
