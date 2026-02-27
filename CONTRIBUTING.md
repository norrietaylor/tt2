# Contributing to tt2

Thank you for your interest in contributing to tt2 (Taekwondo Tech V2)!

## GitHub Secrets

The Unity CI/CD pipeline requires the following secrets to be configured in the repository settings under **Settings → Secrets and variables → Actions**:

| Secret | Description |
|---|---|
| `UNITY_LICENSE` | Unity license XML (exported from Unity Hub or the Unity activation workflow) |
| `UNITY_EMAIL` | Email address associated with your Unity account |
| `UNITY_PASSWORD` | Password for your Unity account |

### Obtaining a Unity License XML

1. Follow the [game-ci manual activation guide](https://game.ci/docs/github/activation) to generate a `.ulf` license file.
2. Copy the full contents of the `.ulf` file and store it as the `UNITY_LICENSE` secret.

## CI/CD Pipeline

The workflow at `.github/workflows/unity-build.yml` runs automatically on:

- Every push to `main`
- Every pull request

It builds the Unity project for **WebGL**, **iOS**, and **Android** targets in parallel, caches the `Library` folder between runs, and uploads each build as a downloadable artifact from the workflow run.
