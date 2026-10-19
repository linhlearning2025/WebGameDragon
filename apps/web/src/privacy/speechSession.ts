import type {
  ConsentBridge,
  SpeechDenialReason,
  SpeechSessionResult,
} from "./consentBridge";
import type { PrivacyApi } from "./privacyApi";

export interface SpeechController {
  stop(): void;
}

export interface SpeechDependencies {
  api: PrivacyApi;
  bridge: ConsentBridge;
  now?: () => number;
}

function reasonFrom(problemType: string): SpeechDenialReason {
  switch (problemType) {
    case "consent_required":
    case "speech_disabled":
    case "guest_not_allowed":
    case "lease_refused":
    case "privacy_unavailable":
      return problemType;
    default:
      // Unknown failures fail closed.
      return "privacy_unavailable";
  }
}

// Asks the backend first; the child never sees a consent prompt, only the alternative activity.
export async function startSpeech(
  childId: string,
  { api, bridge, now = Date.now }: SpeechDependencies,
): Promise<SpeechController | null> {
  const started = await api.startSpeechSession(childId);
  if (!started.ok) {
    bridge.emit("dragon:speech-session-result", {
      allowed: false,
      reason: reasonFrom(started.problemType),
    });
    return null;
  }

  let timer: ReturnType<typeof setTimeout> | undefined;
  let stopped = false;
  const sessionId = started.data.speechSessionId;

  const stop = () => {
    stopped = true;
    if (timer !== undefined) {
      clearTimeout(timer);
    }
    unsubscribe();
  };

  const unsubscribe = bridge.on("dragon:consent-changed", ({ decision }) => {
    if (decision !== "granted") {
      stop();
    }
  });

  const schedule = (expiresAt: string) => {
    const remaining = new Date(expiresAt).getTime() - now();
    timer = setTimeout(renew, Math.max(1000, remaining / 2));
  };

  const renew = async () => {
    if (stopped) {
      return;
    }

    const renewed = await api.heartbeat(childId, sessionId);
    if (stopped) {
      return;
    }

    if (!renewed.ok) {
      stop();
      const result: SpeechSessionResult = {
        allowed: false,
        reason: "lease_refused",
      };
      bridge.emit("dragon:speech-session-result", result);
      return;
    }

    schedule(renewed.data.expiresAt);
  };

  schedule(started.data.expiresAt);
  bridge.emit("dragon:speech-session-result", {
    allowed: true,
    expiresAt: started.data.expiresAt,
  });

  return { stop };
}
