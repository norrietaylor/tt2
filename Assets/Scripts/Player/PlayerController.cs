using UnityEngine;
using UnityEngine.InputSystem;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// Handles core platformer movement: horizontal movement with smooth
    /// acceleration/deceleration, variable-height jump, double-jump, and
    /// coyote time. Reads from Unity's new Input System via
    /// <see cref="PlayerInputActions"/>.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        // ── Inspector ────────────────────────────────────────────────────────
        [Header("Movement")]
        [SerializeField] private float maxSpeed = 8f;
        [SerializeField] private float acceleration = 60f;
        [SerializeField] private float deceleration = 80f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 16f;
        [SerializeField] private float jumpCutMultiplier = 0.4f;   // variable-height: reduce Y vel on early release
        [SerializeField] private float fallMultiplier = 2.5f;      // faster fall arc
        [SerializeField] private float coyoteTime = 0.1f;
        [SerializeField] private float jumpBufferTime = 0.12f;

        [Header("Ground Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.1f;
        [SerializeField] private LayerMask groundLayer;

        // ── State ─────────────────────────────────────────────────────────
        private Rigidbody2D _rb;
        private PlayerInputActions _inputActions;

        private float _moveInput;
        private bool _jumpHeld;

        private bool _isGrounded;
        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private int _jumpsRemaining;         // 0 = no more jumps; starts at 2 (ground + double)
        private bool _wasGroundedLastFrame;

        /// <summary>True while the player is on the ground.</summary>
        public bool IsGrounded => _isGrounded;

        /// <summary>Current horizontal velocity sign; used by the animator.</summary>
        public float HorizontalVelocity => _rb.linearVelocity.x;

        /// <summary>Current vertical velocity; used by the animator.</summary>
        public float VerticalVelocity => _rb.linearVelocity.y;

        // ── Events ────────────────────────────────────────────────────────
        /// <summary>Fired once on the frame the player lands on the ground.</summary>
        public event System.Action OnLanded;

        // ── Unity lifecycle ───────────────────────────────────────────────
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            _inputActions = new PlayerInputActions();
            _inputActions.Player.Jump.performed += _ => OnJumpPerformed();
            _inputActions.Player.Jump.canceled  += _ => OnJumpCanceled();
        }

        private void OnEnable()  => _inputActions.Player.Enable();
        private void OnDisable() => _inputActions.Player.Disable();

        private void Update()
        {
            ReadMoveInput();
            TickTimers();
            ProcessJumpBuffer();
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            ApplyMovement();
            ApplyGravityModifiers();
        }

        // ── Input ─────────────────────────────────────────────────────────
        private void ReadMoveInput()
        {
            _moveInput = _inputActions.Player.Move.ReadValue<Vector2>().x;
        }

        private void OnJumpPerformed()
        {
            _jumpBufferTimer = jumpBufferTime;
        }

        private void OnJumpCanceled()
        {
            _jumpHeld = false;
            // Variable-height: cut the jump short if the player releases early
            if (_rb.linearVelocity.y > 0)
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x,
                                                 _rb.linearVelocity.y * jumpCutMultiplier);
        }

        // ── Timers ────────────────────────────────────────────────────────
        private void TickTimers()
        {
            if (_jumpBufferTimer > 0f) _jumpBufferTimer -= Time.deltaTime;
            if (_coyoteTimer    > 0f) _coyoteTimer    -= Time.deltaTime;
        }

        // ── Ground check ──────────────────────────────────────────────────
        private void CheckGrounded()
        {
            _wasGroundedLastFrame = _isGrounded;
            _isGrounded = Physics2D.OverlapCircle(
                groundCheck != null ? groundCheck.position : transform.position,
                groundCheckRadius,
                groundLayer);

            if (_isGrounded)
            {
                _coyoteTimer = coyoteTime;
                _jumpsRemaining = 2;

                if (!_wasGroundedLastFrame)
                    OnLanded?.Invoke();
            }
        }

        // ── Movement ──────────────────────────────────────────────────────
        private void ApplyMovement()
        {
            float targetSpeed = _moveInput * maxSpeed;
            float speedDiff   = targetSpeed - _rb.linearVelocity.x;
            float accelRate   = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
            float force       = speedDiff * accelRate;

            _rb.AddForce(new Vector2(force, 0f), ForceMode2D.Force);

            // Flip sprite direction
            if (_moveInput > 0.01f)
                transform.localScale = new Vector3(1f, 1f, 1f);
            else if (_moveInput < -0.01f)
                transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        // ── Jump ──────────────────────────────────────────────────────────
        private void ProcessJumpBuffer()
        {
            if (_jumpBufferTimer > 0f && CanJump())
            {
                PerformJump();
                _jumpBufferTimer = 0f;
            }
        }

        private bool CanJump()
        {
            // Ground or coyote time counts as "first jump available"
            if (_coyoteTimer > 0f && _jumpsRemaining == 2) return true;
            // Double-jump (already used ground jump)
            if (_jumpsRemaining == 1) return true;
            return false;
        }

        private void PerformJump()
        {
            _jumpHeld = true;
            _coyoteTimer = 0f;
            _jumpsRemaining--;

            // Reset vertical velocity for a consistent jump height
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // ── Gravity modifiers ─────────────────────────────────────────────
        private void ApplyGravityModifiers()
        {
            if (_rb.linearVelocity.y < 0f)
            {
                // Faster fall
                _rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                                    * (fallMultiplier - 1f) * Time.fixedDeltaTime;
            }
            else if (_rb.linearVelocity.y > 0f && !_jumpHeld)
            {
                // Short-hop — extra gravity when button released mid-air
                _rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                                    * (jumpCutMultiplier) * Time.fixedDeltaTime;
            }
        }

        // ── Gizmos ────────────────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
