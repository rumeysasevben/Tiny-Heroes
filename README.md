# 🗡️ Tiny Heroes

A fast-paced 2D survival roguelite inspired by **Vampire Survivors**, built solo in **Unity 6** in 1 week as a portfolio project.

**🎮 [Play it in your browser on itch.io →](https://rumeysasevben.itch.io/tiny-heroes)**

---

## 📸 Screenshots

<!-- Screenshot'ları repo'ya yükleyince path'leri güncelle -->
<p align="center">
  <img src="screenshots/gameplay-1.png" width="45%" />
  <img src="screenshots/gameplay-2.png" width="45%" />
</p>
<p align="center">
  <img src="screenshots/levelup.png" width="45%" />
  <img src="screenshots/boss.png" width="45%" />
</p>

---

## 🎯 About

Play as a tiny hero swarmed by endless waves of enemies. Your weapons fire automatically — your only job is to **move, dodge, and choose the right upgrades**. Every level-up offers three random powers; stack them, combine them, break the game.

### Controls
- **WASD / Arrow Keys** — Move
- **ESC** — Pause
- Weapons fire automatically

---

## ⚔️ Features

**Combat & Progression**
- 2 weapon systems: **Boomerang** (5 levels) and **Lightning Orbs** (unlocks at level 20)
- Crit system with golden damage popups
- ScriptableObject-driven upgrade system with **soft cap** diminishing returns (×0.9 decay after 10 stacks)
- Auto-fire weapon logic

**Enemies & Waves**
- 10-wave WaveManager with banner UI
- 6 enemy types: Fast, Mid, Tanky, Ghost, Swarm, and a telegraphed **Charger Boss**
- Time-based exponential difficulty scaling
- Spawn telegraphs and enemy shadows

**Power-ups & Pickups**
- 4 ChanceBox potions: Full Heal, Screen Clear, Big XP, Damage Boost (each with colored glow)
- XP Gems with TrailRenderer
- Heart pickups for healing

**Game Feel / Juice**
- Hit Flash, Camera Shake, Hit Pause (50ms on crits)
- Death particles, damage number colors (red = taken, blue = dealt, gold = crit)
- Low-HP vignette pulse
- LevelUp effect and UI animations

**UI & Systems**
- HP/XP bars with numerical display
- Pause Menu, Game Over screen with stats, StatsPanel
- Main Menu with Best Time tracking
- Sound Toggle, persistent AudioManager (`DontDestroyOnLoad`)

---

## 🛠️ Tech Stack

- **Engine:** Unity 6 (URP, 2D)
- **Language:** C#
- **Art:** [Kenney Tiny Dungeon](https://kenney.nl/assets/tiny-dungeon) (CC0)
- **Platform:** WebGL (browser-playable)

### Architecture Highlights
- **Object Pooling** with disabled-holder initialization and name-based runtime lookup
- **Event-driven Health** system (generic, reusable across player/enemies)
- **ScriptableObject** upgrade data for designer-friendly tuning
- **Singleton manager** pattern (GameManager, AudioManager, WaveManager, UpgradeManager)
- Soft-cap stat system with per-stack tracking via `GetEffectiveValue`

---

## 🚀 Build & Run

### Play Online
[https://rumeysasevben.itch.io/tiny-heroes](https://rumeysasevben.itch.io/tiny-heroes)

### Run Locally
1. Clone the repo:
   ```bash
   git clone https://github.com/rumeysasevben/Tiny-Heroes.git
   ```
2. Open the project in **Unity 6 (6000.x)**
3. Open the `MainMenu` scene under `Assets/Scenes/`
4. Press Play

---

## 📌 Development Notes

Built from scratch in **1 week (6-13 June 2026)** as a solo project. The goal was to ship a fully featured, juicy, polished roguelite — not just a prototype — and use it as a portfolio piece demonstrating end-to-end Unity game development: architecture, systems design, game feel, UI/UX, and deployment.

---

## 📫 Contact

**Rumeysa Sevben**
- GitHub: [@rumeysasevben](https://github.com/rumeysasevben)
- itch.io: [rumeysasevben.itch.io](https://rumeysasevben.itch.io)

---

*Made with ❤️ in Unity 6. Feedback welcome!*
