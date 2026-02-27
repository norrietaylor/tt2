# Contributing to Taekwondo Tech v2

Thank you for your interest in contributing to the Taekwondo Tech v2 Unity project! Please read this guide before submitting a pull request.

---

## Table of Contents

1. [Getting Started](#getting-started)
2. [Project Structure](#project-structure)
3. [Coding Standards](#coding-standards)
4. [Naming Conventions](#naming-conventions)
5. [Pull Request Guidelines](#pull-request-guidelines)

---

## Getting Started

1. Install **Unity 2022 LTS** (2022.3.x) via [Unity Hub](https://unity.com/download).
2. Clone the repository and open the project in Unity Hub by pointing it at the repo root.
3. The first open will download packages from `Packages/manifest.json` — this may take a few minutes.
4. Open the `MainMenu` scene at `Assets/Scenes/MainMenu.unity` to verify the project loads correctly.

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
    Persistence/   # SaveSystem, PlayerPrefsHelper
  Prefabs/
  Scenes/
  Art/
    Sprites/
    Animations/
  Audio/
    SFX/
    Music/
  ScriptableObjects/
```

---

## Coding Standards

### 500-Line Cap per Script

> **No single C# script file may exceed 500 lines.**

If a file approaches this limit, extract responsibilities into a new class or leverage ScriptableObjects. This rule exists to keep each file focused and reviewable. This guideline is enforced through CI checks and code review rather than EditorConfig settings.

### Formatting

- Indent with **4 spaces** (no tabs).
- UTF-8 encoding, LF line endings.
- One blank line between methods; no trailing whitespace.

### Namespaces

All scripts must live under the `TaekwondoTech` namespace or a child namespace (e.g., `TaekwondoTech.Core`, `TaekwondoTech.Player`).

---

## Naming Conventions

| Element | Convention | Example |
|---|---|---|
| Classes / Structs / Enums | PascalCase | `GameManager`, `EnemyType` |
| Methods | PascalCase | `LoadScene()`, `TakeDamage()` |
| Public Properties | PascalCase | `Health`, `IsAlive` |
| Private / protected fields | camelCase with `_` prefix | `_health`, `_isGrounded` |
| Constants | UPPER_SNAKE_CASE | `MAX_HEALTH`, `JUMP_FORCE` |
| Interfaces | `I` + PascalCase | `IDamageable`, `ICollectible` |
| Unity Events | PascalCase (no prefix) | `OnPlayerDeath` |

---

## Pull Request Guidelines

1. **Branch naming:** `feature/<short-description>`, `fix/<short-description>`, or `chore/<short-description>`.
2. **PR title:** Use imperative mood — _"Add PlayerController jump mechanic"_ not _"Added jump"_.
3. **Description:** Include a brief summary, testing steps, and screenshots/GIFs for any visual change.
4. **Size:** Keep PRs focused; avoid bundling unrelated changes.
5. **Tests:** Add or update Unity Test Runner tests for any new logic in `Assets/Scripts`.
6. **No 500-line violations:** CI will flag scripts that exceed 500 lines — fix them before requesting review.
7. **Review:** At least one approving review is required before merging.
