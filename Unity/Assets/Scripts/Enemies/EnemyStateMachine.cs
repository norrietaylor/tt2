namespace TaekwondoTech.Enemies
{
    /// <summary>
    /// Generic state machine for managing enemy AI states.
    /// </summary>
    public class EnemyStateMachine
    {
        private IEnemyState _currentState;

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public IEnemyState CurrentState => _currentState;

        /// <summary>
        /// Changes to a new state.
        /// </summary>
        /// <param name="newState">The new state to transition to.</param>
        public void ChangeState(IEnemyState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        /// <summary>
        /// Updates the current state.
        /// </summary>
        public void Update()
        {
            _currentState?.Execute();
        }
    }
}
