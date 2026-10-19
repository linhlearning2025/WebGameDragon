import React, { useEffect, useRef } from 'react';
import Phaser from 'phaser';
import WorldMapScene from '../phaser/WorldMapScene.js';

// Pass childId from your authenticated React route. NEVER trust a childId alone for auth.
// The backend must verify that the logged-in parent owns the child profile.
export default function DragonWorldMap({ childId, onStageSelected }) {
  const parentRef = useRef(null);
  const gameRef = useRef(null);
  useEffect(() => {
    const controller = new AbortController();
    let disposed = false;
    (async () => {
      const res = await fetch(`/api/v1/game/${encodeURIComponent(childId)}/world-map`, {
        credentials: 'include', signal: controller.signal,
      });
      if (!res.ok) throw new Error(`World map HTTP ${res.status}`);
      const mapData = await res.json();
      if (disposed || !parentRef.current) return;
      const game = new Phaser.Game({
        type: Phaser.AUTO, parent: parentRef.current,
        width: 1600, height: 900, backgroundColor: '#74ccf8',
        scale: { mode: Phaser.Scale.FIT, autoCenter: Phaser.Scale.CENTER_BOTH },
        scene: [new WorldMapScene(mapData)],
      });
      gameRef.current = game;
      game.events.on('dragon:stage-selected', onStageSelected);
    })().catch(err => { if (err.name !== 'AbortError') console.error(err); });
    return () => {
      disposed = true;
      controller.abort();
      if (gameRef.current) {
        gameRef.current.events.off('dragon:stage-selected', onStageSelected);
        gameRef.current.destroy(true);
        gameRef.current = null;
      }
    };
  }, [childId, onStageSelected]);
  return <div ref={parentRef} style={{ width: '100%', height: '100%', overflow: 'hidden' }} />;
}

// Example: <DragonWorldMap childId={currentChild.id}
//   onStageSelected={({ stageId, skill }) => navigate(`/game/${skill}/${stageId}`)} />
