# T01 Proof Summary: Wire Test Assembly and CI Test Runner

## Task

Fix test infrastructure so tests can reference production code and run in CI.

## Changes Made

1. **TT2.Tests.EditMode.asmdef** - Added `Assembly-CSharp` to references array so test code can reference production scripts.
2. **PlaceholderTest.cs** and **PlaceholderTest.cs.meta** - Removed scaffold placeholder test that served no purpose.
3. **.github/workflows/unity-build.yml** - Added `test` job using `game-ci/unity-test-runner@v4` before the build matrix. Job runs EditMode tests and uploads results as artifacts. Uses the same fork-skip condition, secrets, and cache strategy as the build job.

## Proof Artifacts

| File | Type | Status | Description |
|------|------|--------|-------------|
| T01-01-file.txt | file | PASS | asmdef contains Assembly-CSharp reference |
| T01-02-file.txt | file | PASS | PlaceholderTest.cs and .meta removed |
| T01-03-file.txt | file | PASS | CI workflow has unity-test-runner@v4 job with editmode + artifact upload |

## Overall Result: PASS

All three requirements implemented and verified.
