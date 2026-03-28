# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

---

## Project Overview

**Taekwondo Tech v2** is a Unity 2022.3.20f1 (LTS) side-scrolling platformer (C#) targeting WebGL, iOS, and Android. The game features a taekwondo martial artist collecting robot parts, combat mechanics, costumes, and power-ups.

- Unity project root: `Unity/` (not repo root)
- Product requirements: `docs/prd.md` (REQ-001 through REQ-012)

---

## Build & Test Commands

There is no CLI build system. All building and testing happens through Unity Editor or CI.

**Running tests locally:** Unity Editor → Window → General → Test Runner → EditMode tab

Tests use Unity Test Framework (NUnit) at `Unity/Assets/Tests/EditMode/`. The test assembly (`TT2.Tests.EditMode`) uses namespace `TaekwondoTech.Tests.EditMode` and references `UnityEngine.TestRunner` and `UnityEditor.TestRunner`.

**CI builds** run automatically on pushes to `main` and on PRs from in-repo branches (not forks — secrets aren't exposed). Builds WebGL, iOS, Android in parallel via `game-ci/unity-builder` with `projectPath: Unity`.

---

## Architecture

### Key Patterns

1. **Singleton managers** — `GameManager`, `InputManager`, `ScoreManager` use `DontDestroyOnLoad()` with duplicate-prevention in `Awake()`. Access globally via static `Instance` property.

2. **Interface-driven design** — Core interfaces in `Unity/Assets/Scripts/Core/Interfaces.cs`:
   - `IDamageable` — Health/damage system (Player, Enemies). Properties: `Health`, `MaxHealth`, `IsAlive`. Methods: `TakeDamage(float)`, `TakeDamage(float, GameObject)`, `Heal(float)`.
   - `ICollectible` — Pickup items. `OnCollect(GameObject)`, `CollectibleType`, `Rarity`.
   - `IInteractable` — Interactive objects. `Interact(GameObject)`, `CanInteract`, `InteractionPrompt`.
   - `IPowerUp` — Power-ups. `Activate(GameObject)`, `Deactivate(GameObject)`, `PowerUpType`, `Duration`, `IsActive`.

3. **State machine** — Enemy AI uses `EnemyStateMachine` with `IEnemyState` interface (Enter/Execute/Exit). States: Idle, Patrol, Chase, Attack, Stunned, Defeated (in `Enemies/States/`).

4. **UnityEvent communication** — Loose coupling via events like `OnHealthChanged`, `OnPlayerDeath`, `OnEnemyDefeated`, `OnPlayerDamaged`.

### Player decomposition

Player logic is split across: `PlayerController` (movement/jumping), `PlayerCombat` (attacks), `PlayerHealth` (3-hit HP, invincibility frames), `PlayerAnimator` (animation states). This is the pattern to follow when adding complex entities.

---

## Mandatory Rules

### All C# scripts go in `Unity/Assets/Scripts/<domain>/`

Never place scripts outside the `Unity/` directory.

### Unity `.meta` files for every new asset

Every new file and folder needs a `.meta` file with a unique 32-char lowercase hex GUID. Without them, Unity can't track assets — builds break.

**C# script `.meta` template:**
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

**Folder `.meta` template** (lives next to the folder, not inside it):
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

Never reuse a GUID that already appears in another `.meta` file.

### Namespaces (mandatory)

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

### 500-line cap (CI enforced)

No single C# script may exceed 500 lines. Split into multiple classes or ScriptableObjects.

### Formatting

- 4 spaces (no tabs), UTF-8, LF line endings, no trailing whitespace
- One blank line between methods

### Naming conventions

| Element | Convention | Example |
|---|---|---|
| Classes / Structs / Enums | PascalCase | `GameManager`, `EnemyType` |
| Methods | PascalCase | `LoadScene()` |
| Public Properties | PascalCase | `Health`, `IsAlive` |
| Private/protected fields | `_` prefix camelCase | `_health`, `_isGrounded` |
| Constants | UPPER_SNAKE_CASE | `MAX_HEALTH` |
| Interfaces | `I` + PascalCase | `IDamageable` |

---

## Branch & PR Conventions

- Branch naming: `feature/<desc>`, `fix/<desc>`, `chore/<desc>`
- PR titles: imperative mood — "Add X" not "Added X"
- One approving review required before merge

---

## Pre-Commit Checklist

- [ ] All new `.cs` files are in `Unity/Assets/Scripts/<domain>/`
- [ ] Every new `.cs` file has a `.cs.meta` with unique GUID
- [ ] Every new folder has a `.meta` file in its parent directory
- [ ] All scripts use correct `TaekwondoTech.*` namespace
- [ ] No file exceeds 500 lines
- [ ] 4-space indent, LF line endings
- [ ] Any `IDamageable` implementation includes all required members
