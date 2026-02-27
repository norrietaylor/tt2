using UnityEngine;

namespace TaekwondoTech.Core
{
    /// <summary>
    /// GameManager — central entry point for game state, scene transitions,
    /// and top-level system initialization. Implemented as a persistent singleton.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // TODO: Initialize subsystems (AudioManager, SaveSystem, etc.)
        }

        /// <summary>
        /// Load a scene by name. All scene transitions should go through here.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("GameManager.LoadScene: sceneName is null or empty.");
                return;
            }

            if (UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath(sceneName) < 0)
            {
                Debug.LogError($"GameManager.LoadScene: scene '{sceneName}' is not in the build settings.");
                return;
            }

            // TODO: Add loading screen / transition logic
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Quit the application (or exit play mode in the editor).
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
