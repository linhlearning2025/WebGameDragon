import type { ConsentDecision, SpeechDenialReason } from "./consentBridge";

export interface Notice {
  purpose: string;
  noticeVersion: string;
  content: string;
}

export interface ConsentState {
  purpose: string;
  decision: ConsentDecision;
  noticeVersion: string;
  decidedAt: string;
  valid: boolean;
}

export interface SpeechSessionGrant {
  speechSessionId: string;
  expiresAt: string;
}

export type ApiResult<T> =
  | { ok: true; data: T }
  | { ok: false; status: number; problemType: SpeechDenialReason | "other" };

export interface PrivacyApi {
  getNotice(purpose: string): Promise<ApiResult<Notice>>;
  getConsentHistory(
    childId: string,
  ): Promise<ApiResult<{ items: ConsentState[] }>>;
  recordConsent(
    childId: string,
    purpose: string,
    decision: ConsentDecision,
    noticeVersion: string,
    idempotencyKey: string,
  ): Promise<ApiResult<ConsentState>>;
  startSpeechSession(childId: string): Promise<ApiResult<SpeechSessionGrant>>;
  heartbeat(
    childId: string,
    speechSessionId: string,
  ): Promise<ApiResult<SpeechSessionGrant>>;
}

const knownProblems: ReadonlySet<string> = new Set<SpeechDenialReason>([
  "consent_required",
  "speech_disabled",
  "privacy_unavailable",
  "guest_not_allowed",
  "lease_refused",
]);

export async function call<T>(
  fetchFn: typeof fetch,
  url: string,
  init?: RequestInit,
): Promise<ApiResult<T>> {
  try {
    const response = await fetchFn(url, init);
    if (response.ok) {
      return { ok: true, data: (await response.json()) as T };
    }

    // Only the problem type is read; response bodies are never surfaced.
    let problemType: SpeechDenialReason | "other" = "other";
    try {
      const body = (await response.json()) as { type?: string };
      if (body.type && knownProblems.has(body.type)) {
        problemType = body.type as SpeechDenialReason;
      }
    } catch {
      // Unreadable problem body; keep "other".
    }

    return { ok: false, status: response.status, problemType };
  } catch {
    return { ok: false, status: 0, problemType: "privacy_unavailable" };
  }
}

export function createPrivacyApi(fetchFn: typeof fetch = fetch): PrivacyApi {
  const json = { "Content-Type": "application/json" };

  return {
    getNotice: (purpose) =>
      call(
        fetchFn,
        `/api/v1/privacy/notice?purpose=${encodeURIComponent(purpose)}`,
      ),
    getConsentHistory: (childId) =>
      call(fetchFn, `/api/v1/children/${childId}/consents`),
    recordConsent: (childId, purpose, decision, noticeVersion, key) =>
      call(fetchFn, `/api/v1/children/${childId}/consents/${purpose}`, {
        method: "PUT",
        headers: { ...json, "Idempotency-Key": key },
        body: JSON.stringify({ decision, noticeVersion }),
      }),
    startSpeechSession: (childId) =>
      call(fetchFn, `/api/v1/children/${childId}/speech-sessions`, {
        method: "POST",
      }),
    heartbeat: (childId, speechSessionId) =>
      call(
        fetchFn,
        `/api/v1/children/${childId}/speech-sessions/${speechSessionId}/heartbeat`,
        { method: "POST" },
      ),
  };
}
