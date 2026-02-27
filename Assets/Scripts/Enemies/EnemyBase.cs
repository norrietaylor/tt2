using UnityEngine;
using UnityEngine.Events;

namespace TaekwondoTech.Enemies
{
    /// <summary>
    /// EnemyBase — base class for all enemies, managing health, damage, and state transitions.
    /// Provides events for loose coupling with other systems.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyBase : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] protected int maxHealth = 50;
        [SerializeField] protected int currentHealth;
        [SerializeField] protected int contactDamage = 1;

        [Header("Visual Indicator")]
        [SerializeField] protected GameObject redIndicator;
        [SerializeField] protected Vector3 indicatorOffset = new Vector3(0, 1f, 0);

        [Header("Events")]
        public UnityEvent<int> OnHealthChanged;
        public UnityEvent OnStunned;
        public UnityEvent OnDefeated;

        protected Rigidbody2D rb;
        protected EnemyStateMachine stateMachine;
        protected bool isStunned;
        protected bool isDefeated;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsStunned => isStunned;
        public bool IsDefeated => isDefeated;
        public Rigidbody2D Rigidbody => rb;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            stateMachine = GetComponent<EnemyStateMachine>();
            currentHealth = maxHealth;
        }

        protected virtual void Start()
        {
            if (redIndicator != null)
            {
                redIndicator.transform.localPosition = indicatorOffset;
            }
        }

        public virtual void TakeDamage(int damage)
        {
            if (isDefeated) return;

            currentHealth = Mathf.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                Defeat();
            }
        }

        public virtual void Stun()
        {
            if (isDefeated || isStunned) return;

            isStunned = true;
            OnStunned?.Invoke();

            if (stateMachine != null)
            {
                stateMachine.TransitionToState(EnemyStateType.Stunned);
            }
        }

        public virtual void RecoverFromStun()
        {
            isStunned = false;
        }

        protected virtual void Defeat()
        {
            isDefeated = true;
            OnDefeated?.Invoke();

            if (stateMachine != null)
            {
                stateMachine.TransitionToState(EnemyStateType.Defeated);
            }
        }

        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            if (isDefeated) return;

            var playerHealth = collision.gameObject.GetComponent<Player.PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvincible)
            {
                playerHealth.TakeDamage(contactDamage);
            }
        }
    }
}
