# Taekwondo Tech v2

A ground-up Unity (C#) reimagining of the Taekwondo Robot Builder side-scrolling platformer — targeting kids ages 6–12 on WebGL, iOS, and Android.

---

## 📋 Product Requirements

The full Product Requirements Document (PRD) is the source of truth for all features and design decisions.

**[📄 View the PRD → docs/prd.md](docs/prd.md)**

| Field | Value |
|-------|-------|
| Status | ✅ Approved |
| Version | 1.1 |
| Last Updated | 2026-02-27 |

---

## Getting Started

### Prerequisites

- [Unity Hub](https://unity.com/download)
- **Unity 2022 LTS** (2022.3.x recommended)

### Setup

1. Clone this repository.
2. Open **Unity Hub** → **Add project from disk** → select the repo root.
3. Unity will resolve packages from `Packages/manifest.json` on first open (may take a few minutes).
4. Open the `MainMenu` scene: `Assets/Scenes/MainMenu.unity`.

---

## Project Structure

```
Assets/
  Scripts/
    Core/          # GameManager, InputManager, ScoreManager, Interfaces (IDamageable)
    Player/        # PlayerController, PlayerCombat, PlayerHealth, PlayerAnimator
    Enemies/
    UI/            # HUDController
    Collectibles/  # Collectible (base), Coin, RobotPart
    Costumes/
    PowerUps/
    Levels/        # LevelManager, CameraFollower, ParallaxBackground
    Persistence/
  Prefabs/
  Scenes/          # MainMenu.unity and future levels
  Art/
    Sprites/
    Animations/    # PlayerAnimator.controller
  Audio/
    SFX/
    Music/
  ScriptableObjects/
```

---

## Implemented Systems

| System | Scripts | Description |
|--------|---------|-------------|
| Player Movement | `PlayerController` | 2D platformer movement, jumping, grounding |
| Player Combat | `PlayerCombat` | Punch (≈1 unit), kick (≈1.5 units), head stomp with bounce |
| Player Health | `PlayerHealth` | 3-hit system with 1 s invincibility frames and sprite flash |
| Player Animation | `PlayerAnimator` | Drives Animator parameters from controller/combat state |
| Collectibles | `Collectible`, `Coin`, `RobotPart` | Trigger-based pickup with visual effect; coins update score |
| Score | `ScoreManager` | Singleton; `AddScore(int)` raises `OnScoreChanged(int)` |
| HUD | `HUDController` | Health hearts and score display via TMP_Text |
| Input | `InputManager` | Centralised input event broker |
| Levels | `LevelManager`, `CameraFollower`, `ParallaxBackground` | Scene flow and camera tracking |

---

## Build Targets

The project is configured for **WebGL**, **iOS**, and **Android** in `ProjectSettings/ProjectSettings.asset`.

---

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for coding standards (500-line cap per script), naming conventions, and PR guidelines.
