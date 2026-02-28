using NUnit.Framework;
using TaekwondoTech.Levels;
using UnityEngine;

namespace TaekwondoTech.Tests.EditMode.Levels
{
    /// <summary>
    /// EditMode tests for LevelManager state machine logic.
    ///
    /// In EditMode, AddComponent triggers Awake() but not Start(). LevelManager's
    /// _currentState defaults to Playing (enum 0) — valid initial state for all tests.
    ///
    /// Note: methods that internally call SceneManager (ReloadScene, LoadScene) are
    /// triggered via Invoke or direct call but are not exercised here because
    /// scene loading requires PlayMode. These tests focus on state transitions and guards.
    /// </summary>
    [TestFixture]
    public class LevelManagerTests
    {
        private GameObject _go;
        private LevelManager _manager;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("LevelManager");
            _manager = _go.AddComponent<LevelManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Time.timeScale = 1f;
            Object.DestroyImmediate(_go);
        }

        // ── Singleton ────────────────────────────────────────────────────────────

        [Test]
        public void Singleton_FirstInstance_SetsStaticInstance()
        {
            Assert.AreEqual(_manager, LevelManager.Instance);
        }

        [Test]
        public void Singleton_SecondInstance_DoesNotReplaceFirstInstance()
        {
            var go2 = new GameObject("LevelManager2");
            var manager2 = go2.AddComponent<LevelManager>();

            // Awake on manager2 sees Instance != null, calls Destroy (async), returns early.
            Assert.AreEqual(_manager, LevelManager.Instance);

            Object.DestroyImmediate(go2);
        }

        // ── Initial state ────────────────────────────────────────────────────────

        [Test]
        public void InitialState_IsPlaying()
        {
            Assert.AreEqual(LevelManager.LevelState.Playing, _manager.CurrentState);
        }

        // ── StartLevel ───────────────────────────────────────────────────────────

        [Test]
        public void StartLevel_ResetsStateToPlaying()
        {
            _manager.PauseLevel();
            _manager.StartLevel();
            Assert.AreEqual(LevelManager.LevelState.Playing, _manager.CurrentState);
        }

        [Test]
        public void StartLevel_SetsTimeScaleToOne()
        {
            _manager.PauseLevel(); // sets timeScale to 0
            _manager.StartLevel();
            Assert.AreEqual(1f, Time.timeScale);
        }

        // ── PauseLevel ───────────────────────────────────────────────────────────

        [Test]
        public void PauseLevel_FromPlayingState_TransitionsToPaused()
        {
            _manager.PauseLevel();
            Assert.AreEqual(LevelManager.LevelState.Paused, _manager.CurrentState);
        }

        [Test]
        public void PauseLevel_FreezesTime()
        {
            _manager.PauseLevel();
            Assert.AreEqual(0f, Time.timeScale);
        }

        [Test]
        public void PauseLevel_FromNonPlayingState_DoesNotChangeState()
        {
            // Transition to GameOver first, then verify PauseLevel is a no-op.
            _manager.OnPlayerDefeated();
            _manager.PauseLevel();
            Assert.AreEqual(LevelManager.LevelState.GameOver, _manager.CurrentState);
        }

        // ── ResumeLevel ──────────────────────────────────────────────────────────

        [Test]
        public void ResumeLevel_FromPausedState_TransitionsToPlaying()
        {
            _manager.PauseLevel();
            _manager.ResumeLevel();
            Assert.AreEqual(LevelManager.LevelState.Playing, _manager.CurrentState);
        }

        [Test]
        public void ResumeLevel_RestoresTimeScale()
        {
            _manager.PauseLevel();
            _manager.ResumeLevel();
            Assert.AreEqual(1f, Time.timeScale);
        }

        [Test]
        public void ResumeLevel_FromPlayingState_DoesNotChangeState()
        {
            // Already Playing; ResumeLevel guard should keep state unchanged.
            _manager.ResumeLevel();
            Assert.AreEqual(LevelManager.LevelState.Playing, _manager.CurrentState);
        }

        // ── OnPlayerDefeated ─────────────────────────────────────────────────────

        [Test]
        public void OnPlayerDefeated_TransitionsToGameOver()
        {
            _manager.OnPlayerDefeated();
            Assert.AreEqual(LevelManager.LevelState.GameOver, _manager.CurrentState);
        }

        [Test]
        public void OnPlayerDefeated_CalledTwice_StateRemainsGameOver()
        {
            _manager.OnPlayerDefeated();
            _manager.OnPlayerDefeated(); // idempotent guard
            Assert.AreEqual(LevelManager.LevelState.GameOver, _manager.CurrentState);
        }

        // ── OnLevelCompleted ─────────────────────────────────────────────────────

        [Test]
        public void OnLevelCompleted_TransitionsToCompleted()
        {
            _manager.OnLevelCompleted();
            Assert.AreEqual(LevelManager.LevelState.Completed, _manager.CurrentState);
        }

        [Test]
        public void OnLevelCompleted_FreezesTime()
        {
            _manager.OnLevelCompleted();
            Assert.AreEqual(0f, Time.timeScale);
        }

        [Test]
        public void OnLevelCompleted_CalledTwice_StateRemainsCompleted()
        {
            _manager.OnLevelCompleted();
            _manager.OnLevelCompleted(); // idempotent guard
            Assert.AreEqual(LevelManager.LevelState.Completed, _manager.CurrentState);
        }

        // ── Pause/Resume round-trip ──────────────────────────────────────────────

        [Test]
        public void PauseResume_RoundTrip_StateIsPlaying()
        {
            _manager.PauseLevel();
            _manager.ResumeLevel();
            _manager.PauseLevel();
            _manager.ResumeLevel();
            Assert.AreEqual(LevelManager.LevelState.Playing, _manager.CurrentState);
        }
    }
}
