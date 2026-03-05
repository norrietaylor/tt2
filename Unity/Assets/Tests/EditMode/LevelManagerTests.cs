using NUnit.Framework;
using UnityEngine;
using TaekwondoTech.Levels;

namespace TaekwondoTech.Tests.EditMode
{
    /// <summary>
    /// NUnit EditMode tests for LevelManager state machine transitions.
    /// </summary>
    public class LevelManagerTests
    {
        private GameObject _gameObject;
        private LevelManager _levelManager;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("LevelManager");
            _levelManager = _gameObject.AddComponent<LevelManager>();
            _levelManager.StartLevel();
        }

        [TearDown]
        public void TearDown()
        {
            Time.timeScale = 1f;
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void InitialState_AfterStartLevel_IsPlaying()
        {
            Assert.AreEqual(LevelManager.LevelState.Playing, _levelManager.CurrentState);
        }

        [Test]
        public void PauseLevel_WhenPlaying_StateBecomesPaused()
        {
            _levelManager.PauseLevel();

            Assert.AreEqual(LevelManager.LevelState.Paused, _levelManager.CurrentState);
        }

        [Test]
        public void ResumeLevel_WhenPaused_StateBecomesPlaying()
        {
            _levelManager.PauseLevel();
            _levelManager.ResumeLevel();

            Assert.AreEqual(LevelManager.LevelState.Playing, _levelManager.CurrentState);
        }

        [Test]
        public void OnLevelCompleted_WhenPlaying_StateBecomesCompleted()
        {
            _levelManager.OnLevelCompleted();

            Assert.AreEqual(LevelManager.LevelState.Completed, _levelManager.CurrentState);
        }

        [Test]
        public void OnPlayerDefeated_WhenPlaying_StateBecomesGameOver()
        {
            _levelManager.OnPlayerDefeated();

            Assert.AreEqual(LevelManager.LevelState.GameOver, _levelManager.CurrentState);
        }

        [Test]
        public void OnPlayerDefeated_WhenAlreadyGameOver_StateRemainsGameOver()
        {
            _levelManager.OnPlayerDefeated();
            _levelManager.OnPlayerDefeated();

            Assert.AreEqual(LevelManager.LevelState.GameOver, _levelManager.CurrentState);
        }

        [Test]
        public void OnLevelCompleted_WhenAlreadyCompleted_StateRemainsCompleted()
        {
            _levelManager.OnLevelCompleted();
            _levelManager.OnLevelCompleted();

            Assert.AreEqual(LevelManager.LevelState.Completed, _levelManager.CurrentState);
        }

        [Test]
        public void PauseLevel_WhenNotPlaying_StateDoesNotChangeToPaused()
        {
            _levelManager.OnPlayerDefeated();
            _levelManager.PauseLevel();

            Assert.AreEqual(LevelManager.LevelState.GameOver, _levelManager.CurrentState);
        }
    }
}
