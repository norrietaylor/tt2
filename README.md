# Taekwondo Tech v2

A ground-up Unity (C#) reimagining of the Taekwondo Robot Builder side-scrolling platformer — targeting kids ages 6–12 on WebGL, iOS, and Android.

See the full product specification in [`docs/prd.md`](docs/prd.md).

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
    Core/          # GameManager, SceneLoader, etc.
    Player/        # PlayerController, PlayerCombat
    Enemies/
    UI/
    Collectibles/
    Costumes/
    PowerUps/
    Levels/
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
