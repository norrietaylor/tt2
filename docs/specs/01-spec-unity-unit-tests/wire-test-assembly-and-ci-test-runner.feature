# Source: docs/specs/01-spec-unity-unit-tests/01-spec-unity-unit-tests.md
# Pattern: CLI/Process
# Recommended test type: Integration

Feature: Wire Test Assembly and CI Test Runner

  Scenario: Test assembly can import production code namespaces
    Given the test assembly TT2.Tests.EditMode references production code
    And a test file uses "using TaekwondoTech.Enemies"
    When the Unity EditMode test suite is compiled
    Then the test assembly compiles without errors
    And the test can instantiate production classes from TaekwondoTech.Enemies

  Scenario: Placeholder test is removed from the test suite
    Given the test assembly previously contained PlaceholderTest.cs
    When the Unity EditMode test suite is executed
    Then no test named "PlaceholderTest" appears in the test results
    And the test runner reports zero tests from the placeholder file

  Scenario: CI runs EditMode tests on pull request
    Given a pull request is opened from an in-repo branch
    When the unity-build workflow is triggered
    Then a test job runs using game-ci/unity-test-runner with testMode editmode
    And the test job targets projectPath Unity on ubuntu-latest
    And test results are uploaded as workflow artifacts

  Scenario: CI test job fails fast before build matrix
    Given a pull request with a failing unit test
    When the unity-build workflow is triggered
    Then the test job executes before the build matrix jobs
    And the build matrix jobs do not start if the test job fails

  Scenario: CI test job uses same license secrets as build job
    Given the unity-build workflow is configured with UNITY_LICENSE, UNITY_EMAIL, and UNITY_PASSWORD secrets
    When the test job runs in CI
    Then the test runner authenticates using the same Unity license secrets
    And the job skips execution for fork PRs where secrets are unavailable

  Scenario: CI test job caches Unity Library folder
    Given a previous CI run has populated the Library cache
    When the test job runs on a subsequent pull request
    Then the Unity Library folder is restored from cache
    And the cache key pattern matches the one used by the build job
