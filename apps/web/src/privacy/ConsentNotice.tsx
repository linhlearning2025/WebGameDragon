import { useState } from "react";
import type { ConsentDecision } from "./consentBridge";
import { submitDecision } from "./consentDecision";

export interface ConsentNoticeProps {
  notice: { noticeVersion: string; content: string };
  onDecision: (decision: ConsentDecision) => Promise<void>;
  // Placeholder until AUTH-005 provides the parental gate.
  gatePassed?: boolean;
}

type Status = "idle" | "saving" | "granted" | "denied" | "error";

const messages: Record<Status, string> = {
  idle: "",
  saving: "Saving your choice...",
  granted: "Microphone use is allowed for this child.",
  denied:
    "Microphone use is not allowed. Your child will use the other activity.",
  error: "We could not save your choice. Please try again.",
};

export function ConsentNotice({
  notice,
  onDecision,
  gatePassed = true,
}: ConsentNoticeProps) {
  const [status, setStatus] = useState<Status>("idle");

  if (!gatePassed) {
    return (
      <section className="privacy-panel" aria-labelledby="consent-title">
        <h2 id="consent-title">Microphone permission</h2>
        <p>Please ask a parent to continue.</p>
      </section>
    );
  }

  const decide = async (decision: "granted" | "denied") => {
    setStatus("saving");
    const outcome = await submitDecision(onDecision, decision);
    setStatus(outcome === "confirmed" ? decision : "error");
  };

  return (
    <section className="privacy-panel" aria-labelledby="consent-title">
      <h2 id="consent-title">Microphone permission</h2>
      <p>{notice.content}</p>
      <div className="privacy-actions">
        <button
          type="button"
          className="privacy-button"
          disabled={status === "saving"}
          onClick={() => decide("granted")}
        >
          Allow microphone
        </button>
        <button
          type="button"
          className="privacy-button"
          disabled={status === "saving"}
          onClick={() => decide("denied")}
        >
          Do not allow
        </button>
      </div>
      <p role="status" aria-live="polite">
        {messages[status]}
      </p>
    </section>
  );
}
