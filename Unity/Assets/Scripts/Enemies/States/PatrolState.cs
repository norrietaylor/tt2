using UnityEngine;

namespace TaekwondoTech.Enemies.States
{
    /// <summary>
    /// Patrol state where the enemy moves between two waypoints.
    /// Transitions to Chase when player enters detection radius.
    /// </summary>
    public class PatrolState : IEnemyState
    {
        private readonly EnemyBase _enemy;
        private Transform _currentTarget;

        public PatrolState(EnemyBase enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            if (_enemy.Animator != null)
            {
                _enemy.Animator.SetBool("IsMoving", true);
            }

            // Set initial target to the closest waypoint
            if (_enemy.WaypointA != null && _enemy.WaypointB != null)
            {
                float distanceToA = Vector2.Distance(_enemy.transform.position, _enemy.WaypointA.position);
                float distanceToB = Vector2.Distance(_enemy.transform.position, _enemy.WaypointB.position);
                _currentTarget = distanceToA < distanceToB ? _enemy.WaypointA : _enemy.WaypointB;
            }
        }

        public void Execute()
        {
            // Check if player is in detection radius
            if (_enemy.GetDistanceToPlayer() <= _enemy.DetectionRadius)
            {
                _enemy.StateMachine.ChangeState(new ChaseState(_enemy));
                return;
            }

            // Check if waypoints are set
            if (_enemy.WaypointA == null || _enemy.WaypointB == null)
            {
                _enemy.StopMovement();
                return;
            }

            // Move towards current target
            _enemy.MoveTowards(_currentTarget.position);

            // Check if reached current target
            float distanceToTarget = Vector2.Distance(_enemy.transform.position, _currentTarget.position);
            if (distanceToTarget < 0.1f)
            {
                // Switch target
                _currentTarget = _currentTarget == _enemy.WaypointA ? _enemy.WaypointB : _enemy.WaypointA;
            }
        }

        public void Exit()
        {
        }
    }
}
