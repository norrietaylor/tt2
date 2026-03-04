using UnityEngine;

namespace TaekwondoTech.Enemies.States
{
    /// <summary>
    /// Stunned state triggered when the enemy is hit.
    /// Brief stun (0.5s) before transitioning to Chase.
    /// </summary>
    public class StunnedState : IEnemyState
    {
        private readonly EnemyBase _enemy;
        private float _stunTimer;
        private const float STUN_DURATION = 0.5f;

        public StunnedState(EnemyBase enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            _enemy.StopMovement();
            _stunTimer = 0f;

            if (_enemy.Animator != null)
            {
                _enemy.Animator.SetBool("IsMoving", false);
                _enemy.Animator.SetBool("IsChasing", false);
                _enemy.Animator.SetTrigger("Stunned");
            }
        }

        public void Execute()
        {
            _stunTimer += Time.deltaTime;

            if (_stunTimer >= STUN_DURATION)
            {
                // Resume chasing player after stun
                _enemy.StateMachine.ChangeState(new ChaseState(_enemy));
            }
        }

        public void Exit()
        {
        }
    }
}
