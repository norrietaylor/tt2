using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using TaekwondoTech.Core;

namespace TaekwondoTech.Tests.EditMode
{
    public class ScoreManagerTests
    {
        private GameObject _gameObject;
        private ScoreManager _scoreManager;

        [SetUp]
        public void SetUp()
        {
            ResetSingleton();
            _gameObject = new GameObject("ScoreManager");
            _scoreManager = _gameObject.AddComponent<ScoreManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_gameObject != null)
                Object.DestroyImmediate(_gameObject);

            ResetSingleton();
        }

        private static void ResetSingleton()
        {
            var field = typeof(ScoreManager).GetField(
                "<Instance>k__BackingField",
                BindingFlags.Static | BindingFlags.NonPublic);
            field?.SetValue(null, null);
        }

        [Test]
        public void AddScore_PositiveAmount_IncreasesScore()
        {
            _scoreManager.AddScore(10);

            Assert.AreEqual(10, _scoreManager.CurrentScore,
                "Score should increase by the added amount.");
        }

        [Test]
        public void AddScore_Zero_ScoreUnchanged()
        {
            _scoreManager.AddScore(0);

            Assert.AreEqual(0, _scoreManager.CurrentScore,
                "Adding zero should not change the score.");
        }

        [Test]
        public void AddScore_Negative_ScoreClampedToZero()
        {
            _scoreManager.AddScore(-5);

            Assert.AreEqual(0, _scoreManager.CurrentScore,
                "Score should be clamped to zero when a negative value is added.");
        }

        [Test]
        public void ResetScore_AfterAddingPoints_ScoreReturnsToZero()
        {
            _scoreManager.AddScore(50);
            _scoreManager.ResetScore();

            Assert.AreEqual(0, _scoreManager.CurrentScore,
                "Score should be zero after ResetScore is called.");
        }

        [Test]
        public void OnScoreChanged_AfterAddScore_FiresWithCorrectValue()
        {
            int receivedScore = -1;
            _scoreManager.OnScoreChanged.AddListener(score => receivedScore = score);

            _scoreManager.AddScore(25);

            Assert.AreEqual(25, receivedScore,
                "OnScoreChanged event should fire with the updated score value after AddScore.");
        }

        [Test]
        public void OnScoreChanged_AfterResetScore_FiresWithZero()
        {
            _scoreManager.AddScore(100);
            int receivedScore = -1;
            _scoreManager.OnScoreChanged.AddListener(score => receivedScore = score);

            _scoreManager.ResetScore();

            Assert.AreEqual(0, receivedScore,
                "OnScoreChanged event should fire with zero after ResetScore is called.");
        }

        [Test]
        public void Singleton_Instance_ReturnsSameObject()
        {
            ScoreManager instance1 = ScoreManager.Instance;
            ScoreManager instance2 = ScoreManager.Instance;

            Assert.AreSame(instance1, instance2,
                "ScoreManager.Instance should return the same object on every access.");
        }

        [Test]
        public void Singleton_Instance_MatchesCreatedComponent()
        {
            Assert.AreSame(_scoreManager, ScoreManager.Instance,
                "ScoreManager.Instance should be the component added to the GameObject.");
        }
    }
}
