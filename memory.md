# Test Improver Memory - norrietaylor/tt2

## Repository Overview
- **Name**: tt2 (Taekwondo Tech V2)
- **Type**: Unity C# platformer game (targeting kids 6-12, cross-platform WebGL/iOS/Android)
- **Status**: Early development — GameManager.cs and Interfaces.cs created; Phase 1 plan actively being worked
- **Unity Version**: 2022.3.20f1 (LTS)

## Build/Test/Coverage Commands
- **Build**: `game-ci/unity-builder@v4` via `.github/workflows/unity-build.yml` (requires UNITY_LICENSE, UNITY_EMAIL, UNITY_PASSWORD secrets)
- **Tests**: `game-ci/unity-test-runner@v4` — NOT YET CONFIGURED in CI
- **Unity Test Framework**: `com.unity.test-framework: 1.1.33` is already in `Unity/Packages/manifest.json`
- **Test assemblies**: Not set up yet — no `.asmdef` files or `Assets/Tests/` directory

## Testing Notes
- GameManager is a MonoBehaviour singleton — EditMode tests need `AddComponent<T>` + `DestroyImmediate` teardown
- To reference production code from test assembly, need to add `.asmdef` to `Assets/Scripts/`
- Unity EditMode tests can test null/empty guards in LoadScene; PlayMode tests needed for actual scene loading
- No production `.asmdef` exists yet — all scripts use default `Assembly-CSharp` assembly

## Maintainer Priorities
- Build a Unity C# platformer game (Phase 1: Foundation currently in progress)
- No specific testing priorities stated yet

## Testing Backlog
1. **GameManager.LoadScene guard logic** — null/empty input, unregistered scene names (EditMode)
2. **Shared interfaces** (PR #60) — once implemented, test IDamageable/ICollectible/IPowerUp contracts
3. **Player controller** (issue #55/#25) — movement, jump, combat: high-value unit tests
4. **Enemy state machine** (issue #57) — state transitions: ideal for unit tests
5. **Save/persistence system** (issue #33) — data integrity tests for cross-platform

## Work In Progress
- Issue created: "Set up Unity EditMode test infrastructure" — proposing `.asmdef` setup + first GameManager tests

## Completed Work
- 2026-02-27 Run 1: Analyzed repo (no source code yet), created monthly summary
- 2026-02-27 Run 2: Identified test-framework already installed, created test infrastructure proposal issue

## Task Run History
| Task | Last Run | Notes |
|------|----------|-------|
| Task 1 (Commands) | 2026-02-27 | Unity build CI found; no test runner CI yet |
| Task 2 (Opportunities) | 2026-02-27 | GameManager + Interfaces identified; backlog updated |
| Task 6 (Test Infrastructure) | 2026-02-27 | Created proposal issue |
| Task 7 (Monthly Summary) | 2026-02-27 | Updated issue #14 |

## Monthly Summary Issues
- 2026-02: Issue #14 (open, updated run 2)

## Checked-off Items by User
- None yet
