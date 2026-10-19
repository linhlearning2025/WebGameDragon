// Dragon English Kids / Phaser 3 World Map scene.
// This scene renders PNG assets according to backend-provided worldMap JSON.
// Import it in a React project that has `phaser` installed.
import Phaser from 'phaser';

const BASE = '/assets/game';
const SKILLS = ['listening', 'reading', 'spelling', 'speaking'];
const THEMES = ['lair', 'forest', 'food', 'crystal', 'family', 'school', 'toys', 'nature', 'castle', 'ice', 'desert', 'grass'];
const NODE_STATES = ['available', 'current', 'completed', 'locked', 'review'];
const DRAGON_SKINS = ['red', 'blue', 'green', 'purple', 'pink', 'yellow'];
const IMAGE = (path) => `${BASE}/${path}.png`;

export default class WorldMapScene extends Phaser.Scene {
  constructor(worldMap) {
    super({ key: 'DragonWorldMap' });
    this.mapData = worldMap;
    this.selectedIslandId = worldMap.world.activeIslandId ?? 'lair';
    this.dynamicObjects = [];
  }

  preload() {
    const load = (key, path) => this.load.image(key, IMAGE(path));
    const name = this.mapData.world.background === 'sky_sunset' ? 'sky_sunset' : 'sky_day';
    load('sky', `world-map/backgrounds/${name}`);
    load('mountains', 'world-map/backgrounds/distant_mountains');
    load('clouds', 'world-map/backgrounds/clouds_overlay');
    THEMES.forEach(x => load(`island:${x}`, `world-map/islands/${x}_island`));
    NODE_STATES.forEach(x => load(`state:${x}`, `level-nodes/states/${x}`));
    SKILLS.forEach(x => load(`skill:${x}`, `level-nodes/${x}/node`));
    DRAGON_SKINS.forEach(x => load(`dragon:${x}`, `dragon/baby/baby_${x}`));
    for (const icon of ['star', 'coin', 'heart', 'exp']) load(`icon:${icon}`, `ui/icons/${icon}`);
    load('panel', 'ui/panels/parchment');
    load('back', 'ui/buttons/back');
    load('progressFrame', 'ui/progress/bar_frame');
    load('progressFill', 'ui/progress/fill_exp');
  }

  create() {
    const { width, height } = this.mapData.world;
    this.cameras.main.setBounds(0, 0, width, height);
    this.add.image(width / 2, height / 2, 'sky').setDisplaySize(width, height).setDepth(-100);
    this.add.image(width / 2, height / 2, 'mountains').setDisplaySize(width, height).setDepth(-90);
    this.add.image(width / 2, height / 2, 'clouds').setDisplaySize(width, height).setAlpha(.36).setDepth(-80);
    this.renderConnections();
    for (const island of this.mapData.world.islands) this.renderIsland(island);
    this.add.image(800, 420, `dragon:${this.mapData.dragon.skin ?? 'red'}`)
      .setDisplaySize(115, 115).setDepth(50);
    this.renderIslandStages(this.selectedIslandId);
    this.renderHUD();
    this.events.on('update-world-map', this.updateFromBackend, this);
  }

  renderConnections() {
    const byId = Object.fromEntries(this.mapData.world.islands.map(i => [i.id, i]));
    const g = this.add.graphics().setDepth(2);
    for (const [start, end] of this.mapData.world.connections ?? []) {
      const a = byId[start], b = byId[end];
      if (!a || !b) continue;
      const midX = (a.x + b.x) / 2;
      g.lineStyle(17, 0x96724d, 1);
      g.beginPath(); g.moveTo(a.x, a.y); g.lineTo(midX, (a.y + b.y) / 2 + 18); g.lineTo(b.x, b.y); g.strokePath();
      g.lineStyle(7, 0xffdf98, .95);
      g.beginPath(); g.moveTo(a.x, a.y); g.lineTo(midX, (a.y + b.y) / 2 + 18); g.lineTo(b.x, b.y); g.strokePath();
    }
  }

