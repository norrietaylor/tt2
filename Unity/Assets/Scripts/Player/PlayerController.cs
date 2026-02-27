using UnityEngine;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// Controls player movement and provides state information for animations.
    /// Basic controller for Phase 1 - will be expanded in later phases.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpForce = 10f;

        [Header("Ground Check")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask _groundLayer;

        private Rigidbody2D _rigidbody;
        private float _moveInput;
        private bool _isGrounded;

        public float Speed => Mathf.Abs(_rigidbody.velocity.x);
        public bool IsGrounded => _isGrounded;
        public bool IsJumping => !_isGrounded && _rigidbody.velocity.y > 0.1f;
        public bool IsFalling => !_isGrounded && _rigidbody.velocity.y < -0.1f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            CheckGroundStatus();
            HandleInput();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void HandleInput()
        {
            _moveInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                Jump();
            }
        }

        private void Move()
        {
            _rigidbody.velocity = new Vector2(_moveInput * _moveSpeed, _rigidbody.velocity.y);

            if (_moveInput != 0)
            {
                transform.localScale = new Vector3(
                    _moveInput > 0 ? 1 : -1,
                    transform.localScale.y,
                    transform.localScale.z
                );
            }
        }

        private void Jump()
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpForce);
        }

        private void CheckGroundStatus()
        {
            if (_groundCheck == null)
            {
                _isGrounded = false;
                return;
            }

            _isGrounded = Physics2D.OverlapCircle(
                _groundCheck.position,
                _groundCheckRadius,
                _groundLayer
            );
        }

        private void OnDrawGizmosSelected()
        {
            if (_groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
            }
        }
    }
}
