using NUnit.Framework;
using UnityEngine;
using TaekwondoTech.Levels;

namespace Tests.EditMode
{
    /// <summary>
    /// NUnit EditMode tests for LevelManager state machine transitions.
    /// Covers: Playing → Paused → Playing, Playing → Completed, Playing → GameOver,
    /// and invalid/idempotent transition guards.
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
            // Start() is not automatically called in EditMode; initialize state manually.
            _levelManager.StartLevel();
        }

        [TearDown]
        public void TearDown()
        {
            // Cancel any pending Invoke calls (e.g., ReloadScene from OnPlayerDefeated)
            // to avoid nondeterministic side effects after the assertion.
            if (_levelManager != null)
            {
                _levelManager.CancelInvoke();
            }
            // Reset Time.timeScale in case any test left it at 0.
            Time.timeScale = 1f;
            Object.DestroyImmediate(_gameObject);
            // OnDestroy clears LevelManager.Instance, so subsequent SetUps are clean.
        }

        [Test]
        public void InitialState_AfterStartLevel_IsPlaying()
        {
            Assert.AreEqual(LevelManager.LevelState.Playing, _levelManager.CurrentState);
        }

        [Test]
        public void PauseLevel_FromPlaying_StateIsPaused()
        {
            _levelManager.PauseLevel();

            Assert.AreEqual(LevelManager.LevelState.Paused, _levelManager.CurrentState);
        }

        [Test]
        public void ResumeLevel_FromPaused_StateIsPlaying()
        {
            _levelManager.PauseLevel();
            _levelManager.ResumeLevel();

            Assert.AreEqual(LevelManager.LevelState.Playing, _levelManager.CurrentState);
        }

        [Test]
        public void OnLevelCompleted_FromPlaying_StateIsCompleted()
        {
            _levelManager.OnLevelCompleted();

            Assert.AreEqual(LevelManager.LevelState.Completed, _levelManager.CurrentState);
        }

        [Test]
        public void OnPlayerDefeated_FromPlaying_StateIsGameOver()
        {
            _levelManager.OnPlayerDefeated();

            Assert.AreEqual(LevelManager.LevelState.GameOver, _levelManager.CurrentState);
        }

        [Test]
        public void OnLevelCompleted_WhenAlreadyCompleted_StateRemainsCompleted()
        {
            _levelManager.OnLevelCompleted();
            _levelManager.OnLevelCompleted(); // second call should be a no-op

            Assert.AreEqual(LevelManager.LevelState.Completed, _levelManager.CurrentState);
        }

        [Test]
        public void OnPlayerDefeated_WhenAlreadyGameOver_StateRemainsGameOver()
        {
            _levelManager.OnPlayerDefeated();
            _levelManager.OnPlayerDefeated(); // second call should be a no-op

            Assert.AreEqual(LevelManager.LevelState.GameOver, _levelManager.CurrentState);
        }

        [Test]
        public void PauseLevel_WhenNotPlaying_StateDoesNotChange()
        {
            _levelManager.OnLevelCompleted(); // transition to Completed
            _levelManager.PauseLevel();       // guard: PauseLevel only works from Playing

            Assert.AreEqual(LevelManager.LevelState.Completed, _levelManager.CurrentState);
        }
    }
}
