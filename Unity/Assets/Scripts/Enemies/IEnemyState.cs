namespace TaekwondoTech.Enemies
{
    /// <summary>
    /// Interface for enemy AI states.
    /// </summary>
    public interface IEnemyState
    {
        /// <summary>
        /// Called when entering this state.
        /// </summary>
        void Enter();

        /// <summary>
        /// Called every frame while in this state.
        /// </summary>
        void Execute();

        /// <summary>
        /// Called when exiting this state.
        /// </summary>
        void Exit();
    }
}
