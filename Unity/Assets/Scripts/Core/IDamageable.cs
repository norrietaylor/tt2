using UnityEngine;

namespace TaekwondoTech.Core
{
    /// <summary>
    /// Interface for entities that can take damage.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Apply damage to this entity.
        /// </summary>
        /// <param name="damage">Amount of damage to apply.</param>
        void TakeDamage(int damage);

        /// <summary>
        /// Check if this entity is currently alive.
        /// </summary>
        bool IsAlive { get; }
    }
}
