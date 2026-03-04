# Workflow & Documentation Standards

**Project:** Taekwondo Tech v2
**Status:** Living Document
**Last Updated:** 2026-03-04

---

## Repository Structure

```
tt2/
├── docs/
│   └── prd.md               # Product Requirements Document (source of truth)
├── scratchpad/              # Developer spec files (input for consolidator)
│   ├── code-organization-patterns.md
│   ├── unity-architecture-guidelines.md
│   └── workflow-documentation-standards.md
├── Unity/
│   └── Assets/Scripts/      # C# source, organized by namespace/feature
├── CONTRIBUTING.md
└── README.md
```

---

## Scratchpad Files

The `scratchpad/` directory contains short developer specification files used as input for the `developer-docs-consolidator` automated workflow. The consolidator synthesizes these into `scratchpad/dev.md`.

### Authoring Guidelines

- **Format:** GitHub-flavored Markdown
- **Length:** Concise — each file should cover one topic and stay under ~150 lines
- **Headers:** Use `##` for major sections, `###` for subsections
- **Code Examples:** Include short, self-contained examples that illustrate the pattern
- **Tables:** Use Markdown tables for mapping/reference information
- **Status:** Include a `Status: Living Document` front matter so readers know it evolves
- One spec file per concern — do not merge unrelated topics into a single file

### Naming Convention

`kebab-case-topic-name.md` (e.g., `code-organization-patterns.md`)

---

## PRD as Source of Truth

`docs/prd.md` is the authoritative product requirements document. When implementing a feature:

1. Locate the relevant requirement (e.g., `REQ-004: Robot Building`)
2. Reference it in code comments: `// Implements REQ-004`
3. Do not add behavior that contradicts the PRD without a PRD update first

---

## Branching & PRs

- Feature branches: `feature/<short-description>` or `claude/issue-<number>-<date>`
- All changes go through pull requests — direct commits to `main` are not permitted
- PR titles should be imperative mood: _"Add parallax background"_ not _"Added parallax"_
- Reference the issue in the PR body: `Closes #<issue-number>`

---

## Automated Workflows

| Workflow | Trigger | Output |
|---|---|---|
| `developer-docs-consolidator` | Daily schedule | `scratchpad/dev.md` synthesizing all spec files |
| `deep-report` | Daily schedule | GitHub Discussion with intelligence briefing |

The `developer-docs-consolidator` reads all `*.md` files in `scratchpad/` (excluding `dev.md` itself) and consolidates them into a single developer reference. To contribute to `dev.md`:

1. Create or update a file in `scratchpad/`
2. The consolidator will pick it up in the next daily run

---

## Commit Message Style

```
<type>: <short imperative summary>

<optional body explaining why, not what>

Closes #<issue-number>
```

**Types:** `feat`, `fix`, `refactor`, `docs`, `chore`, `test`

Example:
```
feat: add Singleton pattern to LevelManager

LevelManager needs a scene-local singleton to coordinate
game-over and pause state from multiple scripts.

Closes #42
```

---

## Code Review Checklist

- [ ] Follows namespace and naming conventions (`code-organization-patterns.md`)
- [ ] New systems have matching interfaces in `Core/Interfaces.cs`
- [ ] Persistent singletons call `DontDestroyOnLoad`; scene-local singletons clear `Instance` in `OnDestroy`
- [ ] Events are unsubscribed in `OnDestroy`
- [ ] `[RequireComponent]` declared for hard dependencies
- [ ] XML doc comments on all public/protected members
- [ ] Scene transitions go through `GameManager.LoadScene`
- [ ] No direct `SceneManager.LoadScene` calls outside manager classes
- [ ] PRD requirement referenced in comments if implementing a spec'd feature
