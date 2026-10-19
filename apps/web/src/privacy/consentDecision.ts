import type { ConsentDecision } from "./consentBridge";

export type SubmitOutcome = "confirmed" | "error";

// Success is reported only after the backend confirms the write.
export async function submitDecision(
  onDecision: (decision: ConsentDecision) => Promise<void>,
  decision: ConsentDecision,
): Promise<SubmitOutcome> {
  try {
    await onDecision(decision);
    return "confirmed";
  } catch {
    return "error";
  }
}
