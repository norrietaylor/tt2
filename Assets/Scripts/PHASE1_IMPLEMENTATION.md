# Phase 1: Foundation Implementation

This document describes the Phase 1 implementation for Taekwondo Tech v2, providing the core foundational systems for player movement, combat, enemies, and collectibles.

## Implemented Components

### 1. Unity Input System Package
- Added `com.unity.inputsystem` package (v1.7.0) to `Packages/manifest.json`
- Created `PlayerInputActions.inputactions` asset with bindings for keyboard and gamepad
- Supports: Move (WASD/Arrows/Gamepad), Jump, Punch, Kick, Special, PowerUp

### 2. Input Management (`Assets/Scripts/Input/`)
- **InputManager.cs**: Singleton that abstracts input across keyboard, touch, and gamepad
  - Auto device switching via Unity's Input System
  - Action maps for Gameplay and UI
  - Exposes input state through properties (MoveInput, JumpPressed, etc.)

### 3. Player Systems (`Assets/Scripts/Player/`)

#### PlayerController.cs
- Horizontal movement with smooth acceleration/deceleration curves
- Gravity-based variable-height jumping (hold to jump higher)
- Double jump functionality
- Air control parameter for mid-air movement adjustment
- Ground check using Physics2D overlap
- Character flipping based on movement direction

#### PlayerCombat.cs
- **Punch**: Short range (1m), 10 damage, 0.3s cooldown
- **Kick**: Medium range (1.5m), 15 damage, 0.5s cooldown
- **Stomp**: Head stomp on enemies with upward bounce (10 force), 20 damage
- UnityEvents for loose coupling (OnPunch, OnKick, OnStomp)
- Attack detection using Physics2D overlap with configurable ranges

#### PlayerHealth.cs
- 3-hit health system (maxHealth configurable)
- Invincibility frames (1 second default) with visual flash effect
- Defeated state management
- UnityEvents for health changes, damage, and defeat
- Respawn functionality

### 4. Enemy AI System (`Assets/Scripts/Enemies/`)

#### EnemyBase.cs
- Base class for all enemies
- Health management (50 HP default)
- Contact damage system (1 damage default)
- Stun and defeat state handling
- Red indicator above head for visual threat marker
- UnityEvents for loose coupling

#### EnemyStateMachine.cs
- State machine managing 6 AI states:
  - **Idle**: Default resting state, transitions to Patrol or Chase
  - **Patrol**: Follows waypoints in a loop
  - **Chase**: Pursues player when in detection radius
  - **Attack**: Engages player when in attack range
  - **Stunned**: Temporary disabled state (2s default)
  - **Defeated**: Final state before destruction
- Configurable detection radius (5m), chase speed (4m/s), patrol speed (2m/s)
- Attack cooldown system (2s default)
- Visual gizmos for debugging (detection radius, attack range, waypoints)

#### State Classes (`Assets/Scripts/Enemies/States/`)
All states implement `IEnemyState` interface with Enter, Update, FixedUpdate, Exit methods:
- **IdleState.cs**: Waits and transitions to Chase or Patrol
- **PatrolState.cs**: Moves between waypoints, faces movement direction
- **ChaseState.cs**: Follows player, stops at chase stop distance
- **AttackState.cs**: Performs attacks on cooldown
- **StunnedState.cs**: Immobilized, recovers after timer
- **DefeatedState.cs**: Disables physics, destroys after delay (2s)

### 5. Collectibles System (`Assets/Scripts/Collectibles/`)

#### Collectible.cs
- Base class for all collectible items (coins, robot parts, power-ups)
- Shimmer effect with configurable speed and amount
- Trigger-based collection on player contact
- UnityEvents for loose coupling (OnCollected)
- Configurable value and destroy-on-collect behavior

#### RobotPart.cs
- Specialized collectible for robot building
- Part types: Head, Body, Arms, Legs, PowerCore
- Rarity system: Common, Rare, Epic (with distinct colors)
- Enhanced glow effect for visual appeal
- Integrates with future craft mode system

### 6. UI System (`Assets/Scripts/UI/`)

#### HUDController.cs
- Manages in-game heads-up display
- Health display using heart icons (array of GameObjects)
- Score tracking and display (TextMeshPro)
- Coin counter
- Reset functionality for level restart

### 7. Level Management (`Assets/Scripts/Levels/`)

