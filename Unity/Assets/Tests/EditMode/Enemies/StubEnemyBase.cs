using System.Reflection;
using UnityEditor;
using UnityEngine;
using TaekwondoTech.Enemies;

namespace TaekwondoTech.Tests.EditMode.Enemies
{
    /// <summary>
    /// Test helper that creates a fully wired EnemyBase instance in EditMode
    /// without scene dependencies. Uses SerializedObject to configure private
    /// serialized fields and provides cleanup for test-created GameObjects.
    /// </summary>
    internal class StubEnemyBase
    {
        private readonly GameObject _enemyObject;
        private readonly EnemyBase _enemy;
        private readonly GameObject _playerObject;
        private readonly GameObject _waypointAObject;
        private readonly GameObject _waypointBObject;
        private readonly GameObject _alertIndicatorObject;

        public EnemyBase Enemy => _enemy;
        public GameObject PlayerObject => _playerObject;
        public GameObject WaypointAObject => _waypointAObject;
        public GameObject WaypointBObject => _waypointBObject;
        public GameObject AlertIndicatorObject => _alertIndicatorObject;
        public EnemyStateMachine StateMachine => _enemy.StateMachine;

        /// <summary>
        /// Creates a StubEnemyBase with configurable parameters.
        /// Awake() is called by Unity when AddComponent runs in EditMode,
        /// which initializes Rigidbody, Collider, StateMachine, and health.
        /// </summary>
        public StubEnemyBase(
            float detectionRadius = 5f,
            float attackRange = 1f,
            int attackDamage = 1,
            float moveSpeed = 2f,
            int maxHealth = 3,
            bool createPlayer = true,
            bool createWaypoints = true,
            bool createAlertIndicator = true)
        {
            // Create the enemy GameObject. AddComponent triggers Awake().
            _enemyObject = new GameObject("TestEnemy");
            _enemy = _enemyObject.AddComponent<EnemyBase>();

            // Use SerializedObject to set private [SerializeField] fields
            var so = new SerializedObject(_enemy);

            so.FindProperty("_detectionRadius").floatValue = detectionRadius;
            so.FindProperty("_attackRange").floatValue = attackRange;
            so.FindProperty("_attackDamage").intValue = attackDamage;
            so.FindProperty("_moveSpeed").floatValue = moveSpeed;
            so.FindProperty("_maxHealth").intValue = maxHealth;

            if (createPlayer)
            {
                _playerObject = new GameObject("TestPlayer");
                so.FindProperty("_player").objectReferenceValue = _playerObject.transform;
            }

            if (createWaypoints)
            {
                _waypointAObject = new GameObject("WaypointA");
                _waypointAObject.transform.position = new Vector3(-5f, 0f, 0f);
                _waypointBObject = new GameObject("WaypointB");
                _waypointBObject.transform.position = new Vector3(5f, 0f, 0f);
                so.FindProperty("_waypointA").objectReferenceValue = _waypointAObject.transform;
                so.FindProperty("_waypointB").objectReferenceValue = _waypointBObject.transform;
            }

            if (createAlertIndicator)
            {
                _alertIndicatorObject = new GameObject("AlertIndicator");
                _alertIndicatorObject.SetActive(false);
                so.FindProperty("_alertIndicator").objectReferenceValue = _alertIndicatorObject;
            }

            so.ApplyModifiedPropertiesWithoutUndo();

            // Re-initialize health to reflect the new _maxHealth
            // (Awake already ran with default _maxHealth before our SerializedObject changes)
            SetPrivateField(_enemy, "_currentHealth", maxHealth);
        }

        /// <summary>
        /// Positions the player at a specific distance from the enemy along the X axis.
        /// </summary>
        public void SetPlayerDistance(float distance)
        {
            if (_playerObject == null) return;
            _playerObject.transform.position = _enemyObject.transform.position
                + new Vector3(distance, 0f, 0f);
        }

        /// <summary>
        /// Uses reflection to set a timer field on a state object, allowing
        /// time-based transition tests without relying on Time.deltaTime.
        /// </summary>
        public static void SetStateTimer(object state, string fieldName, float value)
        {
            var field = state.GetType().GetField(
                fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(state, value);
            }
        }

        /// <summary>
        /// Sets a private field on any object using reflection.
        /// </summary>
        public static void SetPrivateField(object target, string fieldName, object value)
        {
            var type = target.GetType();
            while (type != null)
            {
                var field = type.GetField(
                    fieldName,
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(target, value);
                    return;
                }
                type = type.BaseType;
            }
        }

        /// <summary>
        /// Destroys all GameObjects created by this stub. Call in test TearDown.
        /// </summary>
        public void Destroy()
        {
            if (_alertIndicatorObject != null) Object.DestroyImmediate(_alertIndicatorObject);
            if (_waypointBObject != null) Object.DestroyImmediate(_waypointBObject);
            if (_waypointAObject != null) Object.DestroyImmediate(_waypointAObject);
            if (_playerObject != null) Object.DestroyImmediate(_playerObject);
            if (_enemyObject != null) Object.DestroyImmediate(_enemyObject);
        }
    }
}
