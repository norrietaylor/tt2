# Unity Architecture Guidelines

**Project:** Taekwondo Tech v2
**Status:** Living Document
**Last Updated:** 2026-03-04

---

## Singleton Pattern

Persistent game systems (those that survive scene loads) use a standard singleton pattern:

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

### Rules

- `Awake()` is the **only** place to set `Instance` and `DontDestroyOnLoad`
- Always guard against duplicates with `Destroy(gameObject)` before assigning
- **Persistent singletons** (survive scene loads): `GameManager`, `ScoreManager`
- **Scene-local singletons** (destroyed with the scene): `LevelManager`
  - Scene-local singletons must clear `Instance` in `OnDestroy` to avoid stale references

### When NOT to Use Singletons

Singletons are reserved for top-level systems. Gameplay objects (enemies, collectibles, UI widgets) should not be singletons. Use dependency injection via `[SerializeField]` references or `FindObjectOfType` sparingly in `Start()`.

---

## Interface-Driven Design

Core gameplay contracts are defined as interfaces in `TaekwondoTech.Core.Interfaces`:

| Interface | Purpose |
|---|---|
| `IDamageable` | Any entity that can take damage (player, enemies) |
| `ICollectible` | Items the player can pick up |
| `IInteractable` | Objects the player can activate/open |
| `IPowerUp` | Power-ups that activate/deactivate on the player |

### Principles

- Depend on interfaces, not concrete classes, when communicating across feature boundaries
- Example: combat code calls `IDamageable.TakeDamage()` rather than `PlayerHealth.TakeDamage()`
- New systems should define an interface first, then implement it

---

## Event-Driven Architecture

Use `UnityEvent` for loose coupling between systems and UI:

```csharp
public UnityEvent<int> OnScoreChanged;
// ...
OnScoreChanged?.Invoke(_currentScore);
```

### Guidelines

- Always null-check with `?.Invoke()` before firing events
- UI components subscribe to events in `Start()` and **must unsubscribe in `OnDestroy()`** to prevent memory leaks
- Prefer `UnityEvent<T>` over C# `Action<T>` for inspector-wirable events
- Use C# `Action` / `event` for internal, non-inspector events between scripts

---

## Abstract Base Classes

Use abstract base classes to share common behavior across a family of objects:

```csharp
[RequireComponent(typeof(Collider2D))]
public abstract class Collectible : MonoBehaviour
{
    // Shared trigger detection & effects
    protected abstract void OnCollectedLogic(); // subclasses implement specifics
}
```

### Guidelines

- Abstract base classes go in the feature's root namespace file (e.g., `Collectible.cs` in `Collectibles/`)
- Template Method Pattern: base class defines the sequence, subclasses fill in specifics via `protected abstract`

---

## Scene & State Management

All scene transitions must go through `GameManager.LoadScene(string sceneName)`:

- Validates scene is in build settings before loading
- Provides a single extension point for future transition effects (fades, loading screens)
- Never call `SceneManager.LoadScene` directly from gameplay code (only from managers)

Level state is managed by `LevelManager` (Playing → Paused → Completed / GameOver):

- `Time.timeScale` is only set by `LevelManager`
- Game-over reload uses `Invoke(nameof(ReloadScene), delay)` for a brief pause before restart

---

## Physics

- Player movement uses `Rigidbody2D.velocity` set in `FixedUpdate()`
- Input is read in `Update()`, applied in `FixedUpdate()` — never mix them
- Ground detection uses `Physics2D.OverlapCircle` with a dedicated `_groundCheck` transform
- `OnDrawGizmosSelected()` visualises ground check radius in the editor

---

## Prefab & Component Conventions

- Prefabs own their own audio (`AudioClip`) and effects (`ParticleSystem`) via `[SerializeField]`
- `AudioSource.PlayClipAtPoint` is used for one-shot spatial sounds; attach `AudioSource` for looping
- Particle effects are instantiated and self-destroy based on their own duration + lifetime settings
