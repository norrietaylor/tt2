using System.Collections.Generic;
using UnityEngine;

namespace TaekwondoTech.Core
{
    /// <summary>
    /// Persistent singleton that tracks every robot part the player has collected
    /// and which parts are currently assembled on the robot blueprint.
    ///
    /// Persistence is handled by <see cref="SaveManager"/>; call
    /// <see cref="SaveManager.Save"/> / <see cref="SaveManager.Load"/> to
    /// serialise or restore state.
    /// </summary>
    public class RobotInventory : MonoBehaviour
    {
        public static RobotInventory Instance { get; private set; }

        // All parts collected during the current play-through (and loaded from save).
        private readonly List<RobotPartData> _collectedParts = new List<RobotPartData>();

        // One assembled slot per part type (null = unfilled).
        private readonly Dictionary<RobotPartType, RobotPartData> _assembledSlots =
            new Dictionary<RobotPartType, RobotPartData>();

        /// <summary>Raised whenever a part is collected.</summary>
        public event System.Action<RobotPartData> OnPartCollected;

        /// <summary>Raised whenever the assembled blueprint changes.</summary>
        public event System.Action OnAssemblyChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ------------------------------------------------------------------ //
        //  Collection
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Record a newly collected part.  Duplicate entries are allowed so the
        /// player can collect multiple copies of the same asset.
        /// </summary>
        public void CollectPart(RobotPartData part)
        {
            if (part == null)
            {
                Debug.LogWarning("RobotInventory.CollectPart: part is null.");
                return;
            }

            _collectedParts.Add(part);
            OnPartCollected?.Invoke(part);
        }

        /// <summary>Read-only snapshot of every part in the inventory.</summary>
        public IReadOnlyList<RobotPartData> CollectedParts => _collectedParts;

        // ------------------------------------------------------------------ //
        //  Assembly (Craft Scene)
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Place a part into its corresponding blueprint slot.
        /// Returns false if <paramref name="part"/> is not in the inventory.
        /// </summary>
        public bool AssemblePart(RobotPartData part)
        {
            if (part == null) return false;
            if (!_collectedParts.Contains(part))
            {
                Debug.LogWarning($"RobotInventory.AssemblePart: '{part.displayName}' is not in inventory.");
                return false;
            }

            _assembledSlots[part.partType] = part;
            OnAssemblyChanged?.Invoke();
            return true;
        }

        /// <summary>Remove the part currently in <paramref name="slotType"/>.</summary>
        public void UnassemblePart(RobotPartType slotType)
        {
            if (_assembledSlots.Remove(slotType))
                OnAssemblyChanged?.Invoke();
        }

        /// <summary>Returns the part assembled in <paramref name="slotType"/>, or null.</summary>
        public RobotPartData GetAssembledPart(RobotPartType slotType)
        {
            _assembledSlots.TryGetValue(slotType, out RobotPartData part);
            return part;
        }

        /// <summary>Read-only view of the current assembly slots.</summary>
        public IReadOnlyDictionary<RobotPartType, RobotPartData> AssembledSlots => _assembledSlots;

        // ------------------------------------------------------------------ //
        //  Legendary Mode check
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Returns true when all five part types have at least one collected entry,
        /// enabling the Legendary Mode unlock path.
        /// </summary>
        public bool HasAllParts()
        {
            var found = new HashSet<RobotPartType>();
            foreach (var part in _collectedParts)
                found.Add(part.partType);

            return found.Contains(RobotPartType.Head)
                && found.Contains(RobotPartType.Body)
                && found.Contains(RobotPartType.Arms)
                && found.Contains(RobotPartType.Legs)
                && found.Contains(RobotPartType.PowerCore);
        }

        /// <summary>
        /// Returns true when all five blueprint slots are filled, meaning the
        /// robot has been fully assembled and the companion can be spawned.
        /// </summary>
        public bool IsRobotAssembled()
        {
            return _assembledSlots.ContainsKey(RobotPartType.Head)
                && _assembledSlots.ContainsKey(RobotPartType.Body)
                && _assembledSlots.ContainsKey(RobotPartType.Arms)
                && _assembledSlots.ContainsKey(RobotPartType.Legs)
                && _assembledSlots.ContainsKey(RobotPartType.PowerCore);
        }

        // ------------------------------------------------------------------ //
        //  Internal helpers used by SaveManager
        // ------------------------------------------------------------------ //

        internal void RestoreCollectedParts(IEnumerable<RobotPartData> parts)
        {
            _collectedParts.Clear();
            foreach (var p in parts)
                if (p != null) _collectedParts.Add(p);
        }

        internal void RestoreAssembledSlots(Dictionary<RobotPartType, RobotPartData> slots)
        {
            _assembledSlots.Clear();
            foreach (var kvp in slots)
                if (kvp.Value != null) _assembledSlots[kvp.Key] = kvp.Value;
        }
    }
}
