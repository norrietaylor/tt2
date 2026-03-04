# Code Organization Patterns

**Project:** Taekwondo Tech v2
**Status:** Living Document
**Last Updated:** 2026-03-04

---

## Namespace Conventions

All C# scripts live under the `TaekwondoTech` root namespace, subdivided by feature area:

| Namespace | Directory | Responsibility |
|---|---|---|
| `TaekwondoTech.Core` | `Scripts/Core/` | Game state, scoring, cross-cutting interfaces |
| `TaekwondoTech.Player` | `Scripts/Player/` | Player movement, health, combat, animation |
| `TaekwondoTech.Collectibles` | `Scripts/Collectibles/` | Coin, robot part, and collectible base logic |
| `TaekwondoTech.Levels` | `Scripts/Levels/` | Level management, camera, parallax |
| `TaekwondoTech.UI` | `Scripts/UI/` | HUD, menus, overlays |

New feature areas get a new namespace + matching subdirectory (e.g., `TaekwondoTech.Enemies` in `Scripts/Enemies/`).

---

## Naming Conventions

### Fields

- Private fields use an underscore prefix: `_camelCase`
- Inspector-visible fields use `[SerializeField] private`: `[SerializeField] private float _moveSpeed = 5f;`
- Never expose fields as `public`; use properties instead

### Properties

- Public read-only properties: `PascalCase` — `public float Health { get; private set; }`
- Computed properties preferred over redundant backing fields where trivial

### Methods

- All methods: `PascalCase`
- Private helper methods: `PascalCase` (no underscore prefix)
- Unity lifecycle callbacks follow Unity convention: `Awake`, `Start`, `Update`, `FixedUpdate`, `OnDestroy`

### Local Variables

- All local variables: `camelCase`
- Avoid single-letter names except loop indices (`i`, `j`)

---

## File Structure

- **One class (or interface group) per file**
- File name matches the primary type: `PlayerController.cs` contains `PlayerController`
- `Interfaces.cs` in `Core/` is an exception — groups all game-wide interfaces in one file

---

## Inspector Grouping

Use `[Header("...")]` to group related serialized fields in the Inspector:

```csharp
[Header("Movement Settings")]
[SerializeField] private float _moveSpeed = 5f;
[SerializeField] private float _jumpForce = 10f;

[Header("Ground Check")]
[SerializeField] private Transform _groundCheck;
[SerializeField] private LayerMask _groundLayer;
```

---

## Documentation

All public and protected members require XML doc comments:

```csharp
/// <summary>
/// Apply damage to this entity.
/// </summary>
/// <param name="damage">Amount of damage to apply.</param>
void TakeDamage(float damage);
```

Private implementation methods do not require doc comments unless the logic is non-obvious.

---

## Tags and Layers

- Use `CompareTag("Player")` (not `== "Player"`) for tag comparisons to avoid allocations
- Define layer constants in `Core/` if layer-based queries become widespread

---

## Component Dependencies

Use `[RequireComponent(typeof(...))]` to declare hard dependencies:

```csharp
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour { ... }
```

This ensures Unity auto-adds dependencies and prevents misconfigured prefabs.
