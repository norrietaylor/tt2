using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using TaekwondoTech.Core;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// PlayerHealth manages the player's 3-hit health system with invincibility frames.
    /// Handles damage taking, invincibility flashing, and defeat state.
    /// Implements IDamageable for integration with combat systems.
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        private const int MAX_HEALTH = 3;
        private const float INVINCIBILITY_DURATION = 1f;
        private const float FLASH_FREQUENCY = 10f;

        [Header("Health Settings")]
        [SerializeField] private int _currentHealth = MAX_HEALTH;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Events")]
        public UnityEvent<int> OnHealthChanged;
        public UnityEvent OnPlayerDamaged;
        public UnityEvent OnPlayerDeath;

        private bool _isInvincible = false;
        private bool _isDefeated = false;
        private Coroutine _invincibilityCoroutine;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => MAX_HEALTH;
        public bool IsAlive => _currentHealth > 0 && !_isDefeated;
        public bool IsInvincible => _isInvincible;

        // IDamageable explicit interface implementations
        float IDamageable.Health => _currentHealth;
        float IDamageable.MaxHealth => MAX_HEALTH;
        void IDamageable.TakeDamage(float damage) => TakeDamage(Mathf.RoundToInt(damage));
        void IDamageable.TakeDamage(float damage, GameObject damageSource) => TakeDamage(Mathf.RoundToInt(damage));
        void IDamageable.Heal(float amount) => Heal(Mathf.RoundToInt(amount));

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
        }

        private void Start()
        {
            _currentHealth = Mathf.Clamp(_currentHealth, 0, MAX_HEALTH);
            OnHealthChanged?.Invoke(_currentHealth);
        }

        /// <summary>
        /// Apply damage to the player if not invincible or defeated.
        /// </summary>
        /// <param name="amount">Amount of damage to take.</param>
        public void TakeDamage(int amount)
        {
            if (_isInvincible || _isDefeated || amount <= 0)
            {
                return;
            }

            _currentHealth -= amount;
            _currentHealth = Mathf.Max(0, _currentHealth);

            OnHealthChanged?.Invoke(_currentHealth);
            OnPlayerDamaged?.Invoke();

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
        /// Heal the player by the specified amount.
        /// </summary>
        /// <param name="amount">Amount to heal.</param>
        public void Heal(int amount)
        {
            if (_isDefeated || amount <= 0)
                return;

            _currentHealth = Mathf.Min(_currentHealth + amount, MAX_HEALTH);
            OnHealthChanged?.Invoke(_currentHealth);
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
        /// Coroutine to handle invincibility frames with sprite flashing at 10 Hz.
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
        /// Handle player defeat state.
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

            OnPlayerDeath?.Invoke();
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

            OnHealthChanged?.Invoke(_currentHealth);
        }
    }
}
