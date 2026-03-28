using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TaekwondoTech.Core;
using TaekwondoTech.Enemies;
using TaekwondoTech.Enemies.States;

namespace TaekwondoTech.Tests.EditMode.Enemies
{
    /// <summary>
    /// Test helper implementing IDamageable on the player so AttackState can
    /// deal damage in EditMode without real PlayerHealth.
    /// </summary>
    internal class StubDamageable : MonoBehaviour, IDamageable
    {
        public float Health { get; private set; } = 10f;
        public float MaxHealth => 10f;
        public bool IsAlive => Health > 0f;
        public int DamageReceivedCount { get; private set; }
        public float LastDamageAmount { get; private set; }

        public void TakeDamage(float damage)
        {
            Health -= damage;
            LastDamageAmount = damage;
            DamageReceivedCount++;
        }

        public void TakeDamage(float damage, GameObject damageSource)
        {
            TakeDamage(damage);
        }

        public void Heal(float amount)
        {
            Health = Mathf.Min(Health + amount, MaxHealth);
        }
    }

    /// <summary>
    /// Unit tests for all six concrete enemy states: Idle, Patrol, Chase,
    /// Attack, Stunned, and Defeated. Uses StubEnemyBase for EditMode setup
    /// and reflection-based timer manipulation for time-dependent transitions.
    /// </summary>
    [TestFixture]
    public class EnemyStateTests
    {
        private StubEnemyBase _stub;

        [TearDown]
        public void TearDown()
        {
            if (_stub != null)
            {
                _stub.Destroy();
                _stub = null;
            }
        }

        // -----------------------------------------------------------
        // IdleState
        // -----------------------------------------------------------

        [Test]
        public void IdleState_Enter_CallsStopMovement()
        {
            _stub = new StubEnemyBase();
            var enemy = _stub.Enemy;
            // Give the rigidbody some velocity so we can verify it gets zeroed
            enemy.Rigidbody.velocity = new Vector2(3f, 0f);

            var idleState = new IdleState(enemy);
            idleState.Enter();

            Assert.AreEqual(0f, enemy.Rigidbody.velocity.x, 0.01f,
                "StopMovement should zero horizontal velocity on Enter.");
        }

        [Test]
        public void IdleState_TransitionsToPatrol_AfterIdleDuration()
        {
            _stub = new StubEnemyBase(createPlayer: false);
            var enemy = _stub.Enemy;

            var idleState = new IdleState(enemy);
            _stub.StateMachine.ChangeState(idleState);

            // Set the internal _idleTimer past _idleDuration.
            // _idleDuration is random 1-3s, so set timer well above max.
            StubEnemyBase.SetStateTimer(idleState, "_idleTimer", 5f);
            idleState.Execute();

            Assert.IsInstanceOf<PatrolState>(_stub.StateMachine.CurrentState,
                "IdleState should transition to PatrolState after duration expires.");
        }

        [Test]
        public void IdleState_TransitionsToChase_WhenPlayerWithinDetectionRadius()
        {
            _stub = new StubEnemyBase(detectionRadius: 5f);
            _stub.SetPlayerDistance(3f);

            var idleState = new IdleState(_stub.Enemy);
            _stub.StateMachine.ChangeState(idleState);

            idleState.Execute();

            Assert.IsInstanceOf<ChaseState>(_stub.StateMachine.CurrentState,
                "IdleState should transition to ChaseState when player is within detection radius.");
        }

        // -----------------------------------------------------------
        // PatrolState
        // -----------------------------------------------------------

        [Test]
        public void PatrolState_TransitionsToChase_WhenPlayerWithinDetectionRadius()
        {
            _stub = new StubEnemyBase(detectionRadius: 5f);
            _stub.SetPlayerDistance(3f);

            var patrolState = new PatrolState(_stub.Enemy);
            _stub.StateMachine.ChangeState(patrolState);

            patrolState.Execute();

            Assert.IsInstanceOf<ChaseState>(_stub.StateMachine.CurrentState,
                "PatrolState should transition to ChaseState when player is in detection radius.");
        }

