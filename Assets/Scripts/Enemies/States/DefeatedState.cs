using UnityEngine;

namespace TaekwondoTech.Enemies.States
{
    public class DefeatedState : IEnemyState
    {
        private readonly EnemyStateMachine stateMachine;
        private readonly EnemyBase enemyBase;
        private readonly float destroyDelay = 2f;
        private float timer;

        public DefeatedState(EnemyStateMachine stateMachine, EnemyBase enemyBase)
        {
            this.stateMachine = stateMachine;
            this.enemyBase = enemyBase;
        }

        public void Enter()
        {
            if (enemyBase?.Rigidbody != null)
            {
                enemyBase.Rigidbody.velocity = Vector2.zero;
                enemyBase.Rigidbody.simulated = false;
            }

            timer = destroyDelay;
        }

        public void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f && enemyBase != null)
            {
                Object.Destroy(enemyBase.gameObject);
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
