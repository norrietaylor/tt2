using UnityEngine;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// Handles player combat actions (punch, kick, stomp).
    /// Provides state information for animation system.
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        public enum AttackKind
        {
            None = 0,
            Punch = 1,
            Kick = 2,
            Stomp = 3
        }

        [Header("Combat Settings")]
        [SerializeField] private float _attackCooldown = 0.5f;

        private float _lastAttackTime;
        private AttackKind _attackKind;

        /// <summary>
        /// Current attack type as an integer for the Animator parameter.
        /// </summary>
        public int AttackType => (int)_attackKind;
        public bool IsAttacking => _attackKind != AttackKind.None;

        private void Update()
        {
            HandleCombatInput();
            UpdateAttackState();
        }

        private void HandleCombatInput()
        {
            if (Time.time - _lastAttackTime < _attackCooldown)
                return;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                PerformAttack(AttackKind.Punch);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                PerformAttack(AttackKind.Kick);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                PerformAttack(AttackKind.Stomp);
            }
        }

        private void PerformAttack(AttackKind kind)
        {
            _attackKind = kind;
            _lastAttackTime = Time.time;
        }

        private void UpdateAttackState()
        {
            if (_attackKind != AttackKind.None && Time.time - _lastAttackTime >= _attackCooldown)
            {
                _attackKind = AttackKind.None;
            }
        }
    }
}
