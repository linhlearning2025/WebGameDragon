# Audit access, key management, and tamper evidence (QA-002)

Status: DRAFT. Items marked "pending Security" need Security's agreement before this task is closed.

## Who can see audit records

- Designated security and privacy staff see all audit records. No public API exposes audit events;
  access is through approved database or operations tooling only.
- A parent sees only the consent history for their own children, through the consent history
  endpoint. That endpoint returns consent decisions, never audit events.
- Audit events never contain raw audio, transcripts, tokens, PINs, credentials, or direct child
  identifiers. A child is referenced only by a keyed pseudonym.

## Pseudonym key

- The key comes from the `Privacy__AuditPseudonymKey` environment variable. The API refuses to start
  without it, and the value is never written to logs or responses.
- Treat the key as a secret: store it in the hosting secret store, restrict who can read it, and
  never commit it.
- Rotation: changing the key changes every pseudonym, so older events can no longer be matched to
  newer ones by pseudonym. Key versioning is pending Security: record which key version produced each
  event before the first rotation, or accept the break in correlation.

## Tamper evidence

- Minimum, implemented: consent records and audit events are append-only in the application; any
  attempt to change or delete them through the data layer fails.
- Stronger protection (for example a restricted database role without update or delete rights, or a
  hash chain) is pending Security. No stronger level is claimed here.

## Retention

Audit retention is not set. It is a decision for Security (see `audit_policy` in
`data-inventory.yaml`) and stays open until approved.
