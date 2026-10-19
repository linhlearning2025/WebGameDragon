import { useState } from "react";
import {
  type PrivacyRequestApi,
  type PrivacyRequestStatus,
  type PrivacyRequestType,
  submitPrivacyRequest,
} from "./privacyRequest";

const stateMessages: Record<PrivacyRequestStatus["status"], string> = {
  pending_verification: "Waiting for a parent to confirm this request.",
  accepted: "Your request was accepted.",
  in_progress: "Your request is being handled.",
  completed: "Your request is complete.",
  rejected: "Your request could not be accepted.",
};

export function PrivacyRequestStatusView({
  request,
}: {
  request: PrivacyRequestStatus;
}) {
  return (
    <p role="status" aria-live="polite">
      {stateMessages[request.status]} We aim to finish by{" "}
      <time dateTime={request.dueAt}>{request.dueAt.slice(0, 10)}</time>.
    </p>
  );
}

export interface PrivacyRequestsProps {
  api: PrivacyRequestApi;
  childId: string;
  newKey?: () => string;
}

export function PrivacyRequests({
  api,
  childId,
  newKey = () => crypto.randomUUID(),
}: PrivacyRequestsProps) {
  const [request, setRequest] = useState<PrivacyRequestStatus | null>(null);
  const [failed, setFailed] = useState(false);
  const [busy, setBusy] = useState(false);

  const submit = async (type: PrivacyRequestType) => {
    setBusy(true);
    setFailed(false);
    const result = await submitPrivacyRequest(api, childId, type, newKey());
    if (result.outcome === "confirmed") {
      setRequest(result.request);
    } else {
      setFailed(true);
    }
    setBusy(false);
  };

  return (
    <section className="privacy-panel" aria-labelledby="privacy-requests-title">
      <h2 id="privacy-requests-title">Your child's data</h2>
      <div className="privacy-actions">
        <button
          type="button"
          className="privacy-button"
          disabled={busy}
          onClick={() => submit("access")}
        >
          Ask for a copy of the data
        </button>
        <button
          type="button"
          className="privacy-button"
          disabled={busy}
          onClick={() => submit("deletion")}
        >
          Ask to delete the data
        </button>
      </div>
      {request ? <PrivacyRequestStatusView request={request} /> : null}
      {failed ? (
        <p role="alert">We could not send your request. Please try again.</p>
      ) : null}
    </section>
  );
}
