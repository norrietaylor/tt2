using UnityEngine;
using TaekwondoTech.Core;

namespace TaekwondoTech.Enemies.States
{
    /// <summary>
    /// Attack state where the enemy plays attack animation and damages the player.
    /// Returns to Chase after attack completes.
    /// </summary>
    public class AttackState : IEnemyState
    {
        private readonly EnemyBase _enemy;
        private float _attackTimer;
        private const float ATTACK_DURATION = 0.5f;
        private bool _hasDealtDamage;

        public AttackState(EnemyBase enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            _enemy.StopMovement();
            _attackTimer = 0f;
            _hasDealtDamage = false;

            if (_enemy.Animator != null)
            {
                _enemy.Animator.SetBool("IsMoving", false);
                _enemy.Animator.SetBool("IsChasing", false);
                _enemy.Animator.SetTrigger("Attack");
            }
        }

        public void Execute()
        {
            _attackTimer += Time.deltaTime;

            // Deal damage at the midpoint of the attack
            if (!_hasDealtDamage && _attackTimer >= ATTACK_DURATION * 0.5f)
            {
                DealDamageToPlayer();
                _hasDealtDamage = true;
            }

            // Return to Chase after attack completes
            if (_attackTimer >= ATTACK_DURATION)
            {
                _enemy.StateMachine.ChangeState(new ChaseState(_enemy));
            }
        }

        public void Exit()
        {
        }

        private void DealDamageToPlayer()
        {
            if (_enemy.Player == null) return;

            // Re-check distance before applying damage in case player moved out of range
            if (_enemy.GetDistanceToPlayer() > _enemy.AttackRange) return;

            IDamageable playerDamageable = _enemy.Player.GetComponent<IDamageable>();
            if (playerDamageable != null && playerDamageable.IsAlive)
            {
                playerDamageable.TakeDamage(_enemy.AttackDamage);
            }
        }
    }
}