#### LevelManager.cs
- Tracks level state and completion
- Monitors collectibles gathered, enemies defeated, damage taken
- Star rating system (1-3 stars based on performance)
  - Thresholds: 1★ = 60%, 2★ = 80%, 3★ = 100%
- Auto-registers event listeners for player, collectibles, enemies
- UnityEvents for level complete and level failed

### 8. Test Scene
- **TestLevel.unity**: Basic scene with orthographic camera
- Ready for adding player prefab, platforms, enemies, and collectibles

## Architecture Patterns

1. **Singleton**: Used for managers (GameManager, InputManager)
2. **State Machine**: Enemy AI and player animation states
3. **Observer Pattern**: UnityEvents for loose coupling between systems
4. **Component-Based**: Each system is a separate MonoBehaviour
5. **ScriptableObjects**: Ready for enemy data, costume data (Phase 2+)

## Next Steps for Unity Editor Setup

To make this functional in Unity Editor:

1. **Create Prefabs**:
   - Player prefab with PlayerController, PlayerCombat, PlayerHealth, Rigidbody2D, BoxCollider2D, SpriteRenderer
   - Enemy prefab with EnemyBase, EnemyStateMachine, Rigidbody2D, BoxCollider2D, SpriteRenderer
   - Collectible prefabs for coins and robot parts

2. **Configure Layers**:
   - Add layers: Player, Enemy, Ground, Collectible
   - Set collision matrix in Project Settings > Physics 2D

3. **Setup Input Manager**:
   - Create GameObject with InputManager and PlayerInput components
   - Assign PlayerInputActions asset to PlayerInput component

4. **Build Test Level**:
   - Add Tilemap for platforms
   - Place player spawn point
   - Add waypoints for enemy patrol
   - Place collectibles
   - Configure camera to follow player

5. **Assign Tags**:
   - Tag player GameObject as "Player"
   - Tag ground platforms as "Ground"

## Design Considerations

- **No file exceeds 500 lines** (per PRD requirement)
- **Loose coupling** via UnityEvents and interfaces
- **Configurable via Inspector** - all major values are SerializeFields
- **Debug visualization** - Gizmos for ranges and waypoints
- **Performance-ready** - Physics2D overlap instead of raycasts where appropriate
- **Extensible** - Base classes ready for specialization (EnemyBase → MonkeyTitan, etc.)

## Validation Checklist

Once Unity Editor setup is complete:

- [ ] Player can move left/right with smooth acceleration/deceleration
- [ ] Player can jump and double-jump
- [ ] Player can punch, kick, and stomp enemies
- [ ] Player takes damage with 1s invincibility flash; 3 hits = defeated
- [ ] Base enemy patrols, chases player, attacks, and can be stomped/defeated
- [ ] Coins and robot parts can be collected
- [ ] HUD shows health hearts and score
- [ ] Test level is playable in Unity Editor (Play Mode)
- [ ] Controls work on keyboard; InputManager abstraction ready for gamepad/touch

## File Structure

```
Assets/
├── Scenes/
│   └── TestLevel.unity
├── Scripts/
│   ├── Core/
│   │   └── GameManager.cs
│   ├── Input/
│   │   ├── InputManager.cs
│   │   └── PlayerInputActions.inputactions
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerCombat.cs
│   │   └── PlayerHealth.cs
│   ├── Enemies/
│   │   ├── EnemyBase.cs
│   │   ├── EnemyStateMachine.cs
│   │   └── States/
│   │       ├── IdleState.cs
│   │       ├── PatrolState.cs
│   │       ├── ChaseState.cs
│   │       ├── AttackState.cs
│   │       ├── StunnedState.cs
│   │       └── DefeatedState.cs
│   ├── Collectibles/
│   │   ├── Collectible.cs
│   │   └── RobotPart.cs
│   ├── UI/
│   │   └── HUDController.cs
│   └── Levels/
│       └── LevelManager.cs
```

## Code Quality

- All scripts include XML documentation comments
- Namespace organization: `TaekwondoTech.{System}`
- Consistent naming conventions (PascalCase for public, camelCase for private)
- SerializeField attributes for Inspector-editable values
- Defensive null checks throughout
- Clear state management in enemy AI

## Performance Notes

- Physics2D.OverlapCircleAll used for attack detection (efficient for few enemies)
- State machine avoids string comparisons (uses enum)
- Minimal Update() loops - only when necessary
- Ready for object pooling in Phase 2+ (enemies, projectiles, particles)
