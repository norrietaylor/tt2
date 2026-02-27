using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TaekwondoTech.Core;
using TaekwondoTech.Player;

namespace TaekwondoTech.UI
{
    /// <summary>
    /// Controls the in-game HUD display including health hearts and score.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Health Display")]
        [SerializeField] private Image[] _heartImages;

        [Header("Score Display")]
        [SerializeField] private TMP_Text _scoreText;

        private PlayerHealth _playerHealth;

        private void Start()
        {
            SetupReferences();
            SubscribeToEvents();
            InitializeDisplay();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SetupReferences()
        {
            _playerHealth = FindObjectOfType<PlayerHealth>();

            if (_playerHealth == null)
            {
                Debug.LogWarning("HUDController: PlayerHealth not found in scene.");
            }
        }

        private void SubscribeToEvents()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged.AddListener(UpdateHealthDisplay);
            }

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged.AddListener(UpdateScoreDisplay);
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged.RemoveListener(UpdateHealthDisplay);
            }

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged.RemoveListener(UpdateScoreDisplay);
            }
        }

        private void InitializeDisplay()
        {
            if (_playerHealth != null)
            {
                UpdateHealthDisplay(_playerHealth.CurrentHealth);
            }

            if (ScoreManager.Instance != null)
            {
                UpdateScoreDisplay(ScoreManager.Instance.CurrentScore);
            }
        }

        private void UpdateHealthDisplay(int currentHealth)
        {
            if (_heartImages == null || _heartImages.Length == 0)
            {
                Debug.LogWarning("HUDController: No heart images assigned.");
                return;
            }

            for (int i = 0; i < _heartImages.Length; i++)
            {
                if (_heartImages[i] != null)
                {
                    _heartImages[i].enabled = i < currentHealth;
                }
            }
        }

        private void UpdateScoreDisplay(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Score: {score}";
            }
        }
    }
}
