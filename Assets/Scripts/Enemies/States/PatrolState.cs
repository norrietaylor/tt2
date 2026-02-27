using UnityEngine;

namespace TaekwondoTech.Enemies.States
{
    public class PatrolState : IEnemyState
    {
        private readonly EnemyStateMachine stateMachine;
        private readonly EnemyBase enemyBase;

        public PatrolState(EnemyStateMachine stateMachine, EnemyBase enemyBase)
        {
            this.stateMachine = stateMachine;
            this.enemyBase = enemyBase;
        }

        public void Enter()
        {
        }

        public void Update()
        {
            if (stateMachine.CanSeePlayer())
            {
                stateMachine.TransitionToState(EnemyStateType.Chase);
                return;
            }

            if (stateMachine.HasReachedWaypoint())
            {
                stateMachine.MoveToNextWaypoint();
            }
        }

        public void FixedUpdate()
        {
            if (stateMachine.Waypoints == null || stateMachine.Waypoints.Length == 0)
                return;

            Transform targetWaypoint = stateMachine.Waypoints[stateMachine.CurrentWaypointIndex];
            if (targetWaypoint == null || enemyBase?.Rigidbody == null)
                return;

            Vector2 direction = (targetWaypoint.position - enemyBase.transform.position).normalized;
            enemyBase.Rigidbody.velocity = new Vector2(direction.x * stateMachine.PatrolSpeed, enemyBase.Rigidbody.velocity.y);

            if (Mathf.Abs(direction.x) > 0.1f)
            {
                enemyBase.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1f, 1f);
            }
        }

        public void Exit()
        {
        }
    }
}
