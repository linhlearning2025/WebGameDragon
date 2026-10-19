import Phaser from "phaser";
import { useEffect, useRef } from "react";

class DevelopmentScene extends Phaser.Scene {
  constructor() {
    super("development-shell");
  }

  create() {
    this.add.text(28, 26, "Dragon English Kids", {
      color: "#173c35",
      fontFamily: "Georgia, serif",
      fontSize: "28px",
    });
    this.add.text(30, 66, "World and learning scenes will load here", {
      color: "#365b53",
      fontFamily: "sans-serif",
      fontSize: "15px",
    });
  }
}

export function PhaserHost() {
  const hostRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const parent = hostRef.current;
    if (!parent) return;

    const game = new Phaser.Game({
      type: Phaser.AUTO,
      width: 960,
      height: 540,
      backgroundColor: "#e6f1ed",
      parent,
      scene: DevelopmentScene,
      scale: {
        mode: Phaser.Scale.RESIZE,
        autoCenter: Phaser.Scale.CENTER_BOTH,
      },
    });

    return () => game.destroy(true);
  }, []);

  return (
    <section
      className="phaser-host"
      ref={hostRef}
      aria-label="Game presentation area"
    />
  );
}
