using UnityEngine;

namespace TaekwondoTech.Enemies.States
{
    /// <summary>
    /// Chase state where the enemy moves toward the player.
    /// Shows red alert indicator and transitions to Attack when in range,
    /// or back to Patrol when player exits 1.5x detection radius.
    /// </summary>
    public class ChaseState : IEnemyState
    {
        private readonly EnemyBase _enemy;
        private const float CHASE_EXIT_MULTIPLIER = 1.5f;

        public ChaseState(EnemyBase enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            _enemy.ShowAlertIndicator(true);

            if (_enemy.Animator != null)
            {
                _enemy.Animator.SetBool("IsMoving", true);
                _enemy.Animator.SetBool("IsChasing", true);
            }
        }

        public void Execute()
        {
            if (_enemy.Player == null)
            {
                _enemy.StateMachine.ChangeState(new PatrolState(_enemy));
                return;
            }

            float distanceToPlayer = _enemy.GetDistanceToPlayer();

            // Transition to Attack if in range
            if (distanceToPlayer <= _enemy.AttackRange)
            {
                _enemy.StateMachine.ChangeState(new AttackState(_enemy));
                return;
            }

            // Transition back to Patrol if player is too far
            if (distanceToPlayer > _enemy.DetectionRadius * CHASE_EXIT_MULTIPLIER)
            {
                _enemy.StateMachine.ChangeState(new PatrolState(_enemy));
                return;
            }

            // Move towards player
            _enemy.MoveTowards(_enemy.Player.position);
        }

        public void Exit()
        {
            _enemy.ShowAlertIndicator(false);

            if (_enemy.Animator != null)
            {
                _enemy.Animator.SetBool("IsChasing", false);
            }
        }
    }
}
