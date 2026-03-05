# Workflow Documentation Standards

## Branch Naming

All branches follow the pattern `<type>/<short-description>` using lowercase kebab-case:

| Type | Use |
|---|---|
| `feature/` | New gameplay functionality |
| `fix/` | Bug fixes |
| `chore/` | Non-functional changes (CI, dependencies, tooling) |

Examples: `feature/player-combat`, `fix/enemy-patrol-loop`, `chore/update-unity-version`.

## Pull Request Guidelines

**Title**: Use imperative mood without a period — "Add PlayerController jump mechanic", not "Added jump" or "Jump mechanic added.".

**Description** must include:
1. A brief summary of what changed and why.
2. Testing steps (specific steps a reviewer can follow to verify the behavior).
3. Screenshots or GIFs for any visual change.

**Size**: Each PR addresses one coherent unit of work. Unrelated changes belong in separate PRs.

**Checklist before requesting review**:
- [ ] All new `.cs` files are in `Unity/Assets/Scripts/<domain>/`
- [ ] Every new `.cs` file has a corresponding `.cs.meta` with a unique GUID
- [ ] Every new folder has a corresponding `.meta` file in the parent directory
- [ ] All scripts use the correct `TaekwondoTech.*` namespace
- [ ] No new file exceeds 500 lines
- [ ] 4-space indentation, LF line endings

## CI/CD Pipeline

The workflow at `.github/workflows/unity-build.yml` builds the Unity project for WebGL, iOS, and Android in parallel on every push to `main` and on every PR from branches in this repository.

**Required secrets** (configured under Settings → Secrets and variables → Actions):

| Secret | Description |
|---|---|
| `UNITY_LICENSE` | Unity license XML from Unity Hub activation |
| `UNITY_EMAIL` | Unity account email |
| `UNITY_PASSWORD` | Unity account password |

Builds triggered from forks do not have access to these secrets and will not run the full Unity build jobs. See `.github/workflows/unity-build.yml` for the authoritative CI configuration.

**Build failure causes** (in order of frequency):
1. C# compilation errors in any script.
2. A script exceeds 500 lines.
3. Missing `.meta` file for a new asset or folder.
4. Wrong or missing namespace declaration.

## Commit Messages

Use imperative mood in the subject line, 72 characters or fewer: "Fix enemy patrol reversal at waypoint B". The subject describes what the commit does, not what was done. Body lines (optional) provide context for non-obvious changes.

## Code Review

At least one approving review is required before merging. Reviewers check:
- Correctness of logic relative to the PR description.
- Adherence to the 500-line cap and namespace conventions.
- Presence of `.meta` files for all new assets.
- That Unity Test Runner tests cover any new logic in `Assets/Scripts/`.

## Unity Test Runner

New logic in `Assets/Scripts/` is accompanied by Unity Test Runner tests. Tests live in a dedicated `Tests/` assembly and reference scripts under test via `[UnityTest]` or `[Test]` attributes. Tests do not depend on scene state where avoidable.

## Documentation Updates

When a PR introduces or changes a gameplay system, the corresponding section of `docs/prd.md` is updated to reflect the implemented state. The `scratchpad/` directory holds evolving developer specifications; `scratchpad/dev.md` is the consolidated reference synthesized from those specs by the automated consolidator workflow.
