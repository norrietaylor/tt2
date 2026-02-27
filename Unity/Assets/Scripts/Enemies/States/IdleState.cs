using UnityEngine;

namespace TaekwondoTech.Enemies.States
{
    /// <summary>
    /// Idle state where the enemy stands still for a random interval before transitioning to Patrol.
    /// </summary>
    public class IdleState : IEnemyState
    {
        private readonly EnemyBase _enemy;
        private float _idleTimer;
        private float _idleDuration;

        public IdleState(EnemyBase enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            _enemy.StopMovement();
            _idleDuration = Random.Range(1f, 3f);
            _idleTimer = 0f;

            if (_enemy.Animator != null)
            {
                _enemy.Animator.SetBool("IsMoving", false);
            }
        }

        public void Execute()
        {
            _idleTimer += Time.deltaTime;

            // Check if player is in detection radius
            if (_enemy.GetDistanceToPlayer() <= _enemy.DetectionRadius)
            {
                _enemy.StateMachine.ChangeState(new ChaseState(_enemy));
                return;
            }

            // Transition to Patrol after idle duration
            if (_idleTimer >= _idleDuration)
            {
                _enemy.StateMachine.ChangeState(new PatrolState(_enemy));
            }
        }

        public void Exit()
        {
        }
    }
}
