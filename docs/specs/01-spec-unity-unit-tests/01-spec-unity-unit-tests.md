# 01-spec-unity-unit-tests

## Introduction/Overview

Build out Unity unit test infrastructure and initial test coverage for the Taekwondo Tech v2 codebase. The project currently has near-zero test coverage (1 placeholder test across 24 production scripts / ~1,827 LOC). The test assembly exists but cannot reference production code, and CI builds do not run tests. This spec addresses GitHub issue #143.

## Goals

1. Wire the test assembly (`TT2.Tests.EditMode`) to reference production code so tests can import `TaekwondoTech.*` namespaces
2. Write comprehensive EditMode unit tests for all pure C# classes (EnemyStateMachine + 6 enemy states)
3. Add `game-ci/unity-test-runner` to the CI pipeline so tests gate every PR
4. Remove the placeholder test and establish test patterns for future contributors
5. Achieve >90% line coverage of the pure C# enemy AI classes

## User Stories

- As a **developer**, I want the test assembly to reference production code so that I can write tests that import game namespaces
- As a **developer**, I want unit tests for the enemy state machine so that I can refactor AI behavior with confidence
- As a **contributor**, I want CI to run tests automatically so that broken tests block merges
- As a **developer**, I want mock/stub patterns established so that I can easily add tests for new enemy states

## Demoable Units of Work

### Unit 1: Wire Test Assembly and CI Test Runner

**Purpose:** Fix the test infrastructure so tests can reference production code and run in CI.

**Functional Requirements:**
- The system shall update `TT2.Tests.EditMode.asmdef` to add a reference to the default `Assembly-CSharp` assembly (via `"overrideReferences": false` or by adding an explicit assembly reference), enabling `using TaekwondoTech.*` in test files
- The system shall remove `Unity/Assets/Tests/EditMode/PlaceholderTest.cs` and its `.meta` file
- The system shall add a `test` job to `.github/workflows/unity-build.yml` that uses `game-ci/unity-test-runner@v4` with `testMode: editmode` and `projectPath: Unity`, running on `ubuntu-latest` before the build matrix
- The CI test job shall upload test results as artifacts via `actions/upload-artifact@v4`
- The CI test job shall use the same Unity license secrets (`UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD`) and the same fork-PR skip condition as the build job
- The CI test job shall cache `Unity/Library` using the same cache key pattern as the build job

**Proof Artifacts:**
- File: `Unity/Assets/Tests/EditMode/TT2.Tests.EditMode.asmdef` contains assembly reference enabling production code imports
- File: `.github/workflows/unity-build.yml` contains `game-ci/unity-test-runner@v4` step with `testMode: editmode`
- File: `Unity/Assets/Tests/EditMode/PlaceholderTest.cs` does not exist (removed)

### Unit 2: EnemyStateMachine Unit Tests

**Purpose:** Test the core state machine engine — the simplest pure C# class with no dependencies.

**Functional Requirements:**
- The system shall create `Unity/Assets/Tests/EditMode/Enemies/EnemyStateMachineTests.cs` (with `.meta`) in namespace `TaekwondoTech.Tests.EditMode.Enemies`
- The system shall create a `MockEnemyState` test helper implementing `IEnemyState` that tracks Enter/Execute/Exit call counts and order
- Tests shall verify: initial `CurrentState` is null, `ChangeState` sets `CurrentState`, `ChangeState` calls `Enter()` on new state, `ChangeState` calls `Exit()` on previous state, `Exit()` is called before `Enter()` on transition, `Update()` calls `Execute()` on current state, `Update()` with null state does not throw, `ChangeState(null)` calls `Exit()` on previous state
- The system shall create a folder `.meta` file for `Unity/Assets/Tests/EditMode/Enemies/`

**Proof Artifacts:**
- Test: `EnemyStateMachineTests` passes — all state machine lifecycle assertions verified
- File: `Unity/Assets/Tests/EditMode/Enemies/EnemyStateMachineTests.cs` exists with `[TestFixture]` attribute

### Unit 3: Enemy State Tests with Stub EnemyBase

**Purpose:** Test all 6 concrete enemy states for correct transitions and behavior using a stub/mock approach.

