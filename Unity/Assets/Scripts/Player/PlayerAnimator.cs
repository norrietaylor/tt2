using UnityEngine;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// Drives animator parameters based on PlayerController and PlayerCombat state.
    /// Bridges the gap between game logic and animation system.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator _animator;
        private PlayerController _controller;
        private PlayerCombat _combat;
        private PlayerHealth _health;

        // Animator parameter names
        private static readonly int SPEED = Animator.StringToHash("Speed");
        private static readonly int IS_GROUNDED = Animator.StringToHash("IsGrounded");
        private static readonly int IS_JUMPING = Animator.StringToHash("IsJumping");
        private static readonly int ATTACK_TYPE = Animator.StringToHash("AttackType");
        private static readonly int IS_HURT = Animator.StringToHash("IsHurt");
        private static readonly int IS_DEFEATED = Animator.StringToHash("IsDefeated");

        private bool _isHurt;
        private float _hurtTimer;
        private const float HURT_DURATION = 0.5f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _controller = GetComponent<PlayerController>();
            _combat = GetComponent<PlayerCombat>();
            _health = GetComponent<PlayerHealth>();
        }

        private void OnEnable()
        {
            if (_health != null)
            {
                _health.OnPlayerDamaged.AddListener(OnPlayerHurt);
                _health.OnPlayerDeath.AddListener(OnPlayerDefeat);
            }
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _health.OnPlayerDamaged.RemoveListener(OnPlayerHurt);
                _health.OnPlayerDeath.RemoveListener(OnPlayerDefeat);
            }
        }

        private void Update()
        {
            if (_animator == null)
                return;

            UpdateHurtState();
            UpdateAnimatorParameters();
        }

        private void UpdateAnimatorParameters()
        {
            if (_controller != null)
            {
                _animator.SetFloat(SPEED, _controller.Speed);
                _animator.SetBool(IS_GROUNDED, _controller.IsGrounded);
                _animator.SetBool(IS_JUMPING, _controller.IsJumping);
            }

            if (_combat != null)
            {
                _animator.SetInteger(ATTACK_TYPE, _combat.AttackType);
            }

            _animator.SetBool(IS_HURT, _isHurt);

            if (_health != null)
            {
                _animator.SetBool(IS_DEFEATED, !_health.IsAlive);
            }
        }

        private void UpdateHurtState()
        {
            if (_isHurt)
            {
                _hurtTimer += Time.deltaTime;
                if (_hurtTimer >= HURT_DURATION)
                {
                    _isHurt = false;
                    _hurtTimer = 0f;
                }
            }
        }

        private void OnPlayerHurt()
        {
            _isHurt = true;
            _hurtTimer = 0f;
        }

        private void OnPlayerDefeat()
        {
            _isHurt = false;
        }
    }
}