        [Test]
        public void PatrolState_MovesTowardWaypoint_DuringExecute()
        {
            _stub = new StubEnemyBase(detectionRadius: 5f, createPlayer: false);
            var enemy = _stub.Enemy;

            var patrolState = new PatrolState(enemy);
            _stub.StateMachine.ChangeState(patrolState);

            // Execute to trigger MoveTowards
            patrolState.Execute();

            // MoveTowards sets rigidbody velocity toward the target waypoint.
            // Since player is absent, no chase transition occurs.
            // Velocity should be non-zero in the x direction.
            float velocityX = enemy.Rigidbody.velocity.x;
            Assert.AreNotEqual(0f, velocityX,
                "PatrolState should call MoveTowards, producing non-zero velocity toward a waypoint.");
        }

        // -----------------------------------------------------------
        // ChaseState
        // -----------------------------------------------------------

        [Test]
        public void ChaseState_Enter_ShowsAlertIndicator()
        {
            _stub = new StubEnemyBase();
            var chaseState = new ChaseState(_stub.Enemy);

            chaseState.Enter();

            Assert.IsTrue(_stub.AlertIndicatorObject.activeSelf,
                "ChaseState Enter should call ShowAlertIndicator(true).");
        }

        [Test]
        public void ChaseState_TransitionsToAttack_WhenPlayerWithinAttackRange()
        {
            _stub = new StubEnemyBase(attackRange: 1f);
            _stub.SetPlayerDistance(0.5f);

            var chaseState = new ChaseState(_stub.Enemy);
            _stub.StateMachine.ChangeState(chaseState);

            chaseState.Execute();

            Assert.IsInstanceOf<AttackState>(_stub.StateMachine.CurrentState,
                "ChaseState should transition to AttackState when player is within attack range.");
        }

        [Test]
        public void ChaseState_TransitionsToPatrol_WhenPlayerExceedsHysteresisDistance()
        {
            _stub = new StubEnemyBase(detectionRadius: 5f);
            // Place player beyond 5 * 1.5 = 7.5 units
            _stub.SetPlayerDistance(8f);

            var chaseState = new ChaseState(_stub.Enemy);
            _stub.StateMachine.ChangeState(chaseState);

            chaseState.Execute();

            Assert.IsInstanceOf<PatrolState>(_stub.StateMachine.CurrentState,
                "ChaseState should transition to PatrolState when player exceeds hysteresis distance.");
        }

        [Test]
        public void ChaseState_Exit_HidesAlertIndicator()
        {
            _stub = new StubEnemyBase();
            var chaseState = new ChaseState(_stub.Enemy);
            chaseState.Enter();

            Assert.IsTrue(_stub.AlertIndicatorObject.activeSelf,
                "Alert indicator should be active after Enter.");

            chaseState.Exit();

            Assert.IsFalse(_stub.AlertIndicatorObject.activeSelf,
                "ChaseState Exit should call ShowAlertIndicator(false).");
        }

        // -----------------------------------------------------------
        // AttackState
        // -----------------------------------------------------------

        [Test]
        public void AttackState_Enter_CallsStopMovement()
        {
            _stub = new StubEnemyBase();
            var enemy = _stub.Enemy;
            enemy.Rigidbody.velocity = new Vector2(3f, 0f);

            var attackState = new AttackState(enemy);
            attackState.Enter();

            Assert.AreEqual(0f, enemy.Rigidbody.velocity.x, 0.01f,
                "AttackState Enter should call StopMovement, zeroing horizontal velocity.");
        }

        [Test]
        public void AttackState_TransitionsToChase_AfterAttackDuration()
        {
            _stub = new StubEnemyBase();
            var enemy = _stub.Enemy;

            var attackState = new AttackState(enemy);
            _stub.StateMachine.ChangeState(attackState);

            // Set timer to the attack duration (0.5s) to trigger transition
            StubEnemyBase.SetStateTimer(attackState, "_attackTimer", 0.5f);
            attackState.Execute();

            Assert.IsInstanceOf<ChaseState>(_stub.StateMachine.CurrentState,
                "AttackState should transition to ChaseState after attack duration.");
        }

