# Speech processing: browser behavior and vendor dependencies (FR-011)

Status: pending decision OD-3 (DEC-06, DEC-12, DEC-15). Microphone speaking stays disabled for all
children until a speech approach is approved (FR-026); children use the non-microphone alternative.

## What the product does

- Microphone use needs recorded parent consent for the child, even for browser-only recognition (FR-005).
- The backend decides whether a speech session may start, issues a short lease (proposed 15 seconds),
  and renews it by heartbeat while consent stays valid. Guests never receive a session (FR-027).
- The product does not record, store, or upload raw audio, and does not store transcripts by default.

## What is not yet known

| Question | Answer | Owner |
|----------|--------|-------|
| Which speech approach will be used (browser speech recognition or another provider)? | Pending OD-3 | Privacy + Learning + FE |
| Does audio leave the device, and where does it go? | Pending OD-3 | Privacy |
| Which vendors process the audio, and under which terms? | Pending OD-3 | Legal |

## Notes for the parent notice

Browser speech recognition may send audio to the browser or operating-system vendor, outside this
product's control. The notice must say so once the approach is chosen and must not claim otherwise.
See [processors.md](processors.md) for the vendor checklist.
