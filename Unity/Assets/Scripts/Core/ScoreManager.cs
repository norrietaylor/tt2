using UnityEngine;
using UnityEngine.Events;

namespace TaekwondoTech.Core
{
    /// <summary>
    /// Manages player score with event-driven updates.
    /// Implemented as a persistent singleton.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Events")]
        public UnityEvent<int> OnScoreChanged;

        private int _currentScore;

        public int CurrentScore => _currentScore;

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
            OnScoreChanged?.Invoke(_currentScore);
        }

        /// <summary>
        /// Add points to the current score.
        /// </summary>
        /// <param name="points">Amount to add (can be negative).</param>
        public void AddScore(int points)
        {
            _currentScore += points;
            _currentScore = Mathf.Max(_currentScore, 0);
            OnScoreChanged?.Invoke(_currentScore);
        }

        /// <summary>
        /// Reset score to zero.
        /// </summary>
        public void ResetScore()
        {
            _currentScore = 0;
            OnScoreChanged?.Invoke(_currentScore);
        }
    }
}
