using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TaekwondoTech.UI
{
    using TaekwondoTech.Core;

    // ======================================================================
    //  Drag-and-drop item that lives in the inventory scroll panel.
    // ======================================================================

    /// <summary>
    /// Attach to each inventory icon.  Handles drag begin / drag / end via
    /// Unity's <see cref="IBeginDragHandler"/> / <see cref="IDragHandler"/> /
    /// <see cref="IEndDragHandler"/> interfaces.
    /// </summary>
    public class DraggablePartUI : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RobotPartData PartData { get; set; }

        private Canvas       _canvas;
        private CanvasGroup  _canvasGroup;
        private Transform    _originalParent;
        private Vector3      _originalPosition;
        private RectTransform _rect;

        private void Awake()
        {
            _rect        = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public void Initialise(RobotPartData part, Canvas rootCanvas)
        {
            PartData = part;
            _canvas  = rootCanvas;

            var img = GetComponent<Image>();
            if (img != null && part.sprite != null)
                img.sprite = part.sprite;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalParent   = transform.parent;
            _originalPosition = transform.position;

            // Reparent to canvas root so the icon renders on top of everything.
            transform.SetParent(_canvas.transform, true);
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;

            // If not dropped on a valid slot the icon snaps back.
            if (transform.parent == _canvas.transform)
            {
                transform.SetParent(_originalParent, true);
                transform.position = _originalPosition;
            }
        }
    }

    // ======================================================================
    //  Drop zone (blueprint slot).
    // ======================================================================

    /// <summary>
    /// One of the five blueprint drop zones in the craft scene.
    /// Accepts a <see cref="DraggablePartUI"/> whose
    /// <see cref="RobotPartData.partType"/> matches <see cref="acceptedType"/>.
    /// </summary>
    public class BlueprintSlotUI : MonoBehaviour, IDropHandler
    {
        [Tooltip("The part type this slot accepts.")]
        public RobotPartType acceptedType;

        [Tooltip("Image shown when the slot is empty.")]
        public Image emptyImage;

        [Tooltip("Image shown when the slot is filled.")]
        public Image filledImage;

        /// <summary>The part currently placed in this slot, or null.</summary>
        public RobotPartData CurrentPart { get; private set; }

        public event System.Action<BlueprintSlotUI> OnSlotChanged;

        public void OnDrop(PointerEventData eventData)
        {
            var draggable = eventData.pointerDrag?.GetComponent<DraggablePartUI>();
            if (draggable == null) return;
            if (draggable.PartData.partType != acceptedType)
            {
                Debug.Log($"BlueprintSlotUI: expected {acceptedType}, got {draggable.PartData.partType}.");
                return;
            }

            PlacePart(draggable.PartData);

            // Snap the icon into this slot.
            draggable.transform.SetParent(transform, false);
            draggable.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        public void PlacePart(RobotPartData part)
        {
            CurrentPart = part;

            if (emptyImage  != null) emptyImage.enabled  = part == null;
            if (filledImage != null)
            {
                filledImage.enabled = part != null;
                if (part?.sprite != null)
                    filledImage.sprite = part.sprite;
            }

            // Update RobotInventory assembly.
            if (part != null)
                RobotInventory.Instance?.AssemblePart(part);
            else
                RobotInventory.Instance?.UnassemblePart(acceptedType);

            OnSlotChanged?.Invoke(this);
        }

        public void ClearSlot()
        {
            PlacePart(null);
        }
    }

    // ======================================================================
    //  Main CraftScene UI controller.
    // ======================================================================

    /// <summary>
    /// Root controller for the Craft Scene.
    ///
    /// Responsibilities:
    ///   • Populate the inventory scroll panel with <see cref="DraggablePartUI"/>
    ///     icons for every part in <see cref="RobotInventory"/>.
    ///   • Manage the five <see cref="BlueprintSlotUI"/> drop zones.
    ///   • Show / hide the Build Robot button (active only when all slots filled).
    ///   • Restore any previously assembled slots on scene load.
    ///   • Save on "Build Robot" and on scene unload.
    /// </summary>
    public class CraftSceneUI : MonoBehaviour
    {
        [Header("Inventory Panel")]
        [Tooltip("Scroll Rect content transform — inventory icons are added here.")]
        public Transform inventoryContent;

        [Tooltip("Prefab with Image + DraggablePartUI components.")]
        public GameObject draggablePartPrefab;

        [Header("Blueprint Slots")]
        [Tooltip("Five slots — one per RobotPartType.  Order: Head, Body, Arms, Legs, PowerCore.")]
        public BlueprintSlotUI[] blueprintSlots;

        [Header("Build Button")]
        public Button buildRobotButton;

        [Header("Root Canvas (for drag reparenting)")]
        public Canvas rootCanvas;

        [Header("Navigation")]
        [Tooltip("Scene to load after the robot is successfully built.")]
        public string nextSceneName = "MainMenu";

        private readonly List<DraggablePartUI> _inventoryIcons = new List<DraggablePartUI>();

        // ------------------------------------------------------------------ //
        //  Unity lifecycle
        // ------------------------------------------------------------------ //

        private void Start()
        {
            // Subscribe to blueprint slot changes so we can update the button.
            foreach (var slot in blueprintSlots)
                slot.OnSlotChanged += _ => RefreshBuildButton();

            buildRobotButton.onClick.AddListener(OnBuildRobotClicked);

            PopulateInventory();
            RestoreAssembly();
            RefreshBuildButton();
        }

        private void OnDestroy()
        {
            // Auto-save when the scene is unloaded.
            SaveManager.Save();
        }

        // ------------------------------------------------------------------ //
        //  Inventory population
        // ------------------------------------------------------------------ //

        private void PopulateInventory()
        {
            if (RobotInventory.Instance == null) return;

            foreach (Transform child in inventoryContent)
                Destroy(child.gameObject);

            _inventoryIcons.Clear();

            foreach (var part in RobotInventory.Instance.CollectedParts)
            {
                if (draggablePartPrefab == null) break;

                GameObject icon = Instantiate(draggablePartPrefab, inventoryContent);
                var draggable   = icon.GetComponent<DraggablePartUI>();
                if (draggable == null) draggable = icon.AddComponent<DraggablePartUI>();

                draggable.Initialise(part, rootCanvas);
                _inventoryIcons.Add(draggable);
            }
        }

        // ------------------------------------------------------------------ //
        //  Restore assembly from RobotInventory (populated by SaveManager.Load)
        // ------------------------------------------------------------------ //

        private void RestoreAssembly()
        {
            if (RobotInventory.Instance == null) return;

            foreach (var slot in blueprintSlots)
            {
                var assembled = RobotInventory.Instance.GetAssembledPart(slot.acceptedType);
                if (assembled != null)
                    slot.PlacePart(assembled);
            }
        }

        // ------------------------------------------------------------------ //
        //  Build Robot button
        // ------------------------------------------------------------------ //

        private void RefreshBuildButton()
        {
            bool allFilled = RobotInventory.Instance != null && RobotInventory.Instance.IsRobotAssembled();
            buildRobotButton.interactable = allFilled;
        }

        private void OnBuildRobotClicked()
        {
            if (RobotInventory.Instance == null || !RobotInventory.Instance.IsRobotAssembled())
                return;

            SaveManager.Save();
            Debug.Log("CraftSceneUI: Robot assembled and saved!");

            // Transition back to the game / world map.
            GameManager.Instance?.LoadScene(nextSceneName);
        }
    }
}
