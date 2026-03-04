# TestLevel Scene Setup Guide

This guide explains how to set up the **TestLevel.unity** scene in the Unity Editor to integrate all Phase 1 components.

## Prerequisites

Before setting up the scene, ensure the following scripts are in place:
- `LevelManager.cs` (Assets/Scripts/Levels/)
- `CameraFollower.cs` (Assets/Scripts/Levels/)
- `ParallaxBackground.cs` (Assets/Scripts/Levels/)

## Scene Setup Steps

### 1. Create the TestLevel Scene

1. In Unity Editor, go to **File → New Scene**
2. Save it as `Assets/Scenes/TestLevel.unity`
3. Add the scene to Build Settings (**File → Build Settings → Add Open Scenes**)

### 2. Set Up the Tilemap System

1. Right-click in Hierarchy → **2D Object → Tilemap → Rectangular**
2. This creates a Grid with a Tilemap child
3. Rename the Tilemap to "PlatformsTilemap"
4. Create additional Tilemaps for:
   - `GroundTilemap` (for the floor)
   - `WallsTilemap` (for boundaries)

**Configure Tilemap Colliders:**
- Add `Tilemap Collider 2D` component to each tilemap
- Add `Composite Collider 2D` to the Grid parent
- On the Tilemap Collider 2D, check "Used By Composite"

### 3. Create Level Platforms

Using the Tile Palette (**Window → 2D → Tile Palette**):
1. Paint a ground layer across the bottom
2. Create at least 3 elevated platforms at different heights
3. Add walls on the left and right boundaries
4. Create gaps between platforms for jumping

**Minimum Level Layout:**
```
┌─────────────────────┐
│                     │
│    [platform3]      │
│                     │
│  [platform2]        │
│           [platform1]│
│                     │
│══════GROUND═════════│
└─────────────────────┘
```

### 4. Set Up the Camera

1. Select the Main Camera in Hierarchy
2. **Add Component** → `CameraFollower` script
3. Configure settings:
   - **Smooth Speed**: 0.125
   - **Offset**: (0, 2, -10)
   - **Use Bounds**: ✓ (checked)
   - **Min Bounds**: Set based on your level (e.g., -20, 0)
   - **Max Bounds**: Set based on your level (e.g., 20, 10)
4. The **Target** field will be set at runtime to the player

### 5. Create Parallax Background Layers

For each background layer (far, mid, near):

1. Create an empty GameObject: **GameObject → Create Empty**
2. Rename it (e.g., "BackgroundFar", "BackgroundMid", "BackgroundNear")
3. Add a **Sprite Renderer** component
4. Add the **ParallaxBackground** script
5. Configure parallax multipliers:
   - **Far Background**: 0.1
   - **Mid Background**: 0.3
   - **Near Background**: 0.6
6. Position each layer at different Z depths:
   - Far: z = 10
   - Mid: z = 5
   - Near: z = 2

**Layer Setup Example:**
```
GameObject: BackgroundFar
  - Transform: Position (0, 5, 10)
  - Sprite Renderer: (Add your far background sprite)
  - ParallaxBackground: Parallax Effect Multiplier = 0.1
```

### 6. Add the Level Manager

1. Create an empty GameObject: **GameObject → Create Empty**
2. Rename it to "LevelManager"
3. Add the **LevelManager** script component
4. The script will automatically initialize when the scene starts

### 7. Place Player Prefab (When Available)

Once the Player prefab is created:
1. Drag the Player prefab into the Hierarchy
2. Position it at the starting location (e.g., (-15, 1, 0))
3. In the Main Camera, set the **CameraFollower → Target** field to the Player's Transform

### 8. Place Enemy Prefabs (When Available)

For each enemy:
1. Drag Enemy prefab into the scene
2. Position on different platforms
3. Configure patrol waypoints if the enemy has patrol behavior
4. Minimum: Place 2 enemies in the level

### 9. Place Collectibles (When Available)

1. Place **Coin** collectibles throughout the level
   - Distribute them across all platforms
   - Place some in harder-to-reach locations
2. Place at least one **Robot Part** collectible in the level

### 10. Add HUD Canvas (When Available)

1. Right-click Hierarchy → **UI → Canvas**
2. Rename to "HUD"
3. Set Canvas to **Screen Space - Overlay**
4. Add HUD components:
   - Health hearts display (top-left)
   - Score display (top-right)
   - Any other UI elements

## Level Testing Checklist

Once the scene is set up:

- [ ] Scene opens without errors in Unity Editor
- [ ] Camera follows player smoothly
- [ ] Camera stays within defined bounds
- [ ] All 3 parallax layers scroll at different rates
- [ ] Player can navigate all platforms
- [ ] Player can jump between platforms
- [ ] Enemies patrol or behave as expected
- [ ] Player can collect coins and robot parts
- [ ] Game-over triggers and scene reloads when player is defeated
- [ ] No compiler errors in Unity Console

## Integration with Future Components

### Connecting Player Defeat Event

When the `PlayerHealth` script is implemented, it should call:
```csharp
LevelManager.Instance.OnPlayerDefeated();
```

This triggers the game-over flow and scene reload.

### Level Completion

When level completion criteria are met (e.g., reach end of level), call:
```csharp
LevelManager.Instance.OnLevelCompleted();
```

## Troubleshooting

**Camera not following player:**
- Ensure the Target field is set on CameraFollower
- Check that the player GameObject has a Transform

**Parallax not working:**
- Verify Camera Transform is assigned on each ParallaxBackground
- Check that layers have different Z positions
- Ensure parallax multipliers are different for each layer

**Collisions not working:**
- Add Tilemap Collider 2D to tilemaps
- Ensure player has a Rigidbody2D and Collider2D
- Check Layer Collision Matrix in **Edit → Project Settings → Physics 2D**

## Notes

This scene is a test/integration scene for Phase 1. As more systems are implemented (Player, Enemies, Collectibles, HUD), they should be integrated into this scene for end-to-end testing in Play Mode.
