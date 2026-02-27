using UnityEngine;

namespace TaekwondoTech.Core
{
    /// <summary>
    /// Defines the five robot part types a player can collect.
    /// </summary>
    public enum RobotPartType
    {
        Head,
        Body,
        Arms,
        Legs,
        PowerCore
    }

    /// <summary>
    /// Defines the rarity tiers for a robot part.
    /// </summary>
    public enum RobotPartRarity
    {
        Common,
        Rare,
        Epic
    }

    /// <summary>
    /// ScriptableObject data asset for a single robot part.
    /// Create instances via Assets → Create → TaekwondoTech → Robot Part Data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewRobotPart", menuName = "TaekwondoTech/Robot Part Data")]
    public class RobotPartData : ScriptableObject
    {
        [Tooltip("Which slot this part occupies on the robot blueprint.")]
        public RobotPartType partType;

        [Tooltip("Rarity tier — affects glow colour and drop probability.")]
        public RobotPartRarity rarity;

        [Tooltip("Sprite shown in inventory, craft slots, and on the companion.")]
        public Sprite sprite;

        [Tooltip("Human-readable part name displayed in UI.")]
        public string displayName;

        /// <summary>
        /// Returns the glow/outline colour corresponding to this part's rarity.
        /// Common → grey, Rare → blue, Epic → purple.
        /// </summary>
        public Color RarityColor()
        {
            return rarity switch
            {
                RobotPartRarity.Common => new Color(0.75f, 0.75f, 0.75f),
                RobotPartRarity.Rare   => new Color(0.20f, 0.60f, 1.00f),
                RobotPartRarity.Epic   => new Color(0.70f, 0.20f, 1.00f),
                _                      => Color.white
            };
        }
    }
}
