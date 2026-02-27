using UnityEngine;

namespace TaekwondoTech.Enemies.States
{
    public class StunnedState : IEnemyState
    {
        private readonly EnemyStateMachine stateMachine;
        private readonly EnemyBase enemyBase;

        public StunnedState(EnemyStateMachine stateMachine, EnemyBase enemyBase)
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
        }

        public void FixedUpdate()
        {
        }

        public void Exit()
        {
        }
    }
}
