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
    Core/                    # GameManager, InputManager, ScoreManager, Interfaces
    Player/                  # PlayerController, PlayerCombat, PlayerHealth, PlayerAnimator
    Enemies/                 # Base enemy AI with state machine
      EnemyBase.cs           # MonoBehaviour, implements IDamageable, drives state transitions
      EnemyStateMachine.cs   # Generic state machine (ChangeState<T>)
      IEnemyState.cs         # Interface: Enter(), Execute(), Exit()
      States/
        IdleState.cs         # Stands still 1-3 s, transitions to Patrol
        PatrolState.cs       # Moves between waypoints, transitions to Chase
        ChaseState.cs        # Follows player, red ! indicator on enter
        AttackState.cs       # Applies IDamageable damage, returns to Chase
        StunnedState.cs      # 0.5 s stun on hit, returns to Chase
        DefeatedState.cs     # Defeat animation, destroys GameObject after 1 s
    UI/                      # HUDController
    Collectibles/            # Collectible, Coin, RobotPart
    Costumes/
    PowerUps/
    Levels/                  # LevelManager, CameraFollower, ParallaxBackground
    Persistence/
  Prefabs/
  Scenes/          # MainMenu.unity and future levels
  Art/
    Sprites/
    Animations/
  Audio/
    SFX/
    Music/
  ScriptableObjects/
```

---

## Build Targets

The project is configured for **WebGL**, **iOS**, and **Android** in `ProjectSettings/ProjectSettings.asset`.

---

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for coding standards (500-line cap per script), naming conventions, and PR guidelines.
