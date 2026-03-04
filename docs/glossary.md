---
title: Glossary
description: Reference definitions for Taekwondo Tech v2 technical terminology
---

# Glossary

Technical terms used in the Taekwondo Tech v2 Unity codebase.

## Enemy AI

### AttackState

An `IEnemyState` implementation. The enemy stops moving, plays an attack animation, and deals damage to the player if within `AttackRange`. Transitions back to `ChaseState` after a 0.5-second attack window.

### ChaseState

An `IEnemyState` implementation. The enemy moves toward the player and shows an alert indicator. Transitions to `AttackState` when the player is within `AttackRange`, or back to `PatrolState` when the player exits 1.5× the detection radius.

### DefeatedState

An `IEnemyState` implementation entered when enemy health reaches zero. Disables the collider, plays a defeat animation, and destroys the `GameObject` after a brief delay.

### EnemyBase

The `MonoBehaviour` base class for all enemies. Manages health, holds references to required components (`Rigidbody2D`, `Collider2D`, `Animator`), and owns the `EnemyStateMachine`. Implements `IDamageable` via explicit interface members.

### EnemyStateMachine

A lightweight state machine that holds a reference to the current `IEnemyState`. Calls `Exit()` on the outgoing state and `Enter()` on the incoming state during every `ChangeState()` call, then delegates each `Update()` frame to the current state's `Execute()` method.

### IdleState

An `IEnemyState` implementation. The enemy stands still for a random 1–3-second interval, then transitions to `PatrolState`.

### IEnemyState

The interface contract for all enemy AI states. Requires three methods: `Enter()` (called once on state entry), `Execute()` (called every frame), and `Exit()` (called once on state exit).

### PatrolState

An `IEnemyState` implementation. The enemy moves back and forth between two configured waypoints (`WaypointA`, `WaypointB`). Transitions to `ChaseState` when the player enters the detection radius.

### StunnedState

An `IEnemyState` implementation entered when the enemy takes damage while alive. Briefly freezes the enemy for 0.5 seconds, then transitions to `ChaseState`.

### Waypoint

A `Transform` reference used by `PatrolState` to define the two endpoints of an enemy's patrol path (`WaypointA` and `WaypointB`).

## Core Interfaces

### ICollectible

Interface for collectible items. Requires `OnCollect(GameObject)`, a `CollectibleType` property, and a `CollectibleRarity` property. Implemented by `Coin`, `RobotPart`, and other pickup types.

### IDamageable

Interface for entities that can receive and recover from damage. Requires `Health`, `MaxHealth`, and `IsAlive` properties, plus `TakeDamage(float)`, `TakeDamage(float, GameObject)`, and `Heal(float)` methods. Implemented by `EnemyBase` and `PlayerHealth`.

### IInteractable

Interface for world objects the player can interact with (switches, doors, NPCs). Requires `Interact(GameObject)`, a `CanInteract` boolean, and an `InteractionPrompt` string.

### IPowerUp

Interface for power-ups in the queue system. Requires `PowerUpType`, `Duration`, `IsActive`, `Activate(GameObject)`, and `Deactivate(GameObject)`.

## Enumerations

### CollectibleRarity

Rarity tier for collectible items, used primarily by robot parts. Values: `Common`, `Rare`, `Epic`.

### CollectibleType

Identifies the category of a collectible. Values: `Coin`, `RobotPart`, `PowerUp`, `CostumeItem`.

### PowerUpType

Identifies a power-up's effect. Values: `SpeedBoost`, `Shield`, `ElementalAttack`, `Invincibility`.