**Functional Requirements:**
- The system shall create `Unity/Assets/Tests/EditMode/Enemies/StubEnemyBase.cs` (with `.meta`) — a test helper that provides the data enemy states read from `EnemyBase` without requiring MonoBehaviour. This may be achieved via a subclass, wrapper, or by making state tests use a test-specific interface. The implementation must allow states to call `GetDistanceToPlayer()`, `MoveTowards()`, `StopMovement()`, `ShowAlertIndicator()`, and `StateMachine.ChangeState()` without Unity scene dependencies.
- The system shall create `Unity/Assets/Tests/EditMode/Enemies/EnemyStateTests.cs` (with `.meta`) containing tests for all 6 states
- **IdleState tests:** verify it calls `StopMovement()` on Enter, transitions to `PatrolState` after idle duration expires, transitions to `ChaseState` when player is within detection radius
- **PatrolState tests:** verify it transitions to `ChaseState` when player is within detection radius, moves toward waypoints during Execute
- **ChaseState tests:** verify it calls `ShowAlertIndicator(true)` on Enter, transitions to `AttackState` when player is within attack range, transitions to `PatrolState` when player exceeds detection radius * 1.5 (hysteresis), calls `ShowAlertIndicator(false)` on Exit
- **AttackState tests:** verify it calls `StopMovement()` on Enter, transitions to `ChaseState` after attack duration (0.5s simulated), deals damage at midpoint (0.25s) only once per attack
- **StunnedState tests:** verify it calls `StopMovement()` on Enter, transitions to `ChaseState` after stun duration (0.5s simulated)
- **DefeatedState tests:** verify it calls `StopMovement()` on Enter, disables collider on Enter
- Each test file shall follow naming convention `[Method]_[Condition]_[Expected]`
- All new files and folders shall have corresponding `.meta` files with unique GUIDs

**Proof Artifacts:**
- Test: `EnemyStateTests` passes — all 6 states' transitions and behaviors verified
- File: `Unity/Assets/Tests/EditMode/Enemies/EnemyStateTests.cs` exists with tests for all states
- File: `Unity/Assets/Tests/EditMode/Enemies/StubEnemyBase.cs` exists providing test-friendly enemy data

## Non-Goals (Out of Scope)

- PlayMode tests (require scene setup, deferred to future spec)
- MonoBehaviour tests for ScoreManager, GameManager, PlayerHealth (deferred — require GameObject instantiation patterns)
- Code coverage reporting or coverage gates in CI
- Refactoring production code for testability (e.g., extracting interfaces from EnemyBase)
- Testing UI, Levels, Collectibles, or Player domain classes

## Design Considerations

No specific design requirements identified. Tests are code-only with no UI.

## Repository Standards

- All new files in `Unity/Assets/Tests/EditMode/` with `.meta` files (unique 32-char hex GUIDs)
- New folders require folder `.meta` files in parent directory
- Namespace: `TaekwondoTech.Tests.EditMode` (or child namespaces like `.Enemies`)
- 500-line cap per file (split test files if approaching limit)
- 4-space indent, LF line endings, UTF-8
- PascalCase test classes, `[Method]_[Condition]_[Expected]` test method naming

## Technical Considerations

- **Assembly reference challenge:** Production code is in the default `Assembly-CSharp` assembly (no `.asmdef`). The test assembly must reference it. Unity allows this via `"overrideReferences": false` and removing `"autoReferenced": false`, or by keeping override references and adding `"Assembly-CSharp.dll"` to `precompiledReferences`. The implementer should verify which approach works with Unity 2022.3.
- **EnemyBase stubbing:** Enemy states take `EnemyBase` (a MonoBehaviour) in their constructor. Since MonoBehaviours cannot be instantiated with `new`, the stub approach may need to: (a) use reflection to set fields, (b) create a GameObject in test and add EnemyBase component, or (c) refactor minimally to accept an interface. Option (b) is preferred as it requires no production code changes.
- **Time simulation:** States use `Time.deltaTime` in `Execute()`. EditMode tests can simulate time by calling `Execute()` multiple times or by setting accumulated values. States track elapsed time via internal timers incremented by `Time.deltaTime`.
- **CI runner:** `game-ci/unity-test-runner@v4` requires the same Unity license secrets as the builder. It should run as a separate job before the build matrix to fail fast on test failures.

## Security Considerations

No security implications. Tests do not handle credentials or user data. CI uses existing Unity license secrets already configured.

## Success Metrics

- All EnemyStateMachine tests pass (lifecycle, transitions, null safety)
- All 6 enemy state tests pass (transitions, behavior verification)
- CI pipeline runs tests on every PR and blocks merge on failure
- Test assembly can import any `TaekwondoTech.*` namespace
- No production code changes required (tests are additive only)

## Open Questions

1. **Assembly reference approach:** Does Unity 2022.3 allow `Assembly-CSharp` reference via `precompiledReferences` in the test `.asmdef`, or must `overrideReferences` be set to `false`? The implementer should test both approaches.
2. **EnemyBase in EditMode:** Can `new GameObject().AddComponent<EnemyBase>()` work in EditMode tests given `[RequireComponent]` attributes? If not, the stub approach will need adjustment.
