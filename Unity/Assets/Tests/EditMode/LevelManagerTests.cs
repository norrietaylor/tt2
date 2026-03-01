using NUnit.Framework;
using UnityEngine;
using TaekwondoTech.Levels;

namespace TaekwondoTech.Tests.EditMode
{
    /// <summary>
    /// EditMode unit tests for LevelManager state-machine logic.
    /// Tests exercise all public state transitions without requiring Play mode or scene loads.
    /// </summary>
    public class LevelManagerTests
    {
        private GameObject _go;
        private LevelManager _levelManager;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("LevelManager_Test");
            _levelManager = _go.AddComponent<LevelManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Time.timeScale = 1f;
            Object.DestroyImmediate(_go);
        }

        // ── Initial state ──────────────────────────────────────────────────────

        [Test]
        public void InitialState_IsPlaying()
        {
            Assert.AreEqual(LevelManager.LevelState.Playing, _levelManager.CurrentState);
        }

        // ── StartLevel ─────────────────────────────────────────────────────────

        [Test]
        public void StartLevel_SetsState_ToPlaying()
        {
            _levelManager.PauseLevel();
            _levelManager.StartLevel();
            Assert.AreEqual(LevelManager.LevelState.Playing, _levelManager.CurrentState);
        }

        [Test]
        public void StartLevel_SetsTimeScale_ToOne()
        {
            _levelManager.PauseLevel();
            _levelManager.StartLevel();
            Assert.AreEqual(1f, Time.timeScale);
        }

        // ── PauseLevel ─────────────────────────────────────────────────────────

        [Test]
        public void PauseLevel_WhenPlaying_SetsState_ToPaused()
        {
            _levelManager.PauseLevel();
            Assert.AreEqual(LevelManager.LevelState.Paused, _levelManager.CurrentState);
        }

        [Test]
        public void PauseLevel_WhenPlaying_SetsTimeScale_ToZero()
        {
            _levelManager.PauseLevel();
            Assert.AreEqual(0f, Time.timeScale);
        }

        [Test]
        public void PauseLevel_WhenAlreadyPaused_DoesNotChangeState()
        {
            _levelManager.PauseLevel();
            _levelManager.PauseLevel();
            Assert.AreEqual(LevelManager.LevelState.Paused, _levelManager.CurrentState);
        }

        [Test]
        public void PauseLevel_WhenGameOver_DoesNotChangeState()
        {
            _levelManager.OnPlayerDefeated();
            _levelManager.PauseLevel();
            Assert.AreEqual(LevelManager.LevelState.GameOver, _levelManager.CurrentState);
        }

        [Test]
        public void PauseLevel_WhenCompleted_DoesNotChangeState()
        {
            _levelManager.OnLevelCompleted();
            _levelManager.PauseLevel();
            Assert.AreEqual(LevelManager.LevelState.Completed, _levelManager.CurrentState);
        }

        // ── ResumeLevel ────────────────────────────────────────────────────────

        [Test]
        public void ResumeLevel_WhenPaused_SetsState_ToPlaying()
        {
            _levelManager.PauseLevel();
            _levelManager.ResumeLevel();
            Assert.AreEqual(LevelManager.LevelState.Playing, _levelManager.CurrentState);
        }

        [Test]
        public void ResumeLevel_WhenPaused_SetsTimeScale_ToOne()
        {
            _levelManager.PauseLevel();
            _levelManager.ResumeLevel();
            Assert.AreEqual(1f, Time.timeScale);
        }

        [Test]
        public void ResumeLevel_WhenNotPaused_DoesNotChangeState()
        {
            // State is already Playing — ResumeLevel should be a no-op
            _levelManager.ResumeLevel();
            Assert.AreEqual(LevelManager.LevelState.Playing, _levelManager.CurrentState);
        }

        // ── OnPlayerDefeated ───────────────────────────────────────────────────

        [Test]
        public void OnPlayerDefeated_WhenPlaying_SetsState_ToGameOver()
        {
            _levelManager.OnPlayerDefeated();
            Assert.AreEqual(LevelManager.LevelState.GameOver, _levelManager.CurrentState);
        }

        [Test]
        public void OnPlayerDefeated_WhenGameOver_IsIdempotent()
        {
            _levelManager.OnPlayerDefeated();
            _levelManager.OnPlayerDefeated();
            Assert.AreEqual(LevelManager.LevelState.GameOver, _levelManager.CurrentState);
        }

        [Test]
        public void OnPlayerDefeated_WhenPaused_SetsState_ToGameOver()
        {
            // Game-over can happen from a paused state (e.g., timeout during pause screen)
            _levelManager.PauseLevel();
            _levelManager.OnPlayerDefeated();
            Assert.AreEqual(LevelManager.LevelState.GameOver, _levelManager.CurrentState);
        }

        // ── OnLevelCompleted ───────────────────────────────────────────────────

        [Test]
        public void OnLevelCompleted_WhenPlaying_SetsState_ToCompleted()
        {
            _levelManager.OnLevelCompleted();
            Assert.AreEqual(LevelManager.LevelState.Completed, _levelManager.CurrentState);
        }

        [Test]
        public void OnLevelCompleted_WhenPlaying_SetsTimeScale_ToZero()
        {
            _levelManager.OnLevelCompleted();
            Assert.AreEqual(0f, Time.timeScale);
        }

        [Test]
        public void OnLevelCompleted_WhenCompleted_IsIdempotent()
        {
            _levelManager.OnLevelCompleted();
            _levelManager.OnLevelCompleted();
            Assert.AreEqual(LevelManager.LevelState.Completed, _levelManager.CurrentState);
        }

        // ── Singleton ──────────────────────────────────────────────────────────

        [Test]
        public void Singleton_Instance_IsSetAfterAwake()
        {
            Assert.IsNotNull(LevelManager.Instance);
            Assert.AreSame(_levelManager, LevelManager.Instance);
        }

        [Test]
        public void Singleton_SecondInstance_DoesNotReplaceFirst()
        {
            var go2 = new GameObject("LevelManager_Duplicate");
            try
            {
                go2.AddComponent<LevelManager>();
                // Awake on the second LevelManager queues Destroy(go2); Instance must remain the first
                Assert.AreSame(_levelManager, LevelManager.Instance);
            }
            finally
            {
                Object.DestroyImmediate(go2);
            }
        }
    }
}
