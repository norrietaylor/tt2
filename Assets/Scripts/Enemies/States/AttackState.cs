using UnityEngine;

namespace TaekwondoTech.Enemies.States
{
    public class AttackState : IEnemyState
    {
        private readonly EnemyStateMachine stateMachine;
        private readonly EnemyBase enemyBase;

        public AttackState(EnemyStateMachine stateMachine, EnemyBase enemyBase)
        {
            this.stateMachine = stateMachine;
            this.enemyBase = enemyBase;
        }

        public void Enter()
        {
            if (enemyBase?.Rigidbody != null)
            {
                enemyBase.Rigidbody.velocity = new Vector2(0, enemyBase.Rigidbody.velocity.y);
            }
        }

        public void Update()
        {
            if (!stateMachine.IsPlayerInAttackRange())
            {
                if (stateMachine.CanSeePlayer())
                {
                    stateMachine.TransitionToState(EnemyStateType.Chase);
                }
                else
                {
                    stateMachine.TransitionToState(EnemyStateType.Idle);
                }
                return;
            }

            if (stateMachine.CanAttack())
            {
                stateMachine.PerformAttack();
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
