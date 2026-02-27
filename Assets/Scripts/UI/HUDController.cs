using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TaekwondoTech.UI
{
    /// <summary>
    /// HUDController — manages in-game heads-up display showing health hearts,
    /// score, and other gameplay information.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Health Display")]
        [SerializeField] private GameObject[] healthHearts;

        [Header("Score Display")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI coinText;

        private int currentScore;
        private int currentCoins;

        private void Start()
        {
            UpdateScoreDisplay();
            UpdateCoinDisplay();
        }

        public void UpdateHealth(int currentHealth)
        {
            if (healthHearts == null) return;

            for (int i = 0; i < healthHearts.Length; i++)
            {
                if (healthHearts[i] != null)
                {
                    healthHearts[i].SetActive(i < currentHealth);
                }
            }
        }

        public void AddScore(int points)
        {
            currentScore += points;
            UpdateScoreDisplay();
        }

        public void AddCoins(int amount)
        {
            currentCoins += amount;
            UpdateCoinDisplay();
        }

        private void UpdateScoreDisplay()
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {currentScore}";
            }
        }

        private void UpdateCoinDisplay()
        {
            if (coinText != null)
            {
                coinText.text = $"Coins: {currentCoins}";
            }
        }

        public void ResetHUD()
        {
            currentScore = 0;
            currentCoins = 0;
            UpdateScoreDisplay();
            UpdateCoinDisplay();
        }
    }
}
