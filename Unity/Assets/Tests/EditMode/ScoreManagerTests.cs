using NUnit.Framework;
using UnityEngine;
using TaekwondoTech.Core;

namespace TaekwondoTech.Tests.EditMode
{
    /// <summary>
    /// EditMode unit tests for ScoreManager.
    /// Each test creates a fresh GameObject with ScoreManager and destroys it afterwards
    /// to ensure singleton state is fully isolated between tests.
    /// </summary>
    public class ScoreManagerTests
    {
        private ScoreManager _scoreManager;

        [SetUp]
        public void SetUp()
        {
            // Destroy any pre-existing singleton instance to ensure a clean state.
            if (ScoreManager.Instance != null)
            {
                Object.DestroyImmediate(ScoreManager.Instance.gameObject);
            }

            // AddComponent triggers Awake(), which sets ScoreManager.Instance.
            var go = new GameObject("ScoreManager");
            _scoreManager = go.AddComponent<ScoreManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_scoreManager != null)
            {
                Object.DestroyImmediate(_scoreManager.gameObject);
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // AddScore
        // ──────────────────────────────────────────────────────────────────────

        [Test]
        public void AddScore_PositiveValue_IncreasesScoreByCorrectAmount()
        {
            _scoreManager.AddScore(10);

            Assert.AreEqual(10, _scoreManager.CurrentScore,
                "Score should equal the added amount after a single positive AddScore call.");
        }

        [Test]
        public void AddScore_MultiplePositiveValues_AccumulatesCorrectly()
        {
            _scoreManager.AddScore(5);
            _scoreManager.AddScore(3);

            Assert.AreEqual(8, _scoreManager.CurrentScore,
                "Score should be the sum of all positive values added.");
        }

        [Test]
        public void AddScore_Zero_LeavesScoreUnchanged()
        {
            _scoreManager.AddScore(0);

            Assert.AreEqual(0, _scoreManager.CurrentScore,
                "Score should remain 0 when 0 points are added.");
        }

        [Test]
        public void AddScore_NegativeValue_ClampsScoreToZero()
        {
            // AddScore uses Mathf.Max(_currentScore, 0), so negative inputs are clamped.
            _scoreManager.AddScore(-5);

            Assert.AreEqual(0, _scoreManager.CurrentScore,
                "Score should clamp to 0 when a negative value would make it go below zero.");
        }

        [Test]
        public void AddScore_NegativeValueFromPositiveScore_ClampsToZeroNotNegative()
        {
            _scoreManager.AddScore(3);
            _scoreManager.AddScore(-10);

            Assert.AreEqual(0, _scoreManager.CurrentScore,
                "Score should clamp to 0 even when subtraction from a positive score would yield a negative result.");
        }

        // ──────────────────────────────────────────────────────────────────────
        // ResetScore
        // ──────────────────────────────────────────────────────────────────────

        [Test]
        public void ResetScore_AfterAddingPoints_SetsScoreToZero()
        {
            _scoreManager.AddScore(50);
            _scoreManager.ResetScore();

            Assert.AreEqual(0, _scoreManager.CurrentScore,
                "Score should be 0 after ResetScore regardless of previous value.");
        }

        [Test]
        public void ResetScore_WhenScoreAlreadyZero_RemainsZero()
        {
            _scoreManager.ResetScore();

            Assert.AreEqual(0, _scoreManager.CurrentScore,
                "ResetScore on an already-zero score should leave the score at 0.");
        }

        // ──────────────────────────────────────────────────────────────────────
        // OnScoreChanged event
        // ──────────────────────────────────────────────────────────────────────

        [Test]
        public void OnScoreChanged_FiresAfterAddScore_WithUpdatedValue()
        {
            int receivedValue = -1;
            _scoreManager.OnScoreChanged.AddListener(score => receivedValue = score);

            _scoreManager.AddScore(25);

            Assert.AreEqual(25, receivedValue,
                "OnScoreChanged should fire with the new score value after AddScore.");
        }

        [Test]
        public void OnScoreChanged_FiresAfterResetScore_WithZero()
        {
            _scoreManager.AddScore(30);

            int receivedValue = -1;
            _scoreManager.OnScoreChanged.AddListener(score => receivedValue = score);

            _scoreManager.ResetScore();

            Assert.AreEqual(0, receivedValue,
                "OnScoreChanged should fire with 0 after ResetScore.");
        }

        // ──────────────────────────────────────────────────────────────────────
        // Singleton pattern
        // ──────────────────────────────────────────────────────────────────────

        [Test]
        public void Instance_IsSetAfterComponentAwake()
        {
            Assert.IsNotNull(ScoreManager.Instance,
                "ScoreManager.Instance should be non-null after AddComponent triggers Awake.");
        }

        [Test]
        public void Instance_ReturnsSameObjectAcrossMultipleAccesses()
        {
            ScoreManager first = ScoreManager.Instance;
            ScoreManager second = ScoreManager.Instance;

            Assert.AreSame(first, second,
                "ScoreManager.Instance should return the same object on every access.");
        }

        [Test]
        public void Instance_MatchesComponentAddedInSetUp()
        {
            Assert.AreSame(_scoreManager, ScoreManager.Instance,
                "ScoreManager.Instance should be the same component that was added to the GameObject.");
        }

        [Test]
        public void SecondGameObject_WithScoreManager_IsDestroyedByAwake()
        {
            var duplicate = new GameObject("Duplicate");
            var duplicateManager = duplicate.AddComponent<ScoreManager>();

            // The second instance should have been destroyed by the Singleton guard in Awake.
            Assert.IsTrue(duplicateManager == null || !duplicateManager.gameObject.activeSelf || duplicateManager.gameObject == null,
                "A second ScoreManager component should be destroyed immediately by the singleton Awake guard.");

            // Clean up if somehow still alive.
            if (duplicate != null)
            {
                Object.DestroyImmediate(duplicate);
            }
        }
    }
}
