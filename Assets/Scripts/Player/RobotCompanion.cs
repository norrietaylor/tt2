using UnityEngine;

namespace TaekwondoTech.Player
{
    using TaekwondoTech.Core;

    /// <summary>
    /// Robot companion that follows the player during gameplay.
    ///
    /// When attached to the RobotCompanion prefab this script:
    ///   • Checks whether the robot has been fully assembled
    ///     (<see cref="RobotInventory.IsRobotAssembled"/>).
    ///   • If so, shows the companion and applies the five part sprites to the
    ///     layered <see cref="SpriteRenderer"/>s.
    ///   • Smoothly follows the player transform with a configurable offset
    ///     and follow speed.
    ///   • Hides itself when the robot is not yet assembled.
    /// </summary>
    public class RobotCompanion : MonoBehaviour
    {
        [Header("Player Reference")]
        [Tooltip("The player Transform the companion should follow.  " +
                 "If left empty the companion looks for a GameObject tagged 'Player'.")]
        public Transform playerTransform;

        [Header("Follow Settings")]
        [Tooltip("World-space offset from the player position.")]
        public Vector3 followOffset = new Vector3(1.5f, 0f, 0f);

        [Tooltip("Lerp factor for smooth following (units/second).")]
        [Range(1f, 20f)]
        public float followSpeed = 5f;

        [Header("Part Sprite Renderers")]
        [Tooltip("SpriteRenderer for the Head part layer.")]
        public SpriteRenderer headRenderer;

        [Tooltip("SpriteRenderer for the Body part layer.")]
        public SpriteRenderer bodyRenderer;

        [Tooltip("SpriteRenderer for the Arms part layer.")]
        public SpriteRenderer armsRenderer;

        [Tooltip("SpriteRenderer for the Legs part layer.")]
        public SpriteRenderer legsRenderer;

        [Tooltip("SpriteRenderer for the Power Core part layer.")]
        public SpriteRenderer powerCoreRenderer;

        // ------------------------------------------------------------------ //
        //  Unity lifecycle
        // ------------------------------------------------------------------ //

        private void Start()
        {
            if (playerTransform == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                    playerTransform = playerObj.transform;
                else
                    Debug.LogWarning("RobotCompanion: no Transform tagged 'Player' found.");
            }

            RefreshVisibility();

            // Listen for future assembly changes (e.g. first-time build during play).
            if (RobotInventory.Instance != null)
                RobotInventory.Instance.OnAssemblyChanged += RefreshVisibility;
        }

        private void OnDestroy()
        {
            if (RobotInventory.Instance != null)
                RobotInventory.Instance.OnAssemblyChanged -= RefreshVisibility;
        }

        private void Update()
        {
            if (playerTransform == null) return;
            if (!gameObject.activeSelf) return;

            Vector3 targetPos = playerTransform.position + followOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }

        // ------------------------------------------------------------------ //
        //  Visibility / sprite refresh
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Show or hide the companion and update part sprites based on the
        /// current <see cref="RobotInventory"/> assembly state.
        /// </summary>
        public void RefreshVisibility()
        {
            if (RobotInventory.Instance == null)
            {
                gameObject.SetActive(false);
                return;
            }

            bool assembled = RobotInventory.Instance.IsRobotAssembled();
            gameObject.SetActive(assembled);

            if (assembled)
                ApplyPartSprites();
        }

        private void ApplyPartSprites()
        {
            ApplySprite(headRenderer,      RobotPartType.Head);
            ApplySprite(bodyRenderer,      RobotPartType.Body);
            ApplySprite(armsRenderer,      RobotPartType.Arms);
            ApplySprite(legsRenderer,      RobotPartType.Legs);
            ApplySprite(powerCoreRenderer, RobotPartType.PowerCore);
        }

        private void ApplySprite(SpriteRenderer renderer, RobotPartType partType)
        {
            if (renderer == null) return;

            RobotPartData part = RobotInventory.Instance.GetAssembledPart(partType);
            renderer.sprite  = part?.sprite;
            renderer.enabled = part != null;
        }
    }
}
