# Test Improver Memory - norrietaylor/tt2

## Repository Overview
- **Name**: tt2 (Taekwondo Tech V2)
- **Type**: Unity C# platformer game (targeting kids 6-12, cross-platform WebGL/iOS/Android)
- **Status**: Phase 1 active — LevelManager, CameraFollower, ParallaxBackground landed (PR #65); enemy AI (#64) and collectibles/HUD (#66) in review
- **Unity Version**: 2022.3.20f1 (LTS)

## Build/Test/Coverage Commands
- **Build**: `game-ci/unity-builder@v4` via `.github/workflows/unity-build.yml` (requires UNITY_LICENSE, UNITY_EMAIL, UNITY_PASSWORD secrets)
- **Tests**: `game-ci/unity-test-runner@v4` — NOT YET CONFIGURED in CI
- **Run locally**: Window → General → Test Runner → EditMode tab → Run All
- **Unity Test Framework**: `com.unity.test-framework: 1.1.33` already in `Unity/Packages/manifest.json`
- **Test assemblies**: Set up in PR #81 — `TaekwondoTech.Scripts.asmdef` (production) + `TaekwondoTech.Tests.EditMode.asmdef` (tests)

## Testing Notes
- GameManager is a MonoBehaviour singleton — EditMode tests need `AddComponent<T>` + `DestroyImmediate` teardown
- LevelManager has `OnDestroy` that nulls `Instance` — singleton state cleans up properly between tests
- `Time.timeScale` changes must be reset to 1f in TearDown
- `Invoke` calls do NOT fire in EditMode tests (no time progression) — safe for `OnPlayerDefeated` test
- `SceneManager.LoadScene` NOT testable in EditMode — requires PlayMode
- Production assembly named `TaekwondoTech.Scripts` (covers both TaekwondoTech.Core and TaekwondoTech.Levels namespaces)

## Maintainer Priorities
- Build a Unity C# platformer game (Phase 1: Foundation in active development)
- No specific testing priorities stated yet

## Testing Backlog
1. **`GameManager.LoadScene` validation guards** — null/empty guards testable in EditMode (infrastructure now in place)
2. **Player controller / combat / health** (issues #55, #56) — high-value once PlayerController/PlayerHealth land
3. **Enemy state machine** (issue #57) — state transition logic ideal for unit tests once PR #64 merges
4. **`IDamageable`/`ICollectible` contract tests** — interface conformance via PRs #64/#66 code
5. **Save/persistence system** (issue #33) — data integrity tests for cross-platform

## Work In Progress
- PR #81 open: EditMode test infrastructure + LevelManager state-machine tests (17 tests)

## Completed Work
- 2026-02-27 Run 1: Analyzed repo (no source code yet), created monthly summary
- 2026-02-27 Run 2: Identified test-framework already installed, created test infrastructure proposal issue #62
- 2026-02-28 Run 3: Created PR #81 — test infrastructure + 17 LevelManager tests (closes #62)

## Task Run History
| Task | Last Run | Notes |
|------|----------|-------|
| Task 1 (Commands) | 2026-02-27 | Unity build CI found; no test runner CI yet |
| Task 2 (Opportunities) | 2026-02-27 | GameManager + Interfaces identified; backlog updated |
| Task 3 (Implement Tests) | 2026-02-28 | PR #81 created: LevelManager tests (17 EditMode) |
| Task 6 (Test Infrastructure) | 2026-02-28 | Assembly definitions added in PR #81 |
| Task 7 (Monthly Summary) | 2026-02-28 | Updated issue #14 |

## Monthly Summary Issues
- 2026-02: Issue #14 (open, updated run 3)

## Checked-off Items by User
- None yet
