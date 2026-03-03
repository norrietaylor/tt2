using NUnit.Framework;
using TaekwondoTech.Levels;
using UnityEngine;

namespace TaekwondoTech.Tests.EditMode.Levels
{
    /// <summary>
    /// EditMode tests for LevelManager state machine.
    /// Awake() runs via AddComponent; Start() does not — so _currentLevelName
    /// is empty and StartLevel() is NOT called automatically in these tests.
    /// </summary>
    public class LevelManagerTests
    {
        private GameObject _go;
        private LevelManager _lm;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("LevelManager");
            _lm = _go.AddComponent<LevelManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Time.timeScale = 1f;
            if (_go != null)
                Object.DestroyImmediate(_go);
        }

        // ── Initial state ─────────────────────────────────────────────────────

        [Test]
        public void InitialState_IsPlaying()
        {
            Assert.AreEqual(LevelManager.LevelState.Playing, _lm.CurrentState);
        }

        [Test]
        public void Instance_IsSetAfterAwake()
        {
            Assert.IsNotNull(LevelManager.Instance);
            Assert.AreEqual(_lm, LevelManager.Instance);
        }

        [Test]
        public void Instance_IsNullAfterDestroy()
        {
            Object.DestroyImmediate(_go);
            _go = null; // prevent double-destroy in TearDown
            Assert.IsNull(LevelManager.Instance);
        }

        // ── PauseLevel ────────────────────────────────────────────────────────

        [Test]
        public void PauseLevel_WhenPlaying_SetsStateToPaused()
        {
            _lm.PauseLevel();
            Assert.AreEqual(LevelManager.LevelState.Paused, _lm.CurrentState);
        }

        [Test]
        public void PauseLevel_WhenPlaying_SetsTimeScaleToZero()
        {
            _lm.PauseLevel();
            Assert.AreEqual(0f, Time.timeScale);
        }

        [Test]
        public void PauseLevel_WhenAlreadyPaused_StateRemainsUnchanged()
        {
            _lm.PauseLevel();
            _lm.PauseLevel(); // second call ignored (guard: only acts on Playing)
            Assert.AreEqual(LevelManager.LevelState.Paused, _lm.CurrentState);
        }

        // ── ResumeLevel ───────────────────────────────────────────────────────

        [Test]
        public void ResumeLevel_WhenPaused_SetsStateToPlaying()
        {
            _lm.PauseLevel();
            _lm.ResumeLevel();
            Assert.AreEqual(LevelManager.LevelState.Playing, _lm.CurrentState);
        }

        [Test]
        public void ResumeLevel_WhenPaused_RestoresTimeScaleToOne()
        {
            _lm.PauseLevel();
            _lm.ResumeLevel();
            Assert.AreEqual(1f, Time.timeScale);
        }

        [Test]
        public void ResumeLevel_WhenNotPaused_StateRemainsPlaying()
        {
            _lm.ResumeLevel(); // state is Playing — guard should no-op
            Assert.AreEqual(LevelManager.LevelState.Playing, _lm.CurrentState);
        }

        // ── OnPlayerDefeated ──────────────────────────────────────────────────

        [Test]
        public void OnPlayerDefeated_WhenPlaying_SetsStateToGameOver()
        {
            _lm.OnPlayerDefeated();
            Assert.AreEqual(LevelManager.LevelState.GameOver, _lm.CurrentState);
        }

        [Test]
        public void OnPlayerDefeated_WhenAlreadyGameOver_StateRemainsGameOver()
        {
            _lm.OnPlayerDefeated();
            _lm.OnPlayerDefeated(); // second call ignored
            Assert.AreEqual(LevelManager.LevelState.GameOver, _lm.CurrentState);
        }

        // ── OnLevelCompleted ──────────────────────────────────────────────────

        [Test]
        public void OnLevelCompleted_WhenPlaying_SetsStateToCompleted()
        {
            _lm.OnLevelCompleted();
            Assert.AreEqual(LevelManager.LevelState.Completed, _lm.CurrentState);
        }

        [Test]
        public void OnLevelCompleted_WhenPlaying_SetsTimeScaleToZero()
        {
            _lm.OnLevelCompleted();
            Assert.AreEqual(0f, Time.timeScale);
        }

        [Test]
        public void OnLevelCompleted_WhenAlreadyCompleted_StateRemainsCompleted()
        {
            _lm.OnLevelCompleted();
            _lm.OnLevelCompleted(); // second call ignored
            Assert.AreEqual(LevelManager.LevelState.Completed, _lm.CurrentState);
        }

        // ── StartLevel ────────────────────────────────────────────────────────

        [Test]
        public void StartLevel_SetsStateToPlaying()
        {
            _lm.PauseLevel();
            _lm.StartLevel();
            Assert.AreEqual(LevelManager.LevelState.Playing, _lm.CurrentState);
        }

        [Test]
        public void StartLevel_SetsTimeScaleToOne()
        {
            _lm.PauseLevel(); // timeScale → 0
            _lm.StartLevel();
            Assert.AreEqual(1f, Time.timeScale);
        }
    }
}
