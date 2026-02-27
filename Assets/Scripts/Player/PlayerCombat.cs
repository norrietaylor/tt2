using System.Collections;
using UnityEngine;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// Handles melee combat: punch, kick, and stomp. Each attack uses a
    /// Physics2D overlap to detect enemies and applies knockback. Audio clips
    /// are optional placeholders — real assets will be added later.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PlayerCombat : MonoBehaviour
    {
        // ── Inspector ────────────────────────────────────────────────────────
        [Header("Punch")]
        [SerializeField] private Vector2 punchOffset = new Vector2(0.6f, 0f);
        [SerializeField] private Vector2 punchSize   = new Vector2(0.5f, 0.4f);
        [SerializeField] private float   punchDamage    = 10f;
        [SerializeField] private float   punchKnockback  = 5f;
        [SerializeField] private float   punchCooldown   = 0.35f;
        [SerializeField] private AudioClip punchClip;

        [Header("Kick")]
        [SerializeField] private Vector2 kickOffset = new Vector2(0.5f, -0.1f);
        [SerializeField] private Vector2 kickSize   = new Vector2(0.7f, 0.55f);
        [SerializeField] private float   kickDamage    = 15f;
        [SerializeField] private float   kickKnockback  = 7f;
        [SerializeField] private float   kickCooldown   = 0.5f;
        [SerializeField] private AudioClip kickClip;

        [Header("Stomp")]
        [SerializeField] private float stompBounceForce = 14f;
        [SerializeField] private float stompDamage      = 20f;
        [SerializeField] private float stompFallingThreshold = -1f;  // stomp only when falling faster than this value
        [SerializeField] private AudioClip stompClip;

        [Header("Enemy Detection")]
        [SerializeField] private LayerMask enemyLayer;

        // ── References ────────────────────────────────────────────────────
        private PlayerInputActions _inputActions;
        private Rigidbody2D        _rb;
        private AudioSource        _audioSource;

        // ── Cooldown state ────────────────────────────────────────────────
        private float _punchCooldownTimer;
        private float _kickCooldownTimer;

        // ── Events ────────────────────────────────────────────────────────
        /// <summary>Fired after any attack; passes the attack name string.</summary>
        public event System.Action<string> OnAttack;

        // ── Unity lifecycle ───────────────────────────────────────────────
        private void Awake()
        {
            _rb          = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();

            _inputActions = new PlayerInputActions();
            _inputActions.Player.Punch.performed += _ => TryPunch();
            _inputActions.Player.Kick.performed  += _ => TryKick();
        }

        private void OnEnable()  => _inputActions.Player.Enable();
        private void OnDisable() => _inputActions.Player.Disable();

        private void Update()
        {
            if (_punchCooldownTimer > 0f) _punchCooldownTimer -= Time.deltaTime;
            if (_kickCooldownTimer  > 0f) _kickCooldownTimer  -= Time.deltaTime;
        }

        // ── Attacks ───────────────────────────────────────────────────────
        /// <summary>Attempt a punch. Ignored if on cooldown.</summary>
        public void TryPunch()
        {
            if (_punchCooldownTimer > 0f) return;
            _punchCooldownTimer = punchCooldown;
            PlayClip(punchClip);
            OnAttack?.Invoke("Punch");
            HitEnemiesInBox(GetWorldOffset(punchOffset), punchSize, punchDamage, punchKnockback);
        }

        /// <summary>Attempt a kick. Ignored if on cooldown.</summary>
        public void TryKick()
        {
            if (_kickCooldownTimer > 0f) return;
            _kickCooldownTimer = kickCooldown;
            PlayClip(kickClip);
            OnAttack?.Invoke("Kick");
            HitEnemiesInBox(GetWorldOffset(kickOffset), kickSize, kickDamage, kickKnockback);
        }

        // ── Stomp detection (called from OnTriggerEnter2D by a foot collider
        //    on the player, or checked here each FixedUpdate) ───────────────
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!IsOnEnemyLayer(col.gameObject)) return;

            // Only trigger stomp when falling downward onto the enemy
            if (_rb.linearVelocity.y > stompFallingThreshold) return;

            // Make sure we are above the enemy's centre
            if (transform.position.y <= col.transform.position.y) return;

            ExecuteStomp(col.gameObject);
        }

        private void ExecuteStomp(GameObject enemy)
        {
            PlayClip(stompClip);
            OnAttack?.Invoke("Stomp");

            ApplyDamage(enemy, stompDamage);

            // Bounce the player upward (Mario-style)
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
            _rb.AddForce(Vector2.up * stompBounceForce, ForceMode2D.Impulse);
        }

        // ── Helpers ───────────────────────────────────────────────────────
        private void HitEnemiesInBox(Vector2 worldCenter, Vector2 size,
                                     float damage, float knockback)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(worldCenter, size, 0f, enemyLayer);
            foreach (Collider2D hit in hits)
            {
                ApplyDamage(hit.gameObject, damage);
                ApplyKnockback(hit.gameObject, knockback);
            }
        }

        /// <summary>
        /// Flips the hitbox offset with the player's facing direction so
        /// attacks always fire "forward".
        /// </summary>
        private Vector2 GetWorldOffset(Vector2 localOffset)
        {
            float facingSign = transform.localScale.x >= 0f ? 1f : -1f;
            return (Vector2)transform.position
                 + new Vector2(localOffset.x * facingSign, localOffset.y);
        }

        private static void ApplyDamage(GameObject target, float amount)
        {
            // Attempt to use a generic IDamageable interface; fall back gracefully.
            var damageable = target.GetComponent<IDamageable>();
            damageable?.TakeDamage(amount);
        }

        private void ApplyKnockback(GameObject target, float force)
        {
            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            if (targetRb == null) return;

            Vector2 direction = ((Vector2)(target.transform.position - transform.position)).normalized;
            targetRb.AddForce(direction * force, ForceMode2D.Impulse);
        }

        private bool IsOnEnemyLayer(GameObject go) =>
            ((1 << go.layer) & enemyLayer) != 0;

        private void PlayClip(AudioClip clip)
        {
            if (clip != null && _audioSource != null)
                _audioSource.PlayOneShot(clip);
        }

        // ── Gizmos ────────────────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            float facingSign = Application.isPlaying
                ? (transform.localScale.x >= 0f ? 1f : -1f)
                : 1f;

            // Punch box
            Gizmos.color = Color.yellow;
            Vector2 pWorld = (Vector2)transform.position
                           + new Vector2(punchOffset.x * facingSign, punchOffset.y);
            Gizmos.DrawWireCube(pWorld, punchSize);

            // Kick box
            Gizmos.color = Color.cyan;
            Vector2 kWorld = (Vector2)transform.position
                           + new Vector2(kickOffset.x * facingSign, kickOffset.y);
            Gizmos.DrawWireCube(kWorld, kickSize);
        }
    }

    /// <summary>
    /// Minimal damage interface. Enemy scripts should implement this.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
}
