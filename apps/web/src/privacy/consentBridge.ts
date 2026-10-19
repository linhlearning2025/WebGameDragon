export type ConsentDecision = "granted" | "denied" | "withdrawn";

export type SpeechDenialReason =
  | "consent_required"
  | "speech_disabled"
  | "privacy_unavailable"
  | "guest_not_allowed"
  | "lease_refused";

export type SpeechSessionResult =
  | { allowed: true; expiresAt: string }
  | { allowed: false; reason: SpeechDenialReason };

export interface ConsentBridgeEvents {
  "dragon:speech-session-result": SpeechSessionResult;
  "dragon:consent-changed": {
    purpose: "microphone_speech";
    decision: ConsentDecision;
  };
}

type Handler<K extends keyof ConsentBridgeEvents> = (
  payload: ConsentBridgeEvents[K],
) => void;

export interface ConsentBridge {
  emit<K extends keyof ConsentBridgeEvents>(
    event: K,
    payload: ConsentBridgeEvents[K],
  ): void;
  on<K extends keyof ConsentBridgeEvents>(
    event: K,
    handler: Handler<K>,
  ): () => void;
}

// Payloads never carry audio, transcripts, tokens, or child names.
export function createConsentBridge(): ConsentBridge {
  const handlers = new Map<string, Set<(payload: never) => void>>();

  return {
    emit(event, payload) {
      for (const handler of handlers.get(event) ?? []) {
        handler(payload as never);
      }
    },
    on(event, handler) {
      const set = handlers.get(event) ?? new Set();
      set.add(handler as (payload: never) => void);
      handlers.set(event, set);
      return () => {
        set.delete(handler as (payload: never) => void);
      };
    },
  };
}
