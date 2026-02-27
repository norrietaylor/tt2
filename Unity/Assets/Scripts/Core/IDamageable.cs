namespace TaekwondoTech.Core
{
    /// <summary>
    /// Interface for any entity that can take damage.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Applies damage to the entity.
        /// </summary>
        /// <param name="damage">Amount of damage to apply.</param>
        void TakeDamage(int damage);

        /// <summary>
        /// Returns true if the entity is currently alive.
        /// </summary>
        bool IsAlive { get; }
    }
}
