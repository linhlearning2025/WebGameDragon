import { renderToStaticMarkup } from "react-dom/server";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { ConsentHistory } from "./ConsentHistory";
import type { SpeechSessionResult } from "./consentBridge";
import { createConsentBridge } from "./consentBridge";
import { submitDecision } from "./consentDecision";
import type { ApiResult, PrivacyApi, SpeechSessionGrant } from "./privacyApi";
import { startSpeech } from "./speechSession";

const grant = (expiresAt: string): ApiResult<SpeechSessionGrant> => ({
  ok: true,
  data: { speechSessionId: "session-1", expiresAt },
});

function fakeApi(overrides: Partial<PrivacyApi>): PrivacyApi {
  const fail = () => {
    throw new Error("unexpected call");
  };
  return {
    getNotice: fail,
    getConsentHistory: fail,
    recordConsent: fail,
    startSpeechSession: fail,
    heartbeat: fail,
    ...overrides,
  };
}

describe("consent bridge", () => {
  it("delivers events and stops after unsubscribe", () => {
    const bridge = createConsentBridge();
    const received: string[] = [];
    const off = bridge.on("dragon:consent-changed", (payload) =>
      received.push(payload.decision),
    );

    bridge.emit("dragon:consent-changed", {
      purpose: "microphone_speech",
      decision: "granted",
    });
    off();
    bridge.emit("dragon:consent-changed", {
      purpose: "microphone_speech",
      decision: "withdrawn",
    });

    expect(received).toEqual(["granted"]);
  });
});

describe("startSpeech", () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("gives the child the alternative without any consent prompt when consent is missing", async () => {
    const bridge = createConsentBridge();
    const results: SpeechSessionResult[] = [];
    bridge.on("dragon:speech-session-result", (r) => results.push(r));
    const recordConsent = vi.fn();
    const api = fakeApi({
      recordConsent,
      startSpeechSession: async () => ({
        ok: false,
        status: 403,
        problemType: "consent_required",
      }),
    });

    const controller = await startSpeech("child-1", { api, bridge });

    expect(controller).toBeNull();
    expect(results).toEqual([{ allowed: false, reason: "consent_required" }]);
    expect(recordConsent).not.toHaveBeenCalled();
  });

  it("fails closed on network or unknown errors", async () => {
    const bridge = createConsentBridge();
    const results: SpeechSessionResult[] = [];
    bridge.on("dragon:speech-session-result", (r) => results.push(r));
    const api = fakeApi({
      startSpeechSession: async () => ({
        ok: false,
        status: 500,
        problemType: "other",
      }),
    });

    await startSpeech("child-1", { api, bridge });

    expect(results).toEqual([
      { allowed: false, reason: "privacy_unavailable" },
    ]);
  });

  it("stops capture when the heartbeat is refused", async () => {
    const bridge = createConsentBridge();
    const results: SpeechSessionResult[] = [];
    bridge.on("dragon:speech-session-result", (r) => results.push(r));
    const now = Date.now();
    const heartbeat = vi.fn().mockResolvedValue({
      ok: false,
      status: 403,
      problemType: "lease_refused",
    });
    const api = fakeApi({
      startSpeechSession: async () =>
        grant(new Date(now + 15_000).toISOString()),
      heartbeat,
    });

    await startSpeech("child-1", { api, bridge, now: () => now });
    await vi.advanceTimersByTimeAsync(8_000);

    expect(heartbeat).toHaveBeenCalledTimes(1);
    expect(results.at(-1)).toEqual({ allowed: false, reason: "lease_refused" });

    await vi.advanceTimersByTimeAsync(30_000);
    expect(heartbeat).toHaveBeenCalledTimes(1);
  });

  it("stops renewing as soon as consent is withdrawn", async () => {
    const bridge = createConsentBridge();
    const now = Date.now();
    const heartbeat = vi.fn();
    const api = fakeApi({
      startSpeechSession: async () =>
        grant(new Date(now + 15_000).toISOString()),
      heartbeat,
    });
    await startSpeech("child-1", { api, bridge, now: () => now });

    bridge.emit("dragon:consent-changed", {
      purpose: "microphone_speech",
      decision: "withdrawn",
    });
    await vi.advanceTimersByTimeAsync(30_000);

    expect(heartbeat).not.toHaveBeenCalled();
  });
});

describe("submitDecision", () => {
  it("reports success only after the backend confirms", async () => {
    let resolve: () => void = () => {};
    const pending = new Promise<void>((r) => {
      resolve = r;
    });
    const outcome = submitDecision(() => pending, "granted");
    let settled = false;
    outcome.then(() => {
      settled = true;
    });

    await Promise.resolve();
    expect(settled).toBe(false);

    resolve();
    await expect(outcome).resolves.toBe("confirmed");
  });

  it("reports an error when the write fails", async () => {
    await expect(
      submitDecision(() => Promise.reject(new Error("503")), "granted"),
    ).resolves.toBe("error");
  });
});

describe("ConsentHistory", () => {
  it("shows an empty state", () => {
    expect(renderToStaticMarkup(<ConsentHistory items={[]} />)).toContain(
      "No consent decisions yet.",
    );
  });

  it("flags consent that needs review after a material notice change", () => {
    const html = renderToStaticMarkup(
      <ConsentHistory
        items={[
          {
            purpose: "microphone_speech",
            decision: "granted",
            noticeVersion: "v1",
            decidedAt: "2026-10-19T08:00:00Z",
            valid: false,
          },
        ]}
      />,
    );

    expect(html).toContain("Needs review");
    expect(html).toContain("granted");
  });
});
