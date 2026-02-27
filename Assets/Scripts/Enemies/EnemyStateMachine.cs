using UnityEngine;
using System.Collections.Generic;

namespace TaekwondoTech.Enemies
{
    public enum EnemyStateType
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Stunned,
        Defeated
    }

    /// <summary>
    /// EnemyStateMachine — manages enemy AI states including Idle, Patrol, Chase, Attack, Stunned, and Defeated.
    /// </summary>
    public class EnemyStateMachine : MonoBehaviour
    {
        [Header("State Settings")]
        [SerializeField] private EnemyStateType initialState = EnemyStateType.Idle;

        [Header("Detection")]
        [SerializeField] private float detectionRadius = 5f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Patrol")]
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float waypointReachThreshold = 0.1f;

        [Header("Chase")]
        [SerializeField] private float chaseSpeed = 4f;
        [SerializeField] private float chaseStopDistance = 1.5f;

        [Header("Attack")]
        [SerializeField] private float attackRange = 1.2f;
        [SerializeField] private float attackCooldown = 2f;

        [Header("Stun")]
        [SerializeField] private float stunDuration = 2f;

        private Dictionary<EnemyStateType, IEnemyState> states;
        private IEnemyState currentState;
        private Transform player;
        private EnemyBase enemyBase;
        private int currentWaypointIndex;
        private float lastAttackTime;
        private float stunTimer;

        public float DetectionRadius => detectionRadius;
        public float PatrolSpeed => patrolSpeed;
        public float ChaseSpeed => chaseSpeed;
        public float ChaseStopDistance => chaseStopDistance;
        public float AttackRange => attackRange;
        public float AttackCooldown => attackCooldown;
        public Transform Player => player;
        public Transform[] Waypoints => waypoints;
        public int CurrentWaypointIndex => currentWaypointIndex;

        private void Awake()
        {
            enemyBase = GetComponent<EnemyBase>();
            InitializeStates();
        }

        private void Start()
        {
            FindPlayer();
            TransitionToState(initialState);
        }

        private void Update()
        {
            currentState?.Update();

            if (currentState is States.StunnedState)
            {
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0f && enemyBase != null)
                {
                    enemyBase.RecoverFromStun();
                    TransitionToState(EnemyStateType.Idle);
                }
            }
        }

        private void FixedUpdate()
        {
            currentState?.FixedUpdate();
        }

        private void InitializeStates()
        {
            states = new Dictionary<EnemyStateType, IEnemyState>
            {
                { EnemyStateType.Idle, new States.IdleState(this, enemyBase) },
                { EnemyStateType.Patrol, new States.PatrolState(this, enemyBase) },
                { EnemyStateType.Chase, new States.ChaseState(this, enemyBase) },
                { EnemyStateType.Attack, new States.AttackState(this, enemyBase) },
                { EnemyStateType.Stunned, new States.StunnedState(this, enemyBase) },
                { EnemyStateType.Defeated, new States.DefeatedState(this, enemyBase) }
            };
        }

        public void TransitionToState(EnemyStateType stateType)
        {
            currentState?.Exit();

            if (states.TryGetValue(stateType, out IEnemyState newState))
            {
                currentState = newState;
                currentState.Enter();

                if (stateType == EnemyStateType.Stunned)
                {
                    stunTimer = stunDuration;
                }
            }
        }

        public bool CanSeePlayer()
        {
            if (player == null) return false;

            float distance = Vector2.Distance(transform.position, player.position);
            return distance <= detectionRadius;
        }

        public bool IsPlayerInAttackRange()
        {
            if (player == null) return false;

            float distance = Vector2.Distance(transform.position, player.position);
            return distance <= attackRange;
        }

        public bool CanAttack()
        {
            return Time.time >= lastAttackTime + attackCooldown;
        }

        public void PerformAttack()
        {
            lastAttackTime = Time.time;
        }

        public void MoveToNextWaypoint()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        public bool HasReachedWaypoint()
        {
            if (waypoints == null || waypoints.Length == 0 || currentWaypointIndex >= waypoints.Length)
                return true;

            Transform targetWaypoint = waypoints[currentWaypointIndex];
            if (targetWaypoint == null) return true;

            return Vector2.Distance(transform.position, targetWaypoint.position) < waypointReachThreshold;
        }

        private void FindPlayer()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            if (waypoints != null && waypoints.Length > 0)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < waypoints.Length; i++)
                {
                    if (waypoints[i] != null)
                    {
                        Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
                        if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                        {
                            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                        }
                    }
                }
                if (waypoints.Length > 1 && waypoints[0] != null && waypoints[waypoints.Length - 1] != null)
                {
                    Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
                }
            }
        }
    }

    public interface IEnemyState
    {
        void Enter();
        void Update();
        void FixedUpdate();
        void Exit();
    }
}
