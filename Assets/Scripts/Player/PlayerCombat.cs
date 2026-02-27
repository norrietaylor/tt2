using UnityEngine;
using UnityEngine.Events;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// PlayerCombat — handles punch (short range), kick (medium range), and head stomp attacks.
    /// Invokes UnityEvents for loose coupling with other systems.
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Punch")]
        [SerializeField] private float punchRange = 1f;
        [SerializeField] private int punchDamage = 10;
        [SerializeField] private float punchCooldown = 0.3f;

        [Header("Kick")]
        [SerializeField] private float kickRange = 1.5f;
        [SerializeField] private int kickDamage = 15;
        [SerializeField] private float kickCooldown = 0.5f;

        [Header("Stomp")]
        [SerializeField] private float stompRange = 0.8f;
        [SerializeField] private int stompDamage = 20;
        [SerializeField] private float stompBounceForce = 10f;

        [Header("Detection")]
        [SerializeField] private Transform attackPoint;
        [SerializeField] private LayerMask enemyLayer;

        [Header("Events")]
        public UnityEvent OnPunch;
        public UnityEvent OnKick;
        public UnityEvent OnStomp;

        private Input.InputManager inputManager;
        private PlayerController playerController;
        private Rigidbody2D rb;
        private float lastPunchTime;
        private float lastKickTime;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            inputManager = Input.InputManager.Instance;
        }

        private void Update()
        {
            if (inputManager == null) return;

            if (inputManager.PunchPressed && Time.time >= lastPunchTime + punchCooldown)
            {
                PerformPunch();
            }

            if (inputManager.KickPressed && Time.time >= lastKickTime + kickCooldown)
            {
                PerformKick();
            }

            CheckStomp();
        }

        private void PerformPunch()
        {
            lastPunchTime = Time.time;
            OnPunch?.Invoke();

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, punchRange, enemyLayer);
            foreach (var enemy in hitEnemies)
            {
                var enemyHealth = enemy.GetComponent<Enemies.EnemyBase>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(punchDamage);
                }
            }
        }

        private void PerformKick()
        {
            lastKickTime = Time.time;
            OnKick?.Invoke();

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, kickRange, enemyLayer);
            foreach (var enemy in hitEnemies)
            {
                var enemyHealth = enemy.GetComponent<Enemies.EnemyBase>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(kickDamage);
                }
            }
        }

        private void CheckStomp()
        {
            if (!playerController.IsGrounded && rb.velocity.y < 0)
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, stompRange, enemyLayer);
                foreach (var enemy in hitEnemies)
                {
                    if (enemy.transform.position.y < transform.position.y)
                    {
                        var enemyHealth = enemy.GetComponent<Enemies.EnemyBase>();
                        if (enemyHealth != null && !enemyHealth.IsStunned)
                        {
                            enemyHealth.TakeDamage(stompDamage);
                            enemyHealth.Stun();
                            rb.velocity = new Vector2(rb.velocity.x, stompBounceForce);
                            OnStomp?.Invoke();
                        }
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, punchRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, kickRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, stompRange);
        }
    }
}