  renderIsland(island) {
    const active = island.id === this.selectedIslandId;
    const s = this.add.image(island.x, island.y, `island:${island.theme}`)
      .setDisplaySize(active ? 340 : 310, active ? 226 : 206).setDepth(10 + Math.round(island.y / 100));
    if (!island.unlocked) s.setTint(0x99acc1);
    const hit = this.add.zone(island.x, island.y - 26, 235, 105)
      .setDepth(60).setInteractive({ useHandCursor: true });
    hit.on('pointerdown', () => {
      this.selectedIslandId = island.id;
      this.renderIslandStages(island.id);
      this.game.events.emit('dragon:island-selected', { islandId: island.id });
    });
    const title = this.add.text(island.x, island.y + 117, island.name, {
      fontFamily: 'Arial, sans-serif', fontSize: '19px', fontStyle: 'bold',
      color: '#ffffff', stroke: '#405a73', strokeThickness: 5,
    }).setOrigin(.5).setDepth(55);
    this.dynamicObjects.push(title);
  }

  renderIslandStages(islandId) {
    for (const o of this.stageObjects ?? []) o.destroy();
    this.stageObjects = [];
    const island = this.mapData.world.islands.find(i => i.id === islandId);
    if (!island) return;
    const offsets = [[-91, -43], [-30, -77], [39, -75], [101, -36]];
    island.stages.forEach((stage, index) => {
      const [ox, oy] = offsets[index];
      const x = island.x + ox, y = island.y + oy;
      const state = !island.unlocked ? 'locked' : stage.state;
      const skillImg = this.add.image(x, y, `skill:${stage.skill}`).setDisplaySize(68, 68).setDepth(100);
      if (state === 'locked') skillImg.setTint(0x777f94);
      const ring = this.add.image(x, y, `state:${state}`).setDisplaySize(76, 76)
        .setAlpha(state === 'locked' ? .8 : .5).setDepth(99);
      this.stageObjects.push(skillImg, ring);
      const stars = this.add.text(x, y + 38, '★'.repeat(stage.stars) + '☆'.repeat(3 - stage.stars), {
        fontFamily: 'Arial', fontSize: '17px', color: '#ffe25e',
        stroke: '#775847', strokeThickness: 3,
      }).setOrigin(.5).setDepth(101);
      this.stageObjects.push(stars);
      skillImg.setInteractive({ useHandCursor: state !== 'locked' });
      skillImg.on('pointerdown', () => {
        if (state === 'locked') return;
        this.game.events.emit('dragon:stage-selected', {
          childId: this.mapData.childId, islandId, stageId: stage.stageId,
          skill: stage.skill,
        });
      });
    });
  }

  renderHUD() {
    const card = this.add.image(215, 70, 'panel').setDisplaySize(360, 112).setScrollFactor(0).setDepth(400);
    this.add.text(85, 48, `${this.mapData.dragon.name} · Lv.${this.mapData.dragon.evolutionLevel}`, {
      fontFamily: 'Arial', fontSize: '19px', fontStyle: 'bold', color: '#674932',
    }).setScrollFactor(0).setDepth(401);
    this.expBar = this.add.image(205, 92, 'progressFill').setOrigin(0, .5)
      .setDisplaySize(240, 20).setScrollFactor(0).setDepth(402);
    this.setExp(this.mapData.dragon.exp, this.mapData.dragon.expToNextLevel);
    this.add.text(1190, 29, `⭐ ${this.mapData.progress.totalStars}/${this.mapData.progress.maxStars}`, {
      fontFamily: 'Arial', fontSize: '26px', color: '#fff8bb',
      stroke: '#4e6180', strokeThickness: 4,
    }).setScrollFactor(0).setDepth(402);
  }

  setExp(exp, maxExp) {
    // Use crop, NOT stretching the PNG's borders. Progress is data-driven.
    const p = Phaser.Math.Clamp(exp / Math.max(1, maxExp), 0, 1);
    this.expBar.setDisplaySize(240, 20);
    this.expBar.setCrop(0, 0, Math.round(this.expBar.width * p), this.expBar.height);
  }

  updateFromBackend(next) {
    // Example for polling or a new API response. For full changes, reload the scene.
    this.mapData = next;
    this.scene.restart();
  }
}
