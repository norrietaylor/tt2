using UnityEngine;

namespace TaekwondoTech.Core
{
    /// <summary>
    /// Interface for entities that can take damage (Player, Enemies).
    /// Implements the damage system from REQ-001 (Player Health) and REQ-003 (Enemy System).
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Current health of the entity.
        /// </summary>
        float Health { get; }

        /// <summary>
        /// Maximum health of the entity.
        /// </summary>
        float MaxHealth { get; }

        /// <summary>
        /// Whether the entity is currently alive.
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Apply damage to this entity.
        /// </summary>
        /// <param name="damage">Amount of damage to apply.</param>
        void TakeDamage(float damage);

        /// <summary>
        /// Apply damage to this entity from a specific source.
        /// </summary>
        /// <param name="damage">Amount of damage to apply.</param>
        /// <param name="damageSource">The GameObject that caused the damage (can be null).</param>
        void TakeDamage(float damage, GameObject damageSource);

        /// <summary>
        /// Heal this entity.
        /// </summary>
        /// <param name="amount">Amount of health to restore.</param>
        void Heal(float amount);
    }

    /// <summary>
    /// Interface for collectible items (coins, robot parts, power-ups).
    /// Implements the collection system from REQ-002 (Level System) and REQ-004 (Robot Building).
    /// </summary>
    public interface ICollectible
    {
        /// <summary>
        /// Called when this collectible is picked up by the player.
        /// </summary>
        /// <param name="collector">The GameObject that collected this item (typically the player).</param>
        void OnCollect(GameObject collector);

        /// <summary>
        /// The type of collectible.
        /// </summary>
        CollectibleType CollectibleType { get; }

        /// <summary>
        /// Visual rarity indicator (Common, Rare, Epic).
        /// Used for robot parts and special collectibles.
        /// </summary>
        CollectibleRarity Rarity { get; }
    }

    /// <summary>
    /// Types of collectibles in the game.
    /// </summary>
    public enum CollectibleType
    {
        Coin,
        RobotPart,
        PowerUp,
        CostumeItem
    }

    /// <summary>
    /// Rarity levels for collectibles.
    /// From REQ-004: Parts have visual rarity (Common, Rare, Epic).
    /// </summary>
    public enum CollectibleRarity
    {
        Common,
        Rare,
        Epic
    }

    /// <summary>
    /// Interface for interactive objects the player can interact with.
    /// Supports various interactive elements like switches, doors, NPCs, etc.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Called when the player interacts with this object.
        /// </summary>
        /// <param name="interactor">The GameObject attempting to interact (typically the player).</param>
        void Interact(GameObject interactor);

        /// <summary>
        /// Whether this object can currently be interacted with.
        /// </summary>
        bool CanInteract { get; }

        /// <summary>
        /// Display prompt for interaction (e.g., "Press E to Open").
        /// </summary>
        string InteractionPrompt { get; }
    }

    /// <summary>
    /// Interface for power-ups in the queue system.
    /// Implements REQ-007 (Power-Up Queue System).
    /// Power-up types: Speed Boost, Shield, Elemental Attack, Invincibility.
    /// </summary>
    public interface IPowerUp
    {
        /// <summary>
        /// The type of power-up.
        /// </summary>
        PowerUpType PowerUpType { get; }

        /// <summary>
        /// Duration of the power-up effect in seconds.
        /// </summary>
        float Duration { get; }

        /// <summary>
        /// Activate this power-up on the target GameObject (typically the player).
        /// </summary>
        /// <param name="target">The GameObject to apply the power-up to.</param>
        void Activate(GameObject target);

        /// <summary>
        /// Deactivate/end this power-up's effect.
        /// </summary>
        /// <param name="target">The GameObject to remove the power-up from.</param>
        void Deactivate(GameObject target);

        /// <summary>
        /// Whether this power-up is currently active.
        /// </summary>
        bool IsActive { get; }
    }

    /// <summary>
    /// Types of power-ups from REQ-007.
    /// </summary>
    public enum PowerUpType
    {
        SpeedBoost,
        Shield,
        ElementalAttack,
        Invincibility
    }
}
