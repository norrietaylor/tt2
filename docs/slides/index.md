---
marp: true
theme: default
paginate: true
backgroundColor: #1a1a2e
color: #e0e0e0
style: |
  section {
    font-family: 'Segoe UI', Arial, sans-serif;
  }
  h1 {
    color: #f4a261;
  }
  h2 {
    color: #e76f51;
  }
  strong {
    color: #f4a261;
  }
  a {
    color: #90caf9;
  }
---

# Taekwondo Tech v2

### A Kid-Friendly Martial Arts Platformer

Built in **Unity (C#)** · Ages 6–12 · WebGL · iOS · Android

---

## What is Taekwondo Tech v2?

A **ground-up reimagining** of a side-scrolling platformer where a young taekwondo martial artist:

- Fights through colorful, story-driven levels
- Collects robot parts to build a unique robot companion
- Unlocks 10 dragon costumes with elemental powers
- Defeats enemies with punches, kicks, and special moves

**v1 validated the concept** across 15+ PRs. v2 takes it to production quality.

---

## Why Unity?

| v1 (Phaser.js) | v2 (Unity) |
|----------------|------------|
| 100K+ lines in 2 files | Modular, 500-line cap per script |
| No audio | Full SFX + music |
| Geometric graphics | Hand-crafted sprite art |
| iOS hacks everywhere | Native cross-platform |
| Browser only | WebGL + iOS + Android |

---

## Core Features

- **Platforming** — run, jump, double-jump, punch, kick, stomp
- **Robot Building** — collect parts (Head, Body, Arms, Legs, Power Core)
- **Dragon Costumes** — 10 unique elemental costumes + Legendary fusion form
- **Power-Up Queue** — stackable power-ups with a visual queue system
- **Banana Modes** — bonus collection and survival challenge modes
- **Narrative Progression** — story-driven levels with clear goals

---

## Dragon Costume System

10 elemental dragon costumes, each with:

- Unique visual design and animated wings
- Distinct elemental projectile attack
- Special move with audio + visual effects
- Unlock conditions tied to level progression

Plus an **11th Legendary Mode** — unlocked by collecting all robot part types.

---

## Platform Targets

- **WebGL** — playable in browser, sharable link
- **iOS App Store** — single Unity codebase, no iOS-specific hacks
- **Google Play Store** — touch controls optimized for mobile

**Target audience:** Kids ages 6–12

**Session goal:** 10+ minutes average playtime

---

## Architecture

```
Assets/
  Scripts/
    Core/        # GameManager, SceneLoader
    Player/      # PlayerController, PlayerCombat
    Enemies/     # Enemy AI and behaviors
    UI/          # HUD, menus, inventory
    Collectibles/ # Robot parts, power-ups
    Costumes/    # Dragon costume system
    PowerUps/    # Power-up queue
    Persistence/ # Save/load system
```

**Rule:** No single script file exceeds 500 lines.

---

## Success Metrics

| Goal | Metric | Target |
|------|--------|--------|
| Engagement | Game completion rate | 40% finish 3+ levels |
| Cross-platform | Store approvals | Live on all 3 platforms |
| Session length | Avg session duration | 10+ minutes |
| Code quality | Max file size | 500 lines per script |

---

## Development Status

**Status:** In active development

- PRD v1.1 approved (Feb 27, 2026)
- Unity project structure established
- Core systems being built in priority order:
  1. Core platforming (movement, combat, physics)
  2. Robot part collection & building
  3. Dragon costume system
  4. Audio, narrative, level design
  5. Cross-platform deployment

---

## Key Design Principles

- **Kid-first UX** — controls work on keyboard, gamepad, and touchscreen equally well
- **Audio everywhere** — every action has a sound effect
- **Visual clarity** — rarity shown by color, unlocks have clear conditions
- **Persistence** — all progress saves between sessions
- **Performance** — 60 FPS target on all platforms

---

## Contributing

See [CONTRIBUTING.md](../../CONTRIBUTING.md) for:

- Coding standards (500-line cap)
- Naming conventions
- PR guidelines

**Tech stack:** Unity 2022 LTS · C# · Unity Analytics · Unity Physics

[View the full PRD →](../prd.md)
