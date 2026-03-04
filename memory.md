# Test Improver Memory - norrietaylor/tt2

## Repository Overview
- **Name**: tt2 (Taekwondo Tech V2)
- **Type**: Unity C# platformer game (targeting kids 6-12, cross-platform WebGL/iOS/Android)
- **Status**: Phase 1 active — many scripts now on main (PlayerHealth, ScoreManager, Enemies, Collectibles, HUD)
- **Unity Version**: 2022.3.20f1 (LTS)

## Build/Test/Coverage Commands
- **Build**: `game-ci/unity-builder@v4` via `.github/workflows/unity-build.yml` (requires UNITY_LICENSE, UNITY_EMAIL, UNITY_PASSWORD secrets)
- **Tests (local)**: Window → General → Test Runner → EditMode tab → Run All
- **Tests (CI)**: `game-ci/unity-test-runner@v4` — NOT YET CONFIGURED in CI
- **Unity Test Framework**: `com.unity.test-framework: 1.1.33` already in `Unity/Packages/manifest.json`
- **Test assemblies**: Being added via PR #149 (`TT2.Core.asmdef` + `TT2.Tests.EditMode.asmdef`)

## Testing Notes
- GameManager is a MonoBehaviour singleton — EditMode tests need `AddComponent<T>` + `DestroyImmediate` teardown
- LevelManager has `OnDestroy` that nulls `Instance` — singleton state cleans up properly between tests
- `Time.timeScale` changes must be reset to 1f in TearDown
- `Invoke` calls do NOT fire in EditMode tests (no time progression)
- `SceneManager.LoadScene` NOT testable in EditMode — requires PlayMode
- **CI issue**: Agent-written PRs keep failing Unity builds (issue #156); PR #167 fixes this by adding CLAUDE.md + missing .meta files
- **AGENTS.md / CLAUDE.md**: Neither present on main yet; PR #167 adds CLAUDE.md
- PlayerHealth uses invincibility via Coroutine — not testable in EditMode (no time progression)
- ScoreManager clamps score to 0 (`Mathf.Max(_currentScore, 0)` after AddScore)
- ScoreManager fires `OnScoreChanged` in both `Start()` and after `AddScore`/`ResetScore`

## Maintainer Priorities
- Build a Unity C# platformer game (Phase 1: Foundation in active development)
- Issue #156: Fix agent-written PR CI failures (CLAUDE.md + .meta files)
- No explicit testing priorities stated yet

## Testing Backlog
1. **ScoreManager tests** — branch `claude/issue-145-20260304-0334` ready, needs PR from maintainer (13 test cases)
2. **PlayerHealth tests** — branch `claude/issue-146-20260304-0334` ready, needs PR from maintainer (10 test cases)
3. **Enemy state machine** — `EnemyBase`/`EnemyStateMachine`/States all on main; state transitions prime unit test candidates
4. **CI test runner** — add `game-ci/unity-test-runner@v4` job
5. **`IDamageable`/`ICollectible` contract tests** — interface conformance tests
6. **Collectibles** (Coin, RobotPart, Collectible) — collection events once infrastructure lands
7. **`CameraFollower` / `ParallaxBackground`** — PlayMode candidates

## Work In Progress
None — waiting for PR #167 (CLAUDE.md fix) to unblock PRs #149 and #150.

## Completed Work
- 2026-02-27 Run 1: Analyzed repo (no source code yet), created Feb monthly summary #14
- 2026-02-27 Run 2: Identified test-framework already installed, created test infrastructure proposal issue #62
- 2026-02-28 to 2026-03-03: Multiple failed PR attempts (CI/safeoutputs issues)
- 2026-03-04 Run 7: Commented on #62 (status update + path to closure); closed Feb #14; created March summary

## Task Run History
| Task | Last Run | Notes |
|------|----------|-------|
| Task 1 (Commands) | 2026-02-27 | Unity build CI found; no test runner CI yet |
| Task 2 (Opportunities) | 2026-03-04 | Backlog refreshed; Claude Code branches for #145/#146 exist but need PRs |
| Task 3 (Implement Tests) | never (successful) | All previous attempts failed; Claude Code is now doing this work |
| Task 4 (Maintain PRs) | 2026-03-04 | No Test Improver PRs open; PRs #149/#150 are by maintainer (Claude Code) |
| Task 5 (Comment on Issues) | 2026-03-04 | Commented on #62 |
| Task 6 (Test Infrastructure) | 2026-03-04 | Tracked via PRs #149/#150 |
| Task 7 (Monthly Summary) | 2026-03-04 | Closed Feb #14; created March summary (number TBD) |

## Monthly Summary Issues
- 2026-02: Issue #14 (closed 2026-03-04)
- 2026-03: Created 2026-03-04 (number TBD — search for "[Test Improver] Monthly Activity 2026-03")

## Open PRs of Interest (NOT Test Improver PRs)
- PR #149: Unity Test Framework infrastructure (Claude Code, from issue #144)
- PR #150: LevelManager EditMode tests (Claude Code, from issue #147)
- PR #167: Fix agent PR build failures — CLAUDE.md + missing .meta files (prerequisite for CI)

## Checked-off Items by User
- None yet
