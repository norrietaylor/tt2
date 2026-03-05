# CLAUDE.md — Taekwondo Tech v2

This file provides essential context for AI agents (Claude, Copilot, etc.) working on this repository. **Read this before making any changes.**

---

## Project Overview

**Taekwondo Tech v2** is a Unity 2022 LTS side-scrolling platformer (C#) targeting WebGL, iOS, and Android. The game features a taekwondo martial artist collecting robot parts, combat mechanics, costumes, and power-ups.

---

## Repository Structure

```
tt2/
├── CLAUDE.md          ← You are here
├── CONTRIBUTING.md    ← Coding standards, PR guidelines, CI/CD info
├── Unity/             ← THE UNITY PROJECT (all game code goes here)
│   ├── Assets/
│   │   ├── Scripts/   ← ALL C# scripts go here, organized by domain
│   │   │   ├── Core/         (GameManager, SceneLoader, Interfaces)
│   │   │   ├── Player/       (PlayerController, PlayerCombat, PlayerHealth)
│   │   │   ├── Enemies/      (EnemyBase, EnemyStateMachine, states)
│   │   │   ├── Collectibles/ (Coin, RobotPart, Collectible base)
│   │   │   ├── UI/           (HUDController)
│   │   │   ├── Levels/       (LevelManager, CameraFollower, ParallaxBackground)
│   │   │   ├── Persistence/  (SaveSystem, PlayerPrefsHelper)
│   │   │   ├── Costumes/
│   │   │   ├── PowerUps/
│   │   │   └── Input/
│   │   ├── Prefabs/
│   │   ├── Scenes/
│   │   ├── Art/
│   │   ├── Audio/
│   │   └── ScriptableObjects/
│   ├── Packages/
│   └── ProjectSettings/
└── docs/
    └── prd.md         ← Product requirements document
```

**CRITICAL**: All C# scripts must be placed inside `Unity/Assets/Scripts/<domain>/`. Never place scripts outside the `Unity/` directory.

---

## Unity `.meta` Files — REQUIRED for Every New Asset

Unity requires a `.meta` file alongside **every** file and folder you create. Without `.meta` files, Unity cannot track assets by GUID, causing build failures and broken scene references.

### When you create a new C# script file:

**Create `Unity/Assets/Scripts/<Domain>/MyNewScript.cs` AND `Unity/Assets/Scripts/<Domain>/MyNewScript.cs.meta`**

Template for a C# script `.meta` file:
```yaml
fileFormatVersion: 2
guid: <unique-32-hex-char-guid>
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData:
  assetBundleName:
  assetBundleVariant:
```

### When you create a new folder:

**Create `Unity/Assets/Scripts/NewFolder/` AND `Unity/Assets/Scripts/NewFolder.meta`** (the meta file lives next to the folder, not inside it)

Template for a folder `.meta` file:
```yaml
fileFormatVersion: 2
guid: <unique-32-hex-char-guid>
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData:
  assetBundleName:
  assetBundleVariant:
```

### Generating GUIDs

Each `.meta` file needs a unique 32-character lowercase hex GUID. You can derive one deterministically from the file path. Example:
- `Core/GameManager.cs` → `guid: c0a0e1d2f3b4a5c6d7e8f9a0b1c2d3e4`
- `Enemies/EnemyBase.cs` → `guid: e0e1b2a3f4c5d6e7f8a9b0c1d2e3f4a5`

**Never reuse a GUID** that already appears in another `.meta` file in the project.

---

## Coding Standards

### Namespaces (MANDATORY)

All scripts must use the `TaekwondoTech` namespace or a child namespace:

| Directory | Namespace |
|---|---|
| `Core/` | `TaekwondoTech.Core` |
| `Player/` | `TaekwondoTech.Player` |
| `Enemies/` | `TaekwondoTech.Enemies` |
| `Enemies/States/` | `TaekwondoTech.Enemies.States` |
| `Collectibles/` | `TaekwondoTech.Collectibles` |
| `UI/` | `TaekwondoTech.UI` |
| `Levels/` | `TaekwondoTech.Levels` |
| `Persistence/` | `TaekwondoTech.Persistence` |
| `Costumes/` | `TaekwondoTech.Costumes` |
| `PowerUps/` | `TaekwondoTech.PowerUps` |

### 500-Line Cap (MANDATORY — CI Enforced)

**No single C# script may exceed 500 lines.** If a script approaches this limit, split responsibilities into multiple classes or use ScriptableObjects.

### Formatting

- **4 spaces** per indent level (no tabs)
- UTF-8 encoding, **LF** line endings
- One blank line between methods
- No trailing whitespace

### Naming Conventions

| Element | Convention | Example |
|---|---|---|
| Classes / Structs / Enums | PascalCase | `GameManager`, `EnemyType` |
| Methods | PascalCase | `LoadScene()`, `TakeDamage()` |
| Public Properties | PascalCase | `Health`, `IsAlive` |
| Private/protected fields | camelCase with `_` prefix | `_health`, `_isGrounded` |
| Constants | UPPER_SNAKE_CASE | `MAX_HEALTH`, `JUMP_FORCE` |
| Interfaces | `I` + PascalCase | `IDamageable`, `ICollectible` |
| Unity Events | PascalCase | `OnPlayerDeath` |

---

## Key Interfaces (in `TaekwondoTech.Core`)

These are defined in `Unity/Assets/Scripts/Core/Interfaces.cs`:

- **`IDamageable`** — Entities that take damage (Player, Enemies). Has `Health`, `MaxHealth`, `IsAlive`, `TakeDamage(float)`, `TakeDamage(float, GameObject)`, `Heal(float)`.
- **`ICollectible`** — Items the player can pick up. Has `OnCollect(GameObject)`, `CollectibleType`, `Rarity`.
- **`IInteractable`** — Objects the player can interact with. Has `Interact(GameObject)`, `CanInteract`, `InteractionPrompt`.
- **`IPowerUp`** — Power-up items. Has `PowerUpType`, `Duration`, `Activate(GameObject)`, `Deactivate(GameObject)`, `IsActive`.

When adding new damageable entities, implement `IDamageable` from `TaekwondoTech.Core`.

---

## CI/CD Pipeline

The Unity build workflow (`.github/workflows/unity-build.yml`) builds for **WebGL**, **iOS**, and **Android** on every PR. It requires:
- `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD` secrets (already configured)
- Unity project located at `Unity/` (i.e., `projectPath: Unity`)

**Builds will fail if:**
- C# scripts have compilation errors
- A script exceeds 500 lines
- New assets are missing `.meta` files (Unity cannot track them by GUID)
- Scripts use wrong or missing namespaces

---

## Checklist for Agent-Written PRs

Before committing any code changes, verify:

- [ ] All new C# scripts are in `Unity/Assets/Scripts/<domain>/`
- [ ] Every new `.cs` file has a corresponding `.cs.meta` file with a unique GUID
- [ ] Every new folder has a corresponding `.meta` file (in the parent directory)
- [ ] All scripts use the correct `TaekwondoTech.*` namespace
- [ ] No new file exceeds 500 lines
- [ ] 4-space indentation, LF line endings
- [ ] New classes follow the naming conventions above
- [ ] Any new `IDamageable` implementation includes all required interface members

---

## Product Requirements

See `docs/prd.md` for the full product requirements document, including the game's functional requirements (REQ-001 through REQ-012), user stories, and implementation roadmap.
