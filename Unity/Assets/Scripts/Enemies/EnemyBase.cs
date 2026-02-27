using UnityEngine;
using UnityEngine.Events;
using TaekwondoTech.Core;
using TaekwondoTech.Enemies.States;

namespace TaekwondoTech.Enemies
{
    /// <summary>
    /// Base class for all enemies in the game.
    /// Implements IDamageable and manages enemy state machine.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyBase : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        [SerializeField] private int _maxHealth = 3;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private float _attackRange = 1f;
        [SerializeField] private int _attackDamage = 1;

        [Header("Patrol")]
        [SerializeField] private Transform _waypointA;
        [SerializeField] private Transform _waypointB;

        [Header("References")]
        [SerializeField] private Transform _player;
        [SerializeField] private GameObject _alertIndicator;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;

        [Header("Events")]
        public UnityEvent OnEnemyDefeated;
        public UnityEvent OnEnemyDamaged;

        private int _currentHealth;
        private EnemyStateMachine _stateMachine;
        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        // Public accessors for states
        public float MoveSpeed => _moveSpeed;
        public float DetectionRadius => _detectionRadius;
        public float AttackRange => _attackRange;
        public int AttackDamage => _attackDamage;
        public Transform WaypointA => _waypointA;
        public Transform WaypointB => _waypointB;
        public Transform Player => _player;
        public GameObject AlertIndicator => _alertIndicator;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Animator Animator => _animator;
        public Rigidbody2D Rigidbody => _rigidbody;
        public Collider2D Collider => _collider;
        public EnemyStateMachine StateMachine => _stateMachine;

        public bool IsAlive => _currentHealth > 0;

        private void Awake()
        {
            _currentHealth = _maxHealth;
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _stateMachine = new EnemyStateMachine();

            // Hide alert indicator initially
            if (_alertIndicator != null)
            {
                _alertIndicator.SetActive(false);
            }

            // Try to find player if not assigned
            if (_player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    _player = playerObj.transform;
                }
            }
        }

        private void Start()
        {
            // Initialize with Idle state
            _stateMachine.ChangeState(new IdleState(this));
        }

        private void Update()
        {
            if (IsAlive)
            {
                _stateMachine.Update();
            }
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            _currentHealth -= damage;
            OnEnemyDamaged?.Invoke();

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _stateMachine.ChangeState(new DefeatedState(this));
                OnEnemyDefeated?.Invoke();
            }
            else
            {
                _stateMachine.ChangeState(new StunnedState(this));
            }
        }

        public void ShowAlertIndicator(bool show)
        {
            if (_alertIndicator != null)
            {
                _alertIndicator.SetActive(show);
            }
        }

        public float GetDistanceToPlayer()
        {
            if (_player == null) return float.MaxValue;
            return Vector2.Distance(transform.position, _player.position);
        }

        public void MoveTowards(Vector2 targetPosition)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            _rigidbody.velocity = new Vector2(direction.x * _moveSpeed, _rigidbody.velocity.y);

            // Flip sprite based on movement direction
            if (_spriteRenderer != null && direction.x != 0)
            {
                _spriteRenderer.flipX = direction.x < 0;
            }
        }

        public void StopMovement()
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw detection radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);

            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);

            // Draw waypoint connections
            if (_waypointA != null && _waypointB != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(_waypointA.position, _waypointB.position);
                Gizmos.DrawWireSphere(_waypointA.position, 0.3f);
                Gizmos.DrawWireSphere(_waypointB.position, 0.3f);
            }
        }
    }
}
