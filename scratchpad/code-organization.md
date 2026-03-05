# Code Organization Patterns

## Namespace Conventions

All C# scripts use the `TaekwondoTech` root namespace or a domain-specific child namespace. The namespace mirrors the directory path under `Unity/Assets/Scripts/`.

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

Every new script declares its namespace as the first non-using statement. Cross-domain dependencies are expressed through `using` directives referencing the target namespace (e.g., `using TaekwondoTech.Core`).

## Naming Conventions

| Element | Convention | Example |
|---|---|---|
| Classes / Structs / Enums | PascalCase | `GameManager`, `EnemyType` |
| Methods | PascalCase | `LoadScene()`, `TakeDamage()` |
| Public properties | PascalCase | `Health`, `IsAlive` |
| Private / protected fields | camelCase with `_` prefix | `_health`, `_isGrounded` |
| Constants | UPPER_SNAKE_CASE | `MAX_HEALTH`, `JUMP_FORCE` |
| Interfaces | `I` + PascalCase | `IDamageable`, `ICollectible` |
| Unity Events | PascalCase | `OnPlayerDeath`, `OnEnemyDefeated` |

## Directory and File Structure

Scripts are organized by gameplay domain, not by type. A `PlayerHealth` script lives in `Player/`, not in a generic `Health/` folder. Each domain directory contains only the scripts that directly implement that domain's behavior.

```
Unity/Assets/Scripts/
├── Core/           # GameManager, InputManager, ScoreManager, Interfaces
├── Player/         # PlayerController, PlayerCombat, PlayerHealth, PlayerAnimator
├── Enemies/        # EnemyBase, EnemyStateMachine, IEnemyState
│   └── States/     # IdleState, PatrolState, ChaseState, AttackState, StunnedState, DefeatedState
├── Collectibles/   # Collectible (base), Coin, RobotPart
├── UI/             # HUDController
├── Levels/         # LevelManager, CameraFollower, ParallaxBackground
├── Persistence/    # SaveSystem, PlayerPrefsHelper
├── Costumes/
├── PowerUps/
└── Input/
```

## 500-Line Cap

No single C# script may exceed 500 lines. When a class approaches this limit, responsibilities are split into separate classes or extracted into ScriptableObjects. This limit is enforced by CI and code review.

## Meta Files

Every `.cs` file and every folder under `Unity/Assets/Scripts/` requires a companion `.meta` file with a unique 32-character lowercase hex GUID. Unity uses these GUIDs to track asset references. Missing `.meta` files cause build failures and broken scene references.

## Formatting

- 4 spaces per indent level; no tabs.
- UTF-8 encoding with LF line endings.
- One blank line between methods; no trailing whitespace.
- `[Header("...")]` attributes group related `[SerializeField]` fields in the Inspector.
