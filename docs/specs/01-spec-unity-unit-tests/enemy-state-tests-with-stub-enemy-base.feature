# Source: docs/specs/01-spec-unity-unit-tests/01-spec-unity-unit-tests.md
# Pattern: State
# Recommended test type: Unit

Feature: Enemy State Tests with Stub EnemyBase

  # IdleState

  Scenario: IdleState calls StopMovement on Enter
    Given a StubEnemyBase configured with default parameters
    And an IdleState instance
    When the IdleState Enter method is called
    Then StopMovement has been called on the StubEnemyBase

  Scenario: IdleState transitions to PatrolState after idle duration expires
    Given a StubEnemyBase configured with an idle duration
    And an IdleState that has been entered
    When Execute is called repeatedly until the idle duration has elapsed
    Then the state machine transitions to a PatrolState

  Scenario: IdleState transitions to ChaseState when player is within detection radius
    Given a StubEnemyBase with a detection radius of 5 units
    And the player is positioned 3 units away from the enemy
    And an IdleState that has been entered
    When Execute is called on the IdleState
    Then the state machine transitions to a ChaseState

  # PatrolState

  Scenario: PatrolState transitions to ChaseState when player is within detection radius
    Given a StubEnemyBase with a detection radius of 5 units
    And the player is positioned 3 units away from the enemy
    And a PatrolState that has been entered
    When Execute is called on the PatrolState
    Then the state machine transitions to a ChaseState

  Scenario: PatrolState moves toward waypoints during Execute
    Given a StubEnemyBase with patrol waypoints defined
    And a PatrolState that has been entered
    When Execute is called on the PatrolState
    Then MoveTowards has been called on the StubEnemyBase toward the next waypoint

  # ChaseState

  Scenario: ChaseState shows alert indicator on Enter
    Given a StubEnemyBase configured with default parameters
    And a ChaseState instance
    When the ChaseState Enter method is called
    Then ShowAlertIndicator has been called with true on the StubEnemyBase

  Scenario: ChaseState transitions to AttackState when player is within attack range
    Given a StubEnemyBase with an attack range of 1 unit
    And the player is positioned 0.5 units away from the enemy
    And a ChaseState that has been entered
    When Execute is called on the ChaseState
    Then the state machine transitions to an AttackState

  Scenario: ChaseState transitions to PatrolState when player exceeds hysteresis distance
    Given a StubEnemyBase with a detection radius of 5 units
    And the player is positioned 8 units away from the enemy (beyond 5 * 1.5 = 7.5)
    And a ChaseState that has been entered
    When Execute is called on the ChaseState
    Then the state machine transitions to a PatrolState

  Scenario: ChaseState hides alert indicator on Exit
    Given a StubEnemyBase configured with default parameters
    And a ChaseState that has been entered
    When the ChaseState Exit method is called
    Then ShowAlertIndicator has been called with false on the StubEnemyBase

  # AttackState

  Scenario: AttackState calls StopMovement on Enter
    Given a StubEnemyBase configured with default parameters
    And an AttackState instance
    When the AttackState Enter method is called
    Then StopMovement has been called on the StubEnemyBase

  Scenario: AttackState transitions to ChaseState after attack duration
    Given a StubEnemyBase with an attack duration of 0.5 seconds
    And an AttackState that has been entered
    When Execute is called repeatedly simulating 0.5 seconds of elapsed time
    Then the state machine transitions to a ChaseState

  Scenario: AttackState deals damage exactly once at midpoint of attack
    Given a StubEnemyBase with an attack duration of 0.5 seconds
    And an AttackState that has been entered
    When Execute is called repeatedly simulating 0.5 seconds of elapsed time
    Then damage was dealt to the player exactly once
    And the damage was dealt at approximately the 0.25 second mark

  # StunnedState

  Scenario: StunnedState calls StopMovement on Enter
    Given a StubEnemyBase configured with default parameters
    And a StunnedState instance
    When the StunnedState Enter method is called
    Then StopMovement has been called on the StubEnemyBase

  Scenario: StunnedState transitions to ChaseState after stun duration
    Given a StubEnemyBase with a stun duration of 0.5 seconds
    And a StunnedState that has been entered
    When Execute is called repeatedly simulating 0.5 seconds of elapsed time
    Then the state machine transitions to a ChaseState

  # DefeatedState

  Scenario: DefeatedState calls StopMovement on Enter
    Given a StubEnemyBase configured with default parameters
    And a DefeatedState instance
    When the DefeatedState Enter method is called
    Then StopMovement has been called on the StubEnemyBase

  Scenario: DefeatedState disables collider on Enter
    Given a StubEnemyBase with an active collider component
    And a DefeatedState instance
    When the DefeatedState Enter method is called
    Then the collider on the StubEnemyBase is disabled
