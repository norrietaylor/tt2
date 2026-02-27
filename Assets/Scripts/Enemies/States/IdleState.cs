using UnityEngine;

namespace TaekwondoTech.Enemies.States
{
    public class IdleState : IEnemyState
    {
        private readonly EnemyStateMachine stateMachine;
        private readonly EnemyBase enemyBase;

        public IdleState(EnemyStateMachine stateMachine, EnemyBase enemyBase)
        {
            this.stateMachine = stateMachine;
            this.enemyBase = enemyBase;
        }

        public void Enter()
        {
            if (enemyBase?.Rigidbody != null)
            {
                enemyBase.Rigidbody.velocity = Vector2.zero;
            }
        }

        public void Update()
        {
            if (stateMachine.CanSeePlayer())
            {
                stateMachine.TransitionToState(EnemyStateType.Chase);
            }
            else if (stateMachine.Waypoints != null && stateMachine.Waypoints.Length > 0)
            {
                stateMachine.TransitionToState(EnemyStateType.Patrol);
            }
        }

        public void FixedUpdate()
        {
        }

        public void Exit()
        {
        }
    }
}
