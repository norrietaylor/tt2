using UnityEngine;

namespace TaekwondoTech.Collectibles
{
    public enum RobotPartType
    {
        Head,
        Body,
        Arms,
        Legs,
        PowerCore
    }

    public enum RobotPartRarity
    {
        Common,
        Rare,
        Epic
    }

    /// <summary>
    /// RobotPart — specialized collectible for robot building system.
    /// Has type (Head, Body, Arms, Legs, PowerCore) and rarity (Common, Rare, Epic).
    /// </summary>
    public class RobotPart : Collectible
    {
        [Header("Robot Part Settings")]
        [SerializeField] private RobotPartType partType;
        [SerializeField] private RobotPartRarity rarity = RobotPartRarity.Common;

        [Header("Glow Effect")]
        [SerializeField] private float glowIntensity = 1.5f;
        [SerializeField] private Color commonColor = Color.white;
        [SerializeField] private Color rareColor = Color.blue;
        [SerializeField] private Color epicColor = Color.magenta;

        public RobotPartType PartType => partType;
        public RobotPartRarity Rarity => rarity;

        protected override void Awake()
        {
            base.Awake();
            ApplyRarityColor();
        }

        private void ApplyRarityColor()
        {
            if (spriteRenderer == null) return;

            Color rarityColor = rarity switch
            {
                RobotPartRarity.Common => commonColor,
                RobotPartRarity.Rare => rareColor,
                RobotPartRarity.Epic => epicColor,
                _ => commonColor
            };

            spriteRenderer.color = rarityColor;
        }

        protected override void ApplyShimmer()
        {
            base.ApplyShimmer();

            if (spriteRenderer != null)
            {
                float glow = 1f + Mathf.Sin(Time.time * shimmerSpeed * 2f) * glowIntensity;
                spriteRenderer.material.SetFloat("_GlowAmount", glow);
            }
        }

        protected override void Collect()
        {
            base.Collect();
        }
    }
}