        [Test]
        public void AttackState_DealsDamageExactlyOnce_AtMidpoint()
        {
            _stub = new StubEnemyBase(attackRange: 2f);
            _stub.SetPlayerDistance(0.5f);

            // Add IDamageable to the player so damage can be dealt
            var damageable = _stub.PlayerObject.AddComponent<StubDamageable>();

            var enemy = _stub.Enemy;
            var attackState = new AttackState(enemy);
            _stub.StateMachine.ChangeState(attackState);

            // Simulate reaching midpoint (0.25s) -- set timer just at midpoint
            StubEnemyBase.SetStateTimer(attackState, "_attackTimer", 0.25f);
            attackState.Execute();

            Assert.AreEqual(1, damageable.DamageReceivedCount,
                "Damage should be dealt exactly once at the midpoint.");

            // Execute again past midpoint -- damage should not be dealt a second time
            StubEnemyBase.SetStateTimer(attackState, "_attackTimer", 0.4f);
            attackState.Execute();

            Assert.AreEqual(1, damageable.DamageReceivedCount,
                "Damage should not be dealt a second time after midpoint.");
        }

        // -----------------------------------------------------------
        // StunnedState
        // -----------------------------------------------------------

        [Test]
        public void StunnedState_Enter_CallsStopMovement()
        {
            _stub = new StubEnemyBase();
            var enemy = _stub.Enemy;
            enemy.Rigidbody.velocity = new Vector2(3f, 0f);

            var stunnedState = new StunnedState(enemy);
            stunnedState.Enter();

            Assert.AreEqual(0f, enemy.Rigidbody.velocity.x, 0.01f,
                "StunnedState Enter should call StopMovement, zeroing horizontal velocity.");
        }

        [Test]
        public void StunnedState_TransitionsToChase_AfterStunDuration()
        {
            _stub = new StubEnemyBase();
            var enemy = _stub.Enemy;

            var stunnedState = new StunnedState(enemy);
            _stub.StateMachine.ChangeState(stunnedState);

            // Set timer to the stun duration (0.5s) to trigger transition
            StubEnemyBase.SetStateTimer(stunnedState, "_stunTimer", 0.5f);
            stunnedState.Execute();

            Assert.IsInstanceOf<ChaseState>(_stub.StateMachine.CurrentState,
                "StunnedState should transition to ChaseState after stun duration.");
        }

        // -----------------------------------------------------------
        // DefeatedState
        // -----------------------------------------------------------

        [Test]
        public void DefeatedState_Enter_CallsStopMovement()
        {
            _stub = new StubEnemyBase();
            var enemy = _stub.Enemy;
            enemy.Rigidbody.velocity = new Vector2(3f, 0f);

            // DefeatedState.Enter() calls Object.Destroy() which logs an error
            // in EditMode (Destroy requires play mode). Suppress it.
            LogAssert.ignoreFailingMessages = true;
            var defeatedState = new DefeatedState(enemy);
            defeatedState.Enter();
            LogAssert.ignoreFailingMessages = false;

            Assert.AreEqual(0f, enemy.Rigidbody.velocity.x, 0.01f,
                "DefeatedState Enter should call StopMovement, zeroing horizontal velocity.");
        }

        [Test]
        public void DefeatedState_Enter_DisablesCollider()
        {
            _stub = new StubEnemyBase();
            var enemy = _stub.Enemy;

            Assert.IsTrue(enemy.Collider.enabled,
                "Collider should be enabled before DefeatedState Enter.");

            // DefeatedState.Enter() calls Object.Destroy() which logs an error
            // in EditMode (Destroy requires play mode). Suppress it.
            LogAssert.ignoreFailingMessages = true;
            var defeatedState = new DefeatedState(enemy);
            defeatedState.Enter();
            LogAssert.ignoreFailingMessages = false;

            Assert.IsFalse(enemy.Collider.enabled,
                "DefeatedState Enter should disable the collider.");
        }
    }
}
