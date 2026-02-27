using UnityEngine;
using UnityEngine.Events;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// PlayerHealth — manages player health with a 3-hit system, invincibility frames,
    /// and defeated state. Integrates with HUD through UnityEvents.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private int currentHealth;

        [Header("Invincibility")]
        [SerializeField] private float invincibilityDuration = 1f;
        [SerializeField] private float flashInterval = 0.1f;

        [Header("Events")]
        public UnityEvent<int> OnHealthChanged;
        public UnityEvent OnDamaged;
        public UnityEvent OnDefeated;

        private bool isInvincible;
        private bool isDefeated;
        private SpriteRenderer spriteRenderer;
        private float invincibilityTimer;
        private float flashTimer;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsDefeated => isDefeated;
        public bool IsInvincible => isInvincible;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            currentHealth = maxHealth;
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(currentHealth);
        }

        private void Update()
        {
            if (isInvincible)
            {
                invincibilityTimer -= Time.deltaTime;
                flashTimer -= Time.deltaTime;

                if (flashTimer <= 0f)
                {
                    flashTimer = flashInterval;
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.enabled = !spriteRenderer.enabled;
                    }
                }

                if (invincibilityTimer <= 0f)
                {
                    isInvincible = false;
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.enabled = true;
                    }
                }
            }
        }

        public void TakeDamage(int damage)
        {
            if (isInvincible || isDefeated) return;

            currentHealth = Mathf.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth);
            OnDamaged?.Invoke();

            if (currentHealth > 0)
            {
                StartInvincibility();
            }
            else
            {
                Defeat();
            }
        }

        public void Heal(int amount)
        {
            if (isDefeated) return;

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth);
        }

        private void StartInvincibility()
        {
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
            flashTimer = 0f;
        }

        private void Defeat()
        {
            isDefeated = true;
            OnDefeated?.Invoke();
        }

        public void Respawn()
        {
            currentHealth = maxHealth;
            isDefeated = false;
            isInvincible = false;
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
}
