using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// PlayerCombat handles all player attack mechanics: punch, kick, and stomp.
    /// Each attack detects colliders in range and applies damage to IDamageable objects.
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        private const float PUNCH_RANGE = 1f;
        private const float KICK_RANGE = 1.5f;
        private const float PUNCH_COOLDOWN = 0.3f;
        private const float STOMP_BOUNCE_FORCE = 10f;
        private const float STOMP_CHECK_HEIGHT = 0.5f;

        [Header("Combat Settings")]
        [SerializeField] private int _punchDamage = 1;
        [SerializeField] private int _kickDamage = 1;
        [SerializeField] private int _stompDamage = 1;
        [SerializeField] private Vector2 _hitboxSize = new Vector2(0.8f, 0.8f);

        [Header("Components")]
        [SerializeField] private Rigidbody2D _rigidbody2D;

        [Header("Events")]
        public UnityEvent OnPunchPerformed;
        public UnityEvent OnKickPerformed;
        public UnityEvent OnStompPerformed;

        [Header("Debug")]
        [SerializeField] private bool _showDebugGizmos = true;

        private bool _canPunch = true;
        private Coroutine _punchCooldownCoroutine;
        private float _lastAttackTime;

        private void Awake()
        {
            if (_rigidbody2D == null)
            {
                _rigidbody2D = GetComponent<Rigidbody2D>();
            }

            if (OnPunchPerformed == null)
            {
                OnPunchPerformed = new UnityEvent();
            }

            if (OnKickPerformed == null)
            {
                OnKickPerformed = new UnityEvent();
            }

            if (OnStompPerformed == null)
            {
                OnStompPerformed = new UnityEvent();
            }
        }

        private void Start()
        {
            var inputManager = TaekwondoTech.Core.InputManager.Instance;
            if (inputManager != null)
            {
                inputManager.OnPunchInput.AddListener(PerformPunch);
                inputManager.OnKickInput.AddListener(PerformKick);
            }
            else
            {
                Debug.LogWarning("PlayerCombat: InputManager not found. Combat input will not work.");
            }
        }

        private void OnDestroy()
        {
            var inputManager = TaekwondoTech.Core.InputManager.Instance;
            if (inputManager != null)
            {
                inputManager.OnPunchInput.RemoveListener(PerformPunch);
                inputManager.OnKickInput.RemoveListener(PerformKick);
            }
        }

        /// <summary>
        /// Perform a punch attack with short range and cooldown.
        /// </summary>
        public void PerformPunch()
        {
            if (!_canPunch)
            {
                return;
            }

            Vector2 attackPosition = GetAttackPosition(PUNCH_RANGE);
            PerformAttack(attackPosition, _hitboxSize, _punchDamage);

            OnPunchPerformed?.Invoke();
            StartPunchCooldown();
        }

        /// <summary>
        /// Perform a kick attack with medium range.
        /// </summary>
        public void PerformKick()
        {
            Vector2 attackPosition = GetAttackPosition(KICK_RANGE);
            PerformAttack(attackPosition, _hitboxSize, _kickDamage);

            OnKickPerformed?.Invoke();
        }

        /// <summary>
        /// Check for stomp attack when landing on an enemy.
        /// Should be called from collision/trigger detection.
        /// </summary>
        /// <param name="enemy">The collider of the enemy being stomped.</param>
        public void CheckStompAttack(Collider2D enemy)
        {
            if (enemy == null)
            {
                return;
            }

            if (_rigidbody2D != null && _rigidbody2D.velocity.y <= 0)
            {
                Vector2 enemyTop = enemy.bounds.center;
                enemyTop.y = enemy.bounds.max.y;

                float playerBottom = GetComponent<Collider2D>()?.bounds.min.y ?? transform.position.y;

                if (playerBottom >= enemyTop.y - STOMP_CHECK_HEIGHT)
                {
                    var damageable = enemy.GetComponent<Core.IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(_stompDamage);
                        ApplyStompBounce();
                        OnStompPerformed?.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Execute an attack at the specified position, damaging all IDamageable objects in range.
        /// </summary>
        private void PerformAttack(Vector2 position, Vector2 size, int damage)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(position, size, 0f);

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject)
                {
                    continue;
                }

                var damageable = hit.GetComponent<Core.IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }

        /// <summary>
        /// Get the attack position based on player facing direction.
        /// </summary>
        private Vector2 GetAttackPosition(float range)
        {
            Vector2 position = transform.position;
            float direction = transform.localScale.x >= 0 ? 1f : -1f;
            position.x += range * direction;
            return position;
        }

        /// <summary>
        /// Apply upward bounce force when stomping an enemy.
        /// </summary>
        private void ApplyStompBounce()
        {
            if (_rigidbody2D != null)
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, STOMP_BOUNCE_FORCE);
            }
        }

        /// <summary>
        /// Start punch cooldown timer.
        /// </summary>
        private void StartPunchCooldown()
        {
            if (_punchCooldownCoroutine != null)
            {
                StopCoroutine(_punchCooldownCoroutine);
            }

            _punchCooldownCoroutine = StartCoroutine(PunchCooldownCoroutine());
        }

        /// <summary>
        /// Coroutine for punch cooldown timing.
        /// </summary>
        private IEnumerator PunchCooldownCoroutine()
        {
            _canPunch = false;
            yield return new WaitForSeconds(PUNCH_COOLDOWN);
            _canPunch = true;
            _punchCooldownCoroutine = null;
        }

        private void OnDrawGizmosSelected()
        {
            if (!_showDebugGizmos)
            {
                return;
            }

            Gizmos.color = Color.red;
            Vector2 punchPos = GetAttackPosition(PUNCH_RANGE);
            Gizmos.DrawWireCube(punchPos, _hitboxSize);

            Gizmos.color = Color.blue;
            Vector2 kickPos = GetAttackPosition(KICK_RANGE);
            Gizmos.DrawWireCube(kickPos, _hitboxSize);
        }
    }
}
