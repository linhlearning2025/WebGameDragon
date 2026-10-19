import { renderToStaticMarkup } from "react-dom/server";
import { describe, expect, it } from "vitest";
import { ConsentHistory } from "./ConsentHistory";
import { ConsentNotice } from "./ConsentNotice";

const notice = { noticeVersion: "v1", content: "Placeholder notice text." };

describe("consent UI accessibility", () => {
  const html = renderToStaticMarkup(
    <ConsentNotice notice={notice} onDecision={async () => {}} />,
  );

  it("labels the region by its heading", () => {
    expect(html).toContain('aria-labelledby="consent-title"');
    expect(html).toContain('id="consent-title"');
  });

  it("uses real buttons with visible text for both choices", () => {
    expect(html).toContain('type="button"');
    expect(html).toContain("Allow microphone");
    expect(html).toContain("Do not allow");
  });

  it("announces results politely without relying on audio", () => {
    expect(html).toContain('role="status"');
    expect(html).toContain('aria-live="polite"');
    expect(html).not.toContain("<audio");
  });

  it("asks for a parent instead of showing choices when the gate is not passed", () => {
    const gated = renderToStaticMarkup(
      <ConsentNotice
        notice={notice}
        onDecision={async () => {}}
        gatePassed={false}
      />,
    );

    expect(gated).toContain("ask a parent");
    expect(gated).not.toContain("Allow microphone");
  });

  it("labels the history region by its heading", () => {
    const history = renderToStaticMarkup(<ConsentHistory items={[]} />);

    expect(history).toContain('aria-labelledby="consent-history-title"');
  });
});
