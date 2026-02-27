using UnityEngine;

namespace TaekwondoTech.Enemies.States
{
    public class ChaseState : IEnemyState
    {
        private readonly EnemyStateMachine stateMachine;
        private readonly EnemyBase enemyBase;

        public ChaseState(EnemyStateMachine stateMachine, EnemyBase enemyBase)
        {
            this.stateMachine = stateMachine;
            this.enemyBase = enemyBase;
        }

        public void Enter()
        {
        }

        public void Update()
        {
            if (!stateMachine.CanSeePlayer())
            {
                stateMachine.TransitionToState(EnemyStateType.Idle);
                return;
            }

            if (stateMachine.IsPlayerInAttackRange())
            {
                stateMachine.TransitionToState(EnemyStateType.Attack);
            }
        }

        public void FixedUpdate()
        {
            if (stateMachine.Player == null || enemyBase?.Rigidbody == null)
                return;

            float distanceToPlayer = Vector2.Distance(enemyBase.transform.position, stateMachine.Player.position);

            if (distanceToPlayer > stateMachine.ChaseStopDistance)
            {
                Vector2 direction = (stateMachine.Player.position - enemyBase.transform.position).normalized;
                enemyBase.Rigidbody.velocity = new Vector2(direction.x * stateMachine.ChaseSpeed, enemyBase.Rigidbody.velocity.y);

                if (Mathf.Abs(direction.x) > 0.1f)
                {
                    enemyBase.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1f, 1f);
                }
            }
            else
            {
                enemyBase.Rigidbody.velocity = new Vector2(0, enemyBase.Rigidbody.velocity.y);
            }
        }

        public void Exit()
        {
        }
    }
}
