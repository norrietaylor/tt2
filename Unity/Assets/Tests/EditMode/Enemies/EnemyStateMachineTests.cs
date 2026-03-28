using NUnit.Framework;
using TaekwondoTech.Enemies;

namespace TaekwondoTech.Tests.EditMode.Enemies
{
    /// <summary>
    /// Mock implementation of IEnemyState for testing EnemyStateMachine.
    /// Tracks call counts and global call order for Enter, Execute, and Exit.
    /// </summary>
    internal class MockEnemyState : IEnemyState
    {
        private static int _globalCallCounter;

        public int EnterCallCount { get; private set; }
        public int ExecuteCallCount { get; private set; }
        public int ExitCallCount { get; private set; }

        public int EnterCallOrder { get; private set; }
        public int ExitCallOrder { get; private set; }

        /// <summary>Resets the global call counter used for ordering assertions.</summary>
        public static void ResetGlobalCounter()
        {
            _globalCallCounter = 0;
        }

        public void Enter()
        {
            EnterCallCount++;
            EnterCallOrder = ++_globalCallCounter;
        }

        public void Execute()
        {
            ExecuteCallCount++;
        }

        public void Exit()
        {
            ExitCallCount++;
            ExitCallOrder = ++_globalCallCounter;
        }
    }

    /// <summary>
    /// Unit tests for <see cref="EnemyStateMachine"/>.
    /// Verifies state lifecycle (Enter/Execute/Exit) and transition ordering.
    /// </summary>
    [TestFixture]
    public class EnemyStateMachineTests
    {
        [SetUp]
        public void SetUp()
        {
            MockEnemyState.ResetGlobalCounter();
        }

        [Test]
        public void CurrentState_WhenNewlyInstantiated_IsNull()
        {
            var sm = new EnemyStateMachine();

            Assert.IsNull(sm.CurrentState);
        }

        [Test]
        public void ChangeState_WithNewState_SetsCurrentState()
        {
            var sm = new EnemyStateMachine();
            var state = new MockEnemyState();

            sm.ChangeState(state);

            Assert.AreSame(state, sm.CurrentState);
        }

        [Test]
        public void ChangeState_WithNewState_CallsEnterOnNewState()
        {
            var sm = new EnemyStateMachine();
            var state = new MockEnemyState();

            sm.ChangeState(state);

            Assert.AreEqual(1, state.EnterCallCount);
        }

        [Test]
        public void ChangeState_WithPreviousState_CallsExitOnPreviousState()
        {
            var sm = new EnemyStateMachine();
            var firstState = new MockEnemyState();
            var secondState = new MockEnemyState();
            sm.ChangeState(firstState);

            sm.ChangeState(secondState);

            Assert.AreEqual(1, firstState.ExitCallCount);
        }

        [Test]
        public void ChangeState_OnTransition_CallsExitBeforeEnter()
        {
            var sm = new EnemyStateMachine();
            var firstState = new MockEnemyState();
            var secondState = new MockEnemyState();
            sm.ChangeState(firstState);
            MockEnemyState.ResetGlobalCounter();

            sm.ChangeState(secondState);

            // firstState.Exit must have occurred before secondState.Enter
            Assert.Less(firstState.ExitCallOrder, secondState.EnterCallOrder,
                "Exit on the outgoing state must be called before Enter on the incoming state.");
        }

        [Test]
        public void Update_WithActiveState_CallsExecuteOnCurrentState()
        {
            var sm = new EnemyStateMachine();
            var state = new MockEnemyState();
            sm.ChangeState(state);

            sm.Update();

            Assert.AreEqual(1, state.ExecuteCallCount);
        }

        [Test]
        public void Update_WithNullState_DoesNotThrow()
        {
            var sm = new EnemyStateMachine();

            Assert.DoesNotThrow(() => sm.Update());
        }

        [Test]
        public void ChangeState_ToNull_CallsExitOnPreviousState()
        {
            var sm = new EnemyStateMachine();
            var state = new MockEnemyState();
            sm.ChangeState(state);

            sm.ChangeState(null);

            Assert.AreEqual(1, state.ExitCallCount);
        }

        [Test]
        public void ChangeState_ToNull_SetsCurrentStateToNull()
        {
            var sm = new EnemyStateMachine();
            var state = new MockEnemyState();
            sm.ChangeState(state);

            sm.ChangeState(null);

            Assert.IsNull(sm.CurrentState);
        }
    }
}
