using UnityEngine;
using UnityEngine.Events;
using TaekwondoTech.Core;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// Manages player health and damage.
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private int _maxHealth = 3;

        [Header("Events")]
        public UnityEvent<int> OnHealthChanged;
        public UnityEvent OnPlayerDamaged;
        public UnityEvent OnPlayerDeath;

        private int _currentHealth;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public bool IsAlive => _currentHealth > 0;

        // Explicit IDamageable interface implementations
        float IDamageable.Health => _currentHealth;
        float IDamageable.MaxHealth => _maxHealth;
        void IDamageable.TakeDamage(float damage) => TakeDamage(Mathf.RoundToInt(damage));
        void IDamageable.TakeDamage(float damage, GameObject damageSource) => TakeDamage(Mathf.RoundToInt(damage));
        void IDamageable.Heal(float amount) => Heal(Mathf.RoundToInt(amount));

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(_currentHealth);
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive || damage <= 0)
                return;

            _currentHealth -= damage;
            _currentHealth = Mathf.Max(_currentHealth, 0);

            OnHealthChanged?.Invoke(_currentHealth);
            OnPlayerDamaged?.Invoke();

            if (_currentHealth <= 0)
            {
                OnPlayerDeath?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (!IsAlive || amount <= 0)
                return;

            _currentHealth += amount;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);

            OnHealthChanged?.Invoke(_currentHealth);
        }
    }
}
