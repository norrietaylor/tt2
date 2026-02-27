using UnityEngine;
using UnityEngine.Events;

namespace TaekwondoTech.Levels
{
    /// <summary>
    /// LevelManager — manages level state, completion, and scoring.
    /// Tracks collectibles, enemies defeated, and damage taken for star rating.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        [Header("Level Data")]
        [SerializeField] private string levelName;
        [SerializeField] private int totalCollectibles;
        [SerializeField] private int totalEnemies;

        [Header("Star Rating Thresholds")]
        [SerializeField] private float oneStarThreshold = 0.6f;
        [SerializeField] private float twoStarThreshold = 0.8f;
        [SerializeField] private float threeStarThreshold = 1.0f;

        [Header("Events")]
        public UnityEvent<int> OnLevelComplete;
        public UnityEvent OnLevelFailed;

        private int collectiblesGathered;
        private int enemiesDefeated;
        private int damageTaken;
        private bool levelComplete;

        public string LevelName => levelName;
        public int CollectiblesGathered => collectiblesGathered;
        public int TotalCollectibles => totalCollectibles;
        public int EnemiesDefeated => enemiesDefeated;
        public int TotalEnemies => totalEnemies;

        private void Start()
        {
            RegisterEventListeners();
        }

        private void RegisterEventListeners()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var playerHealth = player.GetComponent<Player.PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.OnDamaged.AddListener(OnPlayerDamaged);
                    playerHealth.OnDefeated.AddListener(OnPlayerDefeated);
                }
            }

            var collectibles = FindObjectsOfType<Collectibles.Collectible>();
            foreach (var collectible in collectibles)
            {
                collectible.OnCollected.AddListener(OnCollectibleGathered);
            }

            var enemies = FindObjectsOfType<Enemies.EnemyBase>();
            foreach (var enemy in enemies)
            {
                enemy.OnDefeated.AddListener(OnEnemyDefeated);
            }
        }

        private void OnCollectibleGathered(int value)
        {
            collectiblesGathered++;
        }

        private void OnEnemyDefeated()
        {
            enemiesDefeated++;
        }

        private void OnPlayerDamaged()
        {
            damageTaken++;
        }

        private void OnPlayerDefeated()
        {
            if (!levelComplete)
            {
                OnLevelFailed?.Invoke();
            }
        }

        public void CompleteLevel()
        {
            if (levelComplete) return;

            levelComplete = true;
            int stars = CalculateStarRating();
            OnLevelComplete?.Invoke(stars);
        }

        private int CalculateStarRating()
        {
            float collectibleRatio = totalCollectibles > 0
                ? (float)collectiblesGathered / totalCollectibles
                : 1f;

            float enemyRatio = totalEnemies > 0
                ? (float)enemiesDefeated / totalEnemies
                : 1f;

            float damageScore = Mathf.Clamp01(1f - (damageTaken * 0.1f));

            float overallScore = (collectibleRatio + enemyRatio + damageScore) / 3f;

            if (overallScore >= threeStarThreshold) return 3;
            if (overallScore >= twoStarThreshold) return 2;
            if (overallScore >= oneStarThreshold) return 1;
            return 0;
        }
    }
}
