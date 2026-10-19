# Consent Bridge Events (PROPOSED — DRAFT)

Typed events between React and Phaser. Payloads carry no audio, transcript, token, or child name.
The client never decides consent; it only forwards backend results.

| Event | Direction | Payload | Purpose |
|---|---|---|---|
| `dragon:speech-session-result` | React → Phaser | `{ allowed: boolean, reason?: "consent_required" \| "speech_disabled" \| "privacy_unavailable" \| "guest_not_allowed" \| "lease_refused", expiresAt?: string }` | Result of the backend speech-session check; `allowed: false` triggers the non-microphone alternative with no consent prompt to the child. |
| `dragon:consent-changed` | React → Phaser | `{ purpose: "microphone_speech", decision: "granted" \| "denied" \| "withdrawn" }` | Parent changed consent; on `withdrawn` Phaser stops capture immediately and lets the child finish the current question without the microphone. |

**Rules**: When a speech-session heartbeat returns 403, React emits `dragon:speech-session-result` with `allowed: false` and `reason: lease_refused`, and Phaser stops capture immediately. Handlers are subscribed on scene start and removed on scene exit. Phaser never calls the
consent API directly. Version changes to these payloads follow the project's contract versioning.
