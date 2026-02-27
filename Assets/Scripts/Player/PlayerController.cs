using UnityEngine;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// PlayerController — handles horizontal movement with acceleration curves,
    /// gravity-based variable-height jumping, and double jump functionality.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float deceleration = 50f;
        [SerializeField] private float airControl = 0.7f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 15f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;
        [SerializeField] private int maxJumps = 2;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        private Rigidbody2D rb;
        private Input.InputManager inputManager;
        private int jumpsRemaining;
        private bool isGrounded;
        private float currentVelocityX;

        public bool IsGrounded => isGrounded;
        public Vector2 Velocity => rb.velocity;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            inputManager = Input.InputManager.Instance;
            jumpsRemaining = maxJumps;
        }

        private void Update()
        {
            CheckGrounded();
            HandleJump();
        }

        private void FixedUpdate()
        {
            HandleMovement();
            ApplyJumpPhysics();
        }

        private void CheckGrounded()
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if (isGrounded && !wasGrounded)
            {
                jumpsRemaining = maxJumps;
            }
        }

        private void HandleMovement()
        {
            if (inputManager == null) return;

            float targetVelocity = inputManager.MoveInput.x * moveSpeed;
            float accelRate = (Mathf.Abs(targetVelocity) > 0.01f) ? acceleration : deceleration;

            if (!isGrounded)
            {
                accelRate *= airControl;
            }

            currentVelocityX = Mathf.MoveTowards(currentVelocityX, targetVelocity, accelRate * Time.fixedDeltaTime);
            rb.velocity = new Vector2(currentVelocityX, rb.velocity.y);

            if (Mathf.Abs(currentVelocityX) > 0.1f)
            {
                transform.localScale = new Vector3(Mathf.Sign(currentVelocityX), 1f, 1f);
            }
        }

        private void HandleJump()
        {
            if (inputManager == null) return;

            if (inputManager.JumpPressed && jumpsRemaining > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpsRemaining--;
            }
        }

        private void ApplyJumpPhysics()
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (rb.velocity.y > 0 && (inputManager == null || !inputManager.JumpPressed))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }
    }
}
