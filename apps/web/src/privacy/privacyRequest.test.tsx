import { renderToStaticMarkup } from "react-dom/server";
import { describe, expect, it, vi } from "vitest";
import { PrivacyRequestStatusView, PrivacyRequests } from "./PrivacyRequests";
import type { ApiResult } from "./privacyApi";
import {
  type PrivacyRequestApi,
  type PrivacyRequestStatus,
  submitPrivacyRequest,
} from "./privacyRequest";

const pending: PrivacyRequestStatus = {
  requestId: "r-1",
  type: "deletion",
  status: "pending_verification",
  dueAt: "2026-11-18T08:00:00Z",
};

function apiReturning(
  result: ApiResult<PrivacyRequestStatus>,
): PrivacyRequestApi {
  return {
    createPrivacyRequest: vi.fn().mockResolvedValue(result),
    getPrivacyRequest: vi.fn().mockResolvedValue(result),
  };
}

describe("submitPrivacyRequest", () => {
  it("reports the request only after the backend confirms it", async () => {
    const api = apiReturning({ ok: true, data: pending });

    await expect(
      submitPrivacyRequest(api, "child-1", "deletion", "key-1"),
    ).resolves.toEqual({ outcome: "confirmed", request: pending });
    expect(api.createPrivacyRequest).toHaveBeenCalledWith(
      "child-1",
      "deletion",
      "key-1",
    );
  });

  it("reports an error and never a request when the backend refuses", async () => {
    const api = apiReturning({ ok: false, status: 403, problemType: "other" });

    await expect(
      submitPrivacyRequest(api, "child-1", "access", "key-1"),
    ).resolves.toEqual({ outcome: "error" });
  });

  it("reports an error when the backend is unreachable", async () => {
    const api = apiReturning({
      ok: false,
      status: 0,
      problemType: "privacy_unavailable",
    });

    await expect(
      submitPrivacyRequest(api, "child-1", "access", "key-1"),
    ).resolves.toEqual({ outcome: "error" });
  });
});

describe("PrivacyRequestStatusView", () => {
  it("shows the backend status and the target date", () => {
    const html = renderToStaticMarkup(
      <PrivacyRequestStatusView request={pending} />,
    );

    expect(html).toContain("Waiting for a parent to confirm");
    expect(html).toContain("2026-11-18");
    expect(html).toContain('role="status"');
  });

  it("does not claim acceptance for a request that is still pending", () => {
    const html = renderToStaticMarkup(
      <PrivacyRequestStatusView request={pending} />,
    );

    expect(html).not.toContain("was accepted");
  });
});

describe("PrivacyRequests", () => {
  it("starts with two labelled buttons and no status or error", () => {
    const html = renderToStaticMarkup(
      <PrivacyRequests
        api={apiReturning({ ok: true, data: pending })}
        childId="child-1"
      />,
    );

    expect(html).toContain("Ask for a copy of the data");
    expect(html).toContain("Ask to delete the data");
    expect(html).toContain('aria-labelledby="privacy-requests-title"');
    expect(html).not.toContain('role="status"');
    expect(html).not.toContain('role="alert"');
  });
});
