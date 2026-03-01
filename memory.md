# Test Improver Memory - norrietaylor/tt2

## Repository Overview
- **Name**: tt2 (Taekwondo Tech V2)
- **Type**: Unity C# platformer game (targeting kids 6-12, cross-platform WebGL/iOS/Android)
- **Status**: Phase 1 active — LevelManager, CameraFollower, ParallaxBackground on main; enemy AI (PR #64) and collectibles/HUD (PR #66) and player combat/health (PR #63) in review
- **Unity Version**: 2022.3.20f1 (LTS)

## Build/Test/Coverage Commands
- **Build**: `game-ci/unity-builder@v4` via `.github/workflows/unity-build.yml` (requires UNITY_LICENSE, UNITY_EMAIL, UNITY_PASSWORD secrets)
- **Tests**: `game-ci/unity-test-runner@v4` — NOT YET CONFIGURED in CI
- **Run locally**: Window → General → Test Runner → EditMode tab → Run All
- **Unity Test Framework**: `com.unity.test-framework: 1.1.33` already in `Unity/Packages/manifest.json`
- **Test assemblies**: Added in run 2026-03-01 — `TaekwondoTech.Scripts.asmdef` (production) + `TaekwondoTech.Tests.EditMode.asmdef` (tests)

## Testing Notes
- GameManager is a MonoBehaviour singleton — EditMode tests need `AddComponent<T>` + `DestroyImmediate` teardown
- LevelManager has `OnDestroy` that nulls `Instance` — singleton state cleans up properly between tests
- `Time.timeScale` changes must be reset to 1f in TearDown
- `Invoke` calls do NOT fire in EditMode tests (no time progression) — safe for `OnPlayerDefeated` test
- `SceneManager.LoadScene` NOT testable in EditMode — requires PlayMode
- Production assembly named `TaekwondoTech.Scripts` (covers both TaekwondoTech.Core and TaekwondoTech.Levels namespaces)
- PR creation through safeoutputs works (run 2026-03-01 succeeded — previous failure was a CI permissions issue specific to 2026-02-28 run)

## Maintainer Priorities
- Build a Unity C# platformer game (Phase 1: Foundation in active development)
- No specific testing priorities stated yet

## Testing Backlog
1. **`GameManager.LoadScene` validation guards** — null/empty guards testable in EditMode (infrastructure now in place once PR merges)
2. **Player controller / combat / health** (PR #63) — PlayerHealth 3-hit system + invincibility once merged
3. **Enemy state machine** (PR #64) — state transition logic ideal for unit tests once merged
4. **`IDamageable`/`ICollectible` contract tests** — interface conformance once implementing classes land on main
5. **Save/persistence system** (issue #33) — data integrity tests for cross-platform

## Work In Progress
- PR open: `test-assist/editmode-test-infrastructure` — EditMode test infrastructure + 19 LevelManager tests (closes #62)
  - Created in run 2026-03-01 via safeoutputs-create_pull_request

## Completed Work
- 2026-02-27 Run 1: Analyzed repo (no source code yet), created monthly summary
- 2026-02-27 Run 2: Identified test-framework already installed, created test infrastructure proposal issue #62
- 2026-02-28 Run 3: Attempted PR #81 — failed (CI permissions error, issue #85)
- 2026-03-01 Run 4: Created test infrastructure PR (assembly defs + 19 LevelManager tests); closed Feb summary #14; created March summary

## Task Run History
| Task | Last Run | Notes |
|------|----------|-------|
| Task 1 (Commands) | 2026-02-27 | Unity build CI found; no test runner CI yet |
| Task 2 (Opportunities) | 2026-02-27 | GameManager + Interfaces identified; backlog updated |
| Task 3 (Implement Tests) | 2026-03-01 | PR created: test infrastructure + 19 LevelManager EditMode tests |
| Task 4 (Maintain PRs) | 2026-03-01 | PR just created, no maintenance needed |
| Task 6 (Test Infrastructure) | 2026-03-01 | Assembly definitions added in PR |
| Task 7 (Monthly Summary) | 2026-03-01 | Closed Feb #14; created March summary |

## Monthly Summary Issues
- 2026-02: Issue #14 (closed 2026-03-01)
- 2026-03: Issue created in run 2026-03-01 (number assigned by GitHub after workflow)

## Checked-off Items by User
- None yet
