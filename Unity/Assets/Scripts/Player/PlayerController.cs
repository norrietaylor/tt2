using UnityEngine;
using UnityEngine.Events;
using TaekwondoTech.Input;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// PlayerController — handles horizontal movement and jump physics for the player.
    ///
    /// Features:
    ///   • Smooth acceleration / deceleration (configurable times in seconds)
    ///   • Variable-height gravity-based jump (short tap = low arc, hold = high arc)
    ///   • Double jump (configurable max jump count)
    ///   • All input consumed via InputManager — no direct Input.GetKey calls
    ///   • Fires UnityEvent <see cref="OnLanded"/> on the frame grounding is detected
    ///
    /// Setup in the Inspector:
    ///   1. Attach a <c>Rigidbody2D</c> to the same GameObject (added automatically via RequireComponent).
    ///   2. Create a child Transform called "GroundCheck" positioned just below the feet and
    ///      assign it to the Ground Check field.
    ///   3. Assign the appropriate layers to Ground Layer.
    ///   4. Make sure an <see cref="InputManager"/> exists in the scene (the GameManager
    ///      prefab is the recommended host).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        // ── Inspector – Movement ──────────────────────────────────────────────────
        [Header("Movement")]
        [Tooltip("Maximum horizontal speed in units/second.")]
        [SerializeField] private float _maxSpeed = 8f;

        [Tooltip("Time in seconds to accelerate from 0 to max speed.")]
        [SerializeField] private float _accelerationTime = 0.1f;

        [Tooltip("Time in seconds to decelerate from max speed to 0.")]
        [SerializeField] private float _decelerationTime = 0.08f;

        // ── Inspector – Jump ──────────────────────────────────────────────────────
        [Header("Jump")]
        [Tooltip("Initial upward velocity applied when the jump button is pressed.")]
        [SerializeField] private float _jumpForce = 14f;

        [Tooltip("Multiplier applied to upward velocity when jump is released early (variable height).")]
        [Range(0f, 1f)]
        [SerializeField] private float _jumpCutMultiplier = 0.4f;

        [Tooltip("Gravity scale applied while falling (velocity.y < 0).")]
        [SerializeField] private float _fallGravityScale = 2.5f;

        [Tooltip("Gravity scale applied while rising but the jump button is not held.")]
        [SerializeField] private float _lowJumpGravityScale = 1.5f;

        [Tooltip("Maximum number of jumps (2 = standard double jump).")]
        [Min(1)]
        [SerializeField] private int _maxJumpCount = 2;

        // ── Inspector – Ground Detection ──────────────────────────────────────────
        [Header("Ground Check")]
        [Tooltip("Child Transform positioned just below the player's feet.")]
        [SerializeField] private Transform _groundCheck;

        [Tooltip("Half-extents of the overlap box used for ground detection.")]
        [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.45f, 0.05f);

        [Tooltip("Layer(s) considered as ground.")]
        [SerializeField] private LayerMask _groundLayer;

        // ── Inspector – Events ────────────────────────────────────────────────────
        [Header("Events")]
        [Tooltip("Fired on the first FixedUpdate frame the player is grounded after being airborne.")]
        public UnityEvent OnLanded;

        // ── Public Read-Only Properties ───────────────────────────────────────────
        /// <summary>True while the player is touching the ground.</summary>
        public bool IsGrounded { get; private set; }

        /// <summary>Current Rigidbody2D velocity.</summary>
        public Vector2 Velocity => _rb.velocity;

        /// <summary>True when the player sprite is facing the positive X axis.</summary>
        public bool IsFacingRight { get; private set; } = true;

        // ── Private State ─────────────────────────────────────────────────────────
        private Rigidbody2D _rb;
        private float _horizontalInput;
        private int   _jumpsRemaining;
        private bool  _isJumpHeld;
        private bool  _wasGrounded;

        // Pre-computed force rates (see CacheMovementRates)
        private float _accelerationRate;
        private float _decelerationRate;

        // ── Unity Lifecycle ───────────────────────────────────────────────────────
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            CacheMovementRates();
        }

        private void OnEnable()
        {
            if (InputManager.Instance == null)
            {
                Debug.LogError("[PlayerController] InputManager.Instance is null. " +
                               "Ensure an InputManager exists in the scene before this component enables.");
                return;
            }

            InputManager.Instance.OnMove         += HandleMove;
            InputManager.Instance.OnJumpStarted  += HandleJumpStarted;
            InputManager.Instance.OnJumpCanceled += HandleJumpCanceled;
        }

        private void OnDisable()
        {
            if (InputManager.Instance == null) return;

            InputManager.Instance.OnMove         -= HandleMove;
            InputManager.Instance.OnJumpStarted  -= HandleJumpStarted;
            InputManager.Instance.OnJumpCanceled -= HandleJumpCanceled;
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            ApplyHorizontalMovement();
            ApplyGravityModifiers();
        }

        // ── Input Callbacks ───────────────────────────────────────────────────────
        private void HandleMove(Vector2 input)
        {
            _horizontalInput = input.x;
        }

        private void HandleJumpStarted()
        {
            if (_jumpsRemaining <= 0) return;

            // Reset vertical velocity before a double jump so the arc is consistent
            if (!IsGrounded)
                _rb.velocity = new Vector2(_rb.velocity.x, 0f);

            _rb.velocity    = new Vector2(_rb.velocity.x, _jumpForce);
            _jumpsRemaining--;
            _isJumpHeld     = true;
        }

        private void HandleJumpCanceled()
        {
            _isJumpHeld = false;

            // Variable height: cut vertical speed if still rising
            if (_rb.velocity.y > 0f)
                _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * _jumpCutMultiplier);
        }

        // ── Physics Helpers ───────────────────────────────────────────────────────
        private void ApplyHorizontalMovement()
        {
            float targetVelocity = _horizontalInput * _maxSpeed;
            float speedDiff      = targetVelocity - _rb.velocity.x;
            float rate           = Mathf.Abs(targetVelocity) > 0.01f ? _accelerationRate : _decelerationRate;

            _rb.AddForce(Vector2.right * speedDiff * rate, ForceMode2D.Force);

            // Update facing direction and flip the sprite scale accordingly
            if (_horizontalInput > 0.01f && !IsFacingRight)
                Flip();
            else if (_horizontalInput < -0.01f && IsFacingRight)
                Flip();
        }

        private void ApplyGravityModifiers()
        {
            if (_rb.velocity.y < 0f)
            {
                // Falling — heavier gravity for snappier feel
                _rb.gravityScale = _fallGravityScale;
            }
            else if (_rb.velocity.y > 0f && !_isJumpHeld)
            {
                // Rising but jump released early — medium gravity gives a short arc
                _rb.gravityScale = _lowJumpGravityScale;
            }
            else
            {
                _rb.gravityScale = 1f;
            }
        }

        private void CheckGrounded()
        {
            _wasGrounded = IsGrounded;

            Vector2 checkOrigin = _groundCheck != null
                ? (Vector2)_groundCheck.position
                : (Vector2)transform.position + Vector2.down * 0.5f;

            IsGrounded = Physics2D.OverlapBox(checkOrigin, _groundCheckSize, 0f, _groundLayer);

            if (IsGrounded)
                _jumpsRemaining = _maxJumpCount;

            // Fire OnLanded on the first grounded frame after being airborne
            if (IsGrounded && !_wasGrounded)
                OnLanded?.Invoke();
        }

        private void Flip()
        {
            IsFacingRight         = !IsFacingRight;
            Vector3 s             = transform.localScale;
            s.x                   = -s.x;
            transform.localScale  = s;
        }

        /// <summary>
        /// Converts human-readable acceleration/deceleration times (seconds) into
        /// per-FixedUpdate force magnitudes used by AddForce.
        /// </summary>
        private void CacheMovementRates()
        {
            // Rate derivation: Force = mass * Δv / Δt
            // We target Δv = _maxSpeed in Δt seconds.
            // Using a gain factor so the feel is independent of rb.mass at default mass = 1.
            const float gainFactor = 50f;
            _accelerationRate = gainFactor / Mathf.Max(_accelerationTime, 0.001f);
            _decelerationRate = gainFactor / Mathf.Max(_decelerationTime, 0.001f);
        }

        private void OnValidate()
        {
            CacheMovementRates();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_groundCheck == null) return;
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(_groundCheck.position, new Vector3(_groundCheckSize.x, _groundCheckSize.y, 0f));
        }
#endif
    }
}
