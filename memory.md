# Test Improver Memory - norrietaylor/tt2

## Repository Overview
- **Name**: tt2 (Taekwondo Tech V2)
- **Type**: Unity C# platformer game (targeting kids 6-12, cross-platform WebGL/iOS/Android)
- **Status**: Phase 1 active — LevelManager, CameraFollower, ParallaxBackground on main; enemy AI (PR #64), collectibles/HUD (PR #66), and player combat/health (PR #63) awaiting merge
- **Unity Version**: 2022.3.20f1 (LTS)

## Build/Test/Coverage Commands
- **Build**: `game-ci/unity-builder@v4` via `.github/workflows/unity-build.yml` (requires UNITY_LICENSE, UNITY_EMAIL, UNITY_PASSWORD secrets)
- **Tests (local)**: Window → General → Test Runner → EditMode tab → Run All
- **Tests (CI)**: `game-ci/unity-test-runner@v4` — NOT YET CONFIGURED in CI
- **Unity Test Framework**: `com.unity.test-framework: 1.1.33` already in `Unity/Packages/manifest.json`
- **Test assemblies**: Added in run 2026-03-03 PR — `TaekwondoTech.Scripts.asmdef` (production) + `TaekwondoTech.Tests.EditMode.asmdef` (tests)

## Testing Notes
- GameManager is a MonoBehaviour singleton — EditMode tests need `AddComponent<T>` + `DestroyImmediate` teardown
- LevelManager has `OnDestroy` that nulls `Instance` — singleton state cleans up properly between tests
- `Time.timeScale` changes must be reset to 1f in TearDown
- `Invoke` calls do NOT fire in EditMode tests (no time progression) — safe for `OnPlayerDefeated` test
- `SceneManager.LoadScene` NOT testable in EditMode — requires PlayMode
- Production assembly named `TaekwondoTech.Scripts` (covers both TaekwondoTech.Core and TaekwondoTech.Levels namespaces)
- safeoutputs create_pull_request works (returned success in run 2026-03-03); PR number unknown until next run
- Previous failed test branches: `test-assist/editmode-test-infrastructure-47290ccb182f68cc`, `test-assist/editmode-test-infrastructure-cd04d6f16f3c5554` (previous attempts)

## Maintainer Priorities
- Build a Unity C# platformer game (Phase 1: Foundation in active development)
- No specific testing priorities stated yet

## Testing Backlog
1. **PlayerHealth 3-hit system** (PR #63) — once merged: invincibility window, defeat event, damage counter
2. **Enemy state machine** (PR #64) — once merged: state transitions (Idle→Patrol→Chase→Attack→Stunned→Defeated)
3. **Collectibles/HUD** (PR #66) — once merged: collection events, counter updates
4. **CI test runner** — add `game-ci/unity-test-runner@v4` job
5. **`IDamageable`/`ICollectible` contract tests** — interface conformance once implementing classes land on main
6. **`CameraFollower` / `ParallaxBackground`** — PlayMode candidates

## Work In Progress
- PR created in run 2026-03-03: `test-assist/editmode-test-infrastructure` — EditMode test infrastructure + 19 tests (closes #62)
  - PR number unknown (need to verify next run via PR list)

## Completed Work
- 2026-02-27 Run 1: Analyzed repo (no source code yet), created Feb monthly summary #14
- 2026-02-27 Run 2: Identified test-framework already installed, created test infrastructure proposal issue #62
- 2026-02-28 Run 3: Attempted PR — failed (CI permissions error, issue #85)
- 2026-03-01 Run 4: Attempted PR again — failed at safeoutputs
- 2026-03-02 Run 5: Memory claimed PR was created but no PR found — actually failed
- 2026-03-03 Run 6 (THIS RUN): Successfully created test infrastructure PR (assembly defs + 19 EditMode tests); closed Feb summary #14; created March summary

## Task Run History
| Task | Last Run | Notes |
|------|----------|-------|
| Task 1 (Commands) | 2026-02-27 | Unity build CI found; no test runner CI yet |
| Task 2 (Opportunities) | 2026-03-02 | Backlog refreshed; PRs #63/#64/#66 still open |
| Task 3 (Implement Tests) | 2026-03-03 | PR created: test infrastructure + 19 EditMode tests |
| Task 4 (Maintain PRs) | 2026-03-03 | No existing Test Improver PRs to maintain at run start |
| Task 5 (Comment on Issues) | never | TODO next run — check issue #62 comments for activity |
| Task 6 (Test Infrastructure) | 2026-03-03 | Assembly definitions added via PR |
| Task 7 (Monthly Summary) | 2026-03-03 | Closed Feb #14; created March summary (number TBD) |

## Monthly Summary Issues
- 2026-02: Issue #14 (closed 2026-03-03)
- 2026-03: Issue created in run 2026-03-03 (number TBD — need to verify next run)

## Checked-off Items by User
- None yet
