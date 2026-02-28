using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TaekwondoTech.Levels
{
    /// <summary>
    /// LevelManager — manages level state (playing, paused, completed)
    /// and coordinates level-specific events like game-over and level completion.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        public enum LevelState
        {
            Playing,
            Paused,
            Completed,
            GameOver
        }

        public static LevelManager Instance { get; private set; }

        [Header("Level Configuration")]
        [SerializeField] private string _currentLevelName;

        private LevelState _currentState = LevelState.Playing;

        public LevelState CurrentState => _currentState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            _currentLevelName = SceneManager.GetActiveScene().name;
            StartLevel();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Starts the level. Call this when the level begins.
        /// </summary>
        public void StartLevel()
        {
            _currentState = LevelState.Playing;
            Time.timeScale = 1f;
            Debug.Log($"LevelManager: Level '{_currentLevelName}' started.");
        }

        /// <summary>
        /// Pauses the level. Freezes game time.
        /// </summary>
        public void PauseLevel()
        {
            if (_currentState == LevelState.Playing)
            {
                _currentState = LevelState.Paused;
                Time.timeScale = 0f;
                Debug.Log("LevelManager: Level paused.");
            }
        }

        /// <summary>
        /// Resumes the level from pause.
        /// </summary>
        public void ResumeLevel()
        {
            if (_currentState == LevelState.Paused)
            {
                _currentState = LevelState.Playing;
                Time.timeScale = 1f;
                Debug.Log("LevelManager: Level resumed.");
            }
        }

        /// <summary>
        /// Called when the player is defeated. Triggers game-over flow.
        /// </summary>
        public void OnPlayerDefeated()
        {
            if (_currentState == LevelState.GameOver)
            {
                return;
            }

            _currentState = LevelState.GameOver;
            Debug.Log("LevelManager: Player defeated. Reloading scene...");

            StartCoroutine(ReloadSceneAfterDelay(2f));
        }

        /// <summary>
        /// Called when the level is completed successfully.
        /// </summary>
        public void OnLevelCompleted()
        {
            if (_currentState == LevelState.Completed)
            {
                return;
            }

            _currentState = LevelState.Completed;
            Time.timeScale = 0f;
            Debug.Log("LevelManager: Level completed!");
        }

        private IEnumerator ReloadSceneAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            Time.timeScale = 1f;
            SceneManager.LoadScene(_currentLevelName);
        }

        /// <summary>
        /// Loads a different scene by name.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }
}
