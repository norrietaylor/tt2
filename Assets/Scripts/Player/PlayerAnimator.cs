using UnityEngine;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// Drives the Animator component on the Player GameObject by reading state
    /// from <see cref="PlayerController"/> and <see cref="PlayerCombat"/>.
    /// Animator parameter names are defined as constants to prevent typos.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAnimator : MonoBehaviour
    {
        // ── Animator parameter name constants ─────────────────────────────
        private static readonly int SpeedHash    = Animator.StringToHash("Speed");
        private static readonly int VelocityYHash = Animator.StringToHash("VelocityY");
        private static readonly int IsGroundedHash=Animator.StringToHash("IsGrounded");
        private static readonly int PunchHash    = Animator.StringToHash("Punch");
        private static readonly int KickHash     = Animator.StringToHash("Kick");
        private static readonly int StompHash    = Animator.StringToHash("Stomp");

        // ── References ────────────────────────────────────────────────────
        private Animator         _animator;
        private PlayerController _controller;
        private PlayerCombat     _combat;

        // ── Unity lifecycle ───────────────────────────────────────────────
        private void Awake()
        {
            _animator   = GetComponent<Animator>();
            _controller = GetComponent<PlayerController>();
            _combat     = GetComponent<PlayerCombat>();
        }

        private void OnEnable()
        {
            if (_combat != null)
                _combat.OnAttack += HandleAttackAnimation;

            if (_controller != null)
                _controller.OnLanded += HandleLanded;
        }

        private void OnDisable()
        {
            if (_combat != null)
                _combat.OnAttack -= HandleAttackAnimation;

            if (_controller != null)
                _controller.OnLanded -= HandleLanded;
        }

        private void Update()
        {
            if (_controller == null) return;

            // Continuous blend-tree parameters
            _animator.SetFloat(SpeedHash,     Mathf.Abs(_controller.HorizontalVelocity));
            _animator.SetFloat(VelocityYHash, _controller.VerticalVelocity);
            _animator.SetBool(IsGroundedHash, _controller.IsGrounded);
        }

        // ── Event handlers ────────────────────────────────────────────────
        private void HandleAttackAnimation(string attackName)
        {
            switch (attackName)
            {
                case "Punch": _animator.SetTrigger(PunchHash); break;
                case "Kick":  _animator.SetTrigger(KickHash);  break;
                case "Stomp": _animator.SetTrigger(StompHash); break;
            }
        }

        private void HandleLanded()
        {
            // Any landing VFX / audio hook can go here via an animation event.
        }
    }
}
