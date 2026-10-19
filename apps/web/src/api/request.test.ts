import { afterEach, describe, expect, it, vi } from "vitest";
import { requestJson } from "./request.js";

describe("requestJson", () => {
  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it("resolves only after the backend confirms a successful response", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue(
        new Response(JSON.stringify({ saved: true }), {
          status: 200,
          headers: { "Content-Type": "application/json" },
        }),
      ),
    );

    await expect(
      requestJson<{ saved: boolean }>("/api/profile", { method: "POST" }),
    ).resolves.toEqual({
      saved: true,
    });
  });

  it("rejects unsuccessful responses without exposing the response body", async () => {
    vi.stubGlobal(
      "fetch",
      vi
        .fn()
        .mockResolvedValue(
          new Response("database password=test-secret", { status: 503 }),
        ),
    );

    await expect(
      requestJson("/api/profile", { method: "POST" }),
    ).rejects.toThrow("HTTP 503");
    await expect(
      requestJson("/api/profile", { method: "POST" }),
    ).rejects.not.toThrow("test-secret");
  });
});
