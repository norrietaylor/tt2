using UnityEngine;

namespace TaekwondoTech.Player
{
    /// <summary>
    /// Handles player combat actions (punch, kick, stomp).
    /// Provides state information for animation system.
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Combat Settings")]
        [SerializeField] private float _attackCooldown = 0.5f;

        private float _lastAttackTime;
        private int _attackType; // 0=none, 1=punch, 2=kick, 3=stomp

        public int AttackType => _attackType;
        public bool IsAttacking => _attackType != 0;

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
                PerformAttack(1); // Punch
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                PerformAttack(2); // Kick
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                PerformAttack(3); // Stomp
            }
        }

        private void PerformAttack(int attackType)
        {
            _attackType = attackType;
            _lastAttackTime = Time.time;
        }

        private void UpdateAttackState()
        {
            if (_attackType != 0 && Time.time - _lastAttackTime >= _attackCooldown)
            {
                _attackType = 0;
            }
        }
    }
}
