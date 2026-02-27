using UnityEngine;
using TaekwondoTech.Core;

namespace TaekwondoTech.Collectibles
{
    /// <summary>
    /// Coin collectible - adds score when collected.
    /// </summary>
    public class Coin : Collectible
    {
        [Header("Coin Settings")]
        [SerializeField] private int _scoreValue = 10;

        protected override void OnCollectedLogic()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(_scoreValue);
            }
        }
    }
}
