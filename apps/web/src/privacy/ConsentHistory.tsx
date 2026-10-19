import type { ConsentState } from "./privacyApi";

export function ConsentHistory({ items }: { items: ConsentState[] }) {
  return (
    <section className="privacy-panel" aria-labelledby="consent-history-title">
      <h2 id="consent-history-title">Consent history</h2>
      {items.length === 0 ? (
        <p>No consent decisions yet.</p>
      ) : (
        <ul>
          {items.map((item) => (
            <li key={`${item.decidedAt}-${item.decision}`}>
              <span>{item.decision}</span>{" "}
              <time dateTime={item.decidedAt}>{item.decidedAt}</time>
              {item.valid ? null : <strong> Needs review</strong>}
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
