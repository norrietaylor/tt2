# Unity Architecture Guidelines

## Persistent Singleton Pattern

Manager-level systems that must persist across scene loads use the persistent singleton pattern. The pattern is applied in `Awake()` using a static `Instance` property.

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

Singletons are used only for true global systems: `GameManager`, `InputManager`, `ScoreManager`. Domain-specific logic (player health, enemy behavior) uses regular MonoBehaviour components attached to scene objects, not singletons.

## Interface-Driven Design

Shared behaviors are expressed through interfaces defined in `TaekwondoTech.Core`. This decouples consumers from concrete implementations.

| Interface | Purpose | Implementors |
|---|---|---|
| `IDamageable` | Entities that take and absorb damage | `PlayerHealth`, `EnemyBase` |
| `ICollectible` | Items the player can pick up | `Coin`, `RobotPart`, `Collectible` |
| `IInteractable` | Objects the player can interact with | Switches, doors, NPCs |
| `IPowerUp` | Timed effects applied to the player | Speed boost, shield, elemental attack |

Code that deals damage calls `IDamageable.TakeDamage(float)` rather than referencing `PlayerHealth` or `EnemyBase` directly. This allows new damageable types to be added without modifying combat code.

## Enemy State Machine

Enemy AI uses a simple state machine implemented in `EnemyStateMachine`. Each state implements `IEnemyState` and receives a reference to its `EnemyBase` owner.

```text
Idle → Patrol → Chase → Attack
               ↑           ↓
               └── Stunned ←
                      ↓
                  Defeated
```

State transitions are triggered by conditions evaluated in `Execute()`. Entering a new state calls `Exit()` on the current state before calling `Enter()` on the new one.

```csharp
public void ChangeState(IEnemyState newState)
{
    _currentState?.Exit();
    _currentState = newState;
    _currentState?.Enter();
}
```

New enemy types extend `EnemyBase` and configure behavior by overriding `[SerializeField]` parameters (`_moveSpeed`, `_detectionRadius`, `_attackRange`) rather than subclassing states.

## MonoBehaviour Lifecycle

Scripts follow a consistent lifecycle ordering:

- `Awake()` — component reference lookup (`GetComponent<T>()`), state initialization, singleton setup.
- `Start()` — cross-component wiring, initial state transitions that depend on other `Awake()` calls completing.
- `Update()` — input polling, state machine ticks, per-frame logic.
- `FixedUpdate()` — physics (`Rigidbody2D` velocity assignment).
- `OnDrawGizmosSelected()` — editor debug visualizations (detection radii, waypoints).

`GetComponent<T>()` is called in `Awake()` and cached in a private field. It is never called in `Update()` or `FixedUpdate()`.

## RequireComponent

Components that depend on specific Unity components declare this with `[RequireComponent]`. This enforces dependencies at edit time and prevents runtime null-reference errors.

```csharp
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyBase : MonoBehaviour, IDamageable { ... }
```

## Unity Events for Decoupled Signaling

`UnityEvent` fields (decorated with `[Header("Events")]`) allow scene-level wiring of reactions to game events without code coupling. For example, `EnemyBase.OnEnemyDefeated` can trigger a sound effect, a particle effect, or a score increment entirely from the Inspector.

```csharp
[Header("Events")]
public UnityEvent OnEnemyDefeated;
public UnityEvent OnEnemyDamaged;
```

New components raise events for outcomes that other systems may need to react to, rather than calling those systems directly.

## ScriptableObjects

Data that varies per asset type (enemy stats, collectible properties, power-up durations) is extracted into ScriptableObjects rather than hardcoded in script. This allows designers to tune values without code changes and supports asset-level reuse across prefabs.
