using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// PlayerHealth manages the player's 3-hit health system with invincibility frames.
    /// Handles damage taking, invincibility flashing, and defeat state.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        private const int MAX_HEALTH = 3;
        private const float INVINCIBILITY_DURATION = 1f;
        private const float FLASH_FREQUENCY = 10f;

        [Header("Health Settings")]
        [SerializeField] private int _currentHealth = MAX_HEALTH;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Events")]
        public UnityEvent<int> OnPlayerDamaged;
        public UnityEvent OnPlayerDefeated;

        private bool _isInvincible = false;
        private bool _isDefeated = false;
        private Coroutine _invincibilityCoroutine;

        public int CurrentHealth => _currentHealth;
        public bool IsInvincible => _isInvincible;
        public bool IsDefeated => _isDefeated;

        private void Awake()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
                if (_spriteRenderer == null)
                {
                    _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                }
            }

            if (OnPlayerDamaged == null)
            {
                OnPlayerDamaged = new UnityEvent<int>();
            }

            if (OnPlayerDefeated == null)
            {
                OnPlayerDefeated = new UnityEvent();
            }
        }

        private void Start()
        {
            _currentHealth = MAX_HEALTH;
        }

        /// <summary>
        /// Apply damage to the player if not invincible or defeated.
        /// </summary>
        /// <param name="amount">Amount of damage to take.</param>
        public void TakeDamage(int amount)
        {
            if (_isInvincible || _isDefeated)
            {
                return;
            }

            _currentHealth -= amount;
            _currentHealth = Mathf.Max(0, _currentHealth);

            OnPlayerDamaged?.Invoke(_currentHealth);

            if (_currentHealth <= 0)
            {
                HandleDefeat();
            }
            else
            {
                StartInvincibility();
            }
        }

        /// <summary>
        /// Start invincibility period with visual flashing.
        /// </summary>
        private void StartInvincibility()
        {
            if (_invincibilityCoroutine != null)
            {
                StopCoroutine(_invincibilityCoroutine);
            }

            _invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
        }

        /// <summary>
        /// Coroutine to handle invincibility frames with sprite flashing.
        /// </summary>
        private IEnumerator InvincibilityCoroutine()
        {
            _isInvincible = true;
            float elapsed = 0f;
            float flashInterval = 1f / (FLASH_FREQUENCY * 2f);

            while (elapsed < INVINCIBILITY_DURATION)
            {
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.enabled = !_spriteRenderer.enabled;
                }

                yield return new WaitForSeconds(flashInterval);
                elapsed += flashInterval;
            }

            if (_spriteRenderer != null)
            {
                _spriteRenderer.enabled = true;
            }

            _isInvincible = false;
            _invincibilityCoroutine = null;
        }

        /// <summary>
        /// Handle player defeat.
        /// </summary>
        private void HandleDefeat()
        {
            if (_isDefeated)
            {
                return;
            }

            _isDefeated = true;

            if (_invincibilityCoroutine != null)
            {
                StopCoroutine(_invincibilityCoroutine);
                _invincibilityCoroutine = null;
            }

            if (_spriteRenderer != null)
            {
                _spriteRenderer.enabled = true;
            }

            OnPlayerDefeated?.Invoke();
        }

        /// <summary>
        /// Reset health to maximum (for respawning/restarting).
        /// </summary>
        public void ResetHealth()
        {
            _currentHealth = MAX_HEALTH;
            _isDefeated = false;
            _isInvincible = false;

            if (_invincibilityCoroutine != null)
            {
                StopCoroutine(_invincibilityCoroutine);
                _invincibilityCoroutine = null;
            }

            if (_spriteRenderer != null)
            {
                _spriteRenderer.enabled = true;
            }
        }
    }
}
