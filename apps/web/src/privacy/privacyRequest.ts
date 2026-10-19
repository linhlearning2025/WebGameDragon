import { type ApiResult, call } from "./privacyApi";

export type PrivacyRequestType = "access" | "deletion";

export type PrivacyRequestState =
  | "pending_verification"
  | "accepted"
  | "in_progress"
  | "completed"
  | "rejected";

export interface PrivacyRequestStatus {
  requestId: string;
  type: PrivacyRequestType;
  status: PrivacyRequestState;
  dueAt: string;
}

export interface PrivacyRequestApi {
  createPrivacyRequest(
    childId: string,
    type: PrivacyRequestType,
    idempotencyKey: string,
  ): Promise<ApiResult<PrivacyRequestStatus>>;
  getPrivacyRequest(
    requestId: string,
  ): Promise<ApiResult<PrivacyRequestStatus>>;
}

export function createPrivacyRequestApi(
  fetchFn: typeof fetch = fetch,
): PrivacyRequestApi {
  return {
    createPrivacyRequest: (childId, type, idempotencyKey) =>
      call(fetchFn, `/api/v1/children/${childId}/privacy-requests`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Idempotency-Key": idempotencyKey,
        },
        body: JSON.stringify({ type }),
      }),
    getPrivacyRequest: (requestId) =>
      call(fetchFn, `/api/v1/privacy-requests/${requestId}`),
  };
}

// The request is shown only after the backend has confirmed it.
export type SubmitRequestOutcome =
  | { outcome: "confirmed"; request: PrivacyRequestStatus }
  | { outcome: "error" };

export async function submitPrivacyRequest(
  api: PrivacyRequestApi,
  childId: string,
  type: PrivacyRequestType,
  idempotencyKey: string,
): Promise<SubmitRequestOutcome> {
  const result = await api.createPrivacyRequest(childId, type, idempotencyKey);
  return result.ok
    ? { outcome: "confirmed", request: result.data }
    : { outcome: "error" };
}
