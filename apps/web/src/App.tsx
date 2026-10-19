import { PhaserHost } from "./game/PhaserHost";

export function App() {
  return (
    <main className="app-shell">
      <header className="app-header">
        <p className="app-kicker">Dragon English Kids</p>
        <h1>Learning world</h1>
        <p className="app-status">Local development environment</p>
      </header>
      <section className="game-section" aria-label="Game workspace">
        <PhaserHost />
      </section>
    </main>
  );
}
