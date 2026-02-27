namespace TaekwondoTech.Core
{
    /// <summary>
    /// Interface for objects that can take damage from player attacks or hazards.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Apply damage to this object.
        /// </summary>
        /// <param name="amount">The amount of damage to apply.</param>
        void TakeDamage(int amount);
    }
}
