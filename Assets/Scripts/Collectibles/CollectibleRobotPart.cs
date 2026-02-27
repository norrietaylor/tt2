using System.Collections;
using UnityEngine;

namespace TaekwondoTech.Collectibles
{
    using TaekwondoTech.Core;

    /// <summary>
    /// MonoBehaviour placed on a robot-part collectible prefab in the level.
    ///
    /// Requirements met:
    ///   • Trigger collider causes pickup on player contact.
    ///   • Shimmer / glow particle effect whose colour matches the part's rarity.
    ///   • Pickup animation (scale-pop + fade) and optional AudioSource SFX.
    ///   • Notifies <see cref="RobotInventory"/> on collect.
    ///   • Auto-saves via <see cref="SaveManager"/> after collection.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CollectibleRobotPart : MonoBehaviour
    {
        [Header("Part Data")]
        [Tooltip("The RobotPartData ScriptableObject that describes this collectible.")]
        public RobotPartData partData;

        [Header("Visual Feedback")]
        [Tooltip("Particle System used for the shimmer/glow effect.  " +
                 "Its Start Color will be tinted to match the part rarity.")]
        public ParticleSystem glowParticles;

        [Tooltip("SpriteRenderer that shows the part art.")]
        public SpriteRenderer spriteRenderer;

        [Header("Audio")]
        [Tooltip("AudioSource played on pickup (assign a pickup SFX clip).")]
        public AudioSource pickupAudio;

        [Header("Animation Timing")]
        [Tooltip("Duration of the scale-pop pickup animation in seconds.")]
        [Range(0.1f, 1f)]
        public float pickupAnimDuration = 0.35f;

        private bool _collected;

        // ------------------------------------------------------------------ //
        //  Unity lifecycle
        // ------------------------------------------------------------------ //

        private void Start()
        {
            if (partData == null)
            {
                Debug.LogError($"CollectibleRobotPart on '{name}': partData is not assigned.", this);
                return;
            }

            ApplyRarityVisuals();
        }

        // ------------------------------------------------------------------ //
        //  Trigger
        // ------------------------------------------------------------------ //

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_collected) return;
            if (!other.CompareTag("Player")) return;

            _collected = true;
            StartCoroutine(PlayPickupAndCollect());
        }

        // ------------------------------------------------------------------ //
        //  Visual helpers
        // ------------------------------------------------------------------ //

        private void ApplyRarityVisuals()
        {
            Color rarityColor = partData.RarityColor();

            // Tint the sprite outline / main renderer.
            if (spriteRenderer != null)
                spriteRenderer.color = Color.white; // keep original art; rarity shown via particles

            // Tint glow particles to match rarity.
            if (glowParticles != null)
            {
                var main = glowParticles.main;
                main.startColor = new ParticleSystem.MinMaxGradient(rarityColor);

                if (!glowParticles.isPlaying)
                    glowParticles.Play();
            }
        }

        // ------------------------------------------------------------------ //
        //  Pickup sequence
        // ------------------------------------------------------------------ //

        private IEnumerator PlayPickupAndCollect()
        {
            // Play SFX.
            if (pickupAudio != null)
                pickupAudio.Play();

            // Stop glow particles.
            if (glowParticles != null)
                glowParticles.Stop();

            // Scale-pop animation: grow then shrink to zero.
            yield return StartCoroutine(ScalePopAnimation());

            // Notify inventory — only destroy on success.
            if (RobotInventory.Instance != null)
            {
                RobotInventory.Instance.CollectPart(partData);
                SaveManager.Save();
                Destroy(gameObject);
            }
            else
            {
                // Restore visual state so the part can be picked up again once the
                // inventory singleton is available.
                _collected = false;
                transform.localScale = Vector3.one;
                if (glowParticles != null) glowParticles.Play();
                Debug.LogWarning("CollectibleRobotPart: RobotInventory.Instance is null; pickup deferred.");
            }
        }

        private IEnumerator ScalePopAnimation()
        {
            float elapsed   = 0f;
            float halfTime  = pickupAnimDuration * 0.5f;
            Vector3 origScale = transform.localScale;

            // Grow phase.
            while (elapsed < halfTime)
            {
                float t = elapsed / halfTime;
                transform.localScale = origScale * (1f + 0.4f * t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;

            // Shrink to zero phase.
            while (elapsed < halfTime)
            {
                float t = elapsed / halfTime;
                transform.localScale = origScale * (1.4f * (1f - t));
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = Vector3.zero;
        }
    }
}
