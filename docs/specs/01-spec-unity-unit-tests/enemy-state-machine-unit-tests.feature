# Source: docs/specs/01-spec-unity-unit-tests/01-spec-unity-unit-tests.md
# Pattern: State
# Recommended test type: Unit

Feature: EnemyStateMachine Unit Tests

  Scenario: Initial state machine has null current state
    Given a newly instantiated EnemyStateMachine
    When the CurrentState property is read
    Then CurrentState returns null

  Scenario: ChangeState sets the current state
    Given a newly instantiated EnemyStateMachine
    And a MockEnemyState implementing IEnemyState
    When ChangeState is called with the MockEnemyState
    Then CurrentState returns the MockEnemyState instance

  Scenario: ChangeState calls Enter on the new state
    Given a newly instantiated EnemyStateMachine
    And a MockEnemyState that tracks Enter call count
    When ChangeState is called with the MockEnemyState
    Then Enter has been called exactly once on the MockEnemyState

  Scenario: ChangeState calls Exit on the previous state
    Given an EnemyStateMachine with an active MockEnemyState as the current state
    And a second MockEnemyState
    When ChangeState is called with the second MockEnemyState
    Then Exit has been called exactly once on the first MockEnemyState

  Scenario: Exit is called before Enter on state transition
    Given an EnemyStateMachine with an active MockEnemyState as the current state
    And a second MockEnemyState that records call order
    When ChangeState is called with the second MockEnemyState
    Then the first state's Exit was called before the second state's Enter

  Scenario: Update calls Execute on the current state
    Given an EnemyStateMachine with an active MockEnemyState
    When Update is called on the state machine
    Then Execute has been called exactly once on the MockEnemyState

  Scenario: Update with null state does not throw
    Given a newly instantiated EnemyStateMachine with no state set
    When Update is called on the state machine
    Then no exception is thrown

  Scenario: ChangeState to null calls Exit on the previous state
    Given an EnemyStateMachine with an active MockEnemyState
    When ChangeState is called with null
    Then Exit has been called exactly once on the MockEnemyState
    And CurrentState returns null
