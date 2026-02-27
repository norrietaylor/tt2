using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaekwondoTech.Core
{
    /// <summary>
    /// Handles JSON serialisation/deserialisation of the robot inventory and
    /// assembly state to/from <see cref="PlayerPrefs"/>.
    ///
    /// Call <see cref="Save"/> before the application quits or changes scene,
    /// and <see cref="Load"/> on startup (after all <see cref="RobotPartData"/>
    /// assets have been registered via <see cref="RegisterKnownParts"/>).
    /// </summary>
    public static class SaveManager
    {
        private const string InventoryKey  = "RobotInventory_v1";
        private const string AssemblyKey   = "RobotAssembly_v1";

        // GUID → RobotPartData lookup populated by RegisterKnownParts.
        private static readonly Dictionary<string, RobotPartData> _partRegistry =
            new Dictionary<string, RobotPartData>();

        // ------------------------------------------------------------------ //
        //  Registration
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Register all known <see cref="RobotPartData"/> assets so that they
        /// can be looked up by their Unity asset GUID when loading.
        /// Call this once at startup before <see cref="Load"/>.
        /// Logs a warning if duplicate asset names are detected, as names are
        /// used as stable save identifiers.
        /// </summary>
        public static void RegisterKnownParts(IEnumerable<RobotPartData> allParts)
        {
            _partRegistry.Clear();
            foreach (var part in allParts)
            {
                if (part == null) continue;
                string guid = GetPartGuid(part);
                if (_partRegistry.ContainsKey(guid))
                    Debug.LogWarning($"SaveManager.RegisterKnownParts: duplicate asset name '{guid}'. " +
                                     "Rename one of the RobotPartData assets to avoid save corruption.");
                _partRegistry[guid] = part;
            }
        }

        // ------------------------------------------------------------------ //
        //  Save
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Serialise the current <see cref="RobotInventory"/> state to
        /// <see cref="PlayerPrefs"/> as JSON.
        /// </summary>
        public static void Save()
        {
            var inventory = RobotInventory.Instance;
            if (inventory == null)
            {
                Debug.LogWarning("SaveManager.Save: RobotInventory instance not found.");
                return;
            }

            // --- collected parts ---
            var collectedGuids = new List<string>();
            foreach (var part in inventory.CollectedParts)
                collectedGuids.Add(GetPartGuid(part));

            PlayerPrefs.SetString(InventoryKey, JsonUtility.ToJson(new StringListWrapper { items = collectedGuids }));

            // --- assembled slots ---
            var assembledPairs = new List<AssembledPair>();
            foreach (var kvp in inventory.AssembledSlots)
                assembledPairs.Add(new AssembledPair { slotType = (int)kvp.Key, partGuid = GetPartGuid(kvp.Value) });

            PlayerPrefs.SetString(AssemblyKey, JsonUtility.ToJson(new AssembledPairListWrapper { items = assembledPairs }));

            PlayerPrefs.Save();
        }

        // ------------------------------------------------------------------ //
        //  Load
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Restore <see cref="RobotInventory"/> state from <see cref="PlayerPrefs"/>.
        /// Must be called after <see cref="RegisterKnownParts"/>.
        /// </summary>
        public static void Load()
        {
            var inventory = RobotInventory.Instance;
            if (inventory == null)
            {
                Debug.LogWarning("SaveManager.Load: RobotInventory instance not found.");
                return;
            }

            // --- collected parts ---
            var collected = new List<RobotPartData>();
            string invJson = PlayerPrefs.GetString(InventoryKey, string.Empty);
            if (!string.IsNullOrEmpty(invJson))
            {
                var wrapper = JsonUtility.FromJson<StringListWrapper>(invJson);
                if (wrapper?.items != null)
                {
                    foreach (var guid in wrapper.items)
                    {
                        if (_partRegistry.TryGetValue(guid, out RobotPartData part))
                            collected.Add(part);
                        else
                            Debug.LogWarning($"SaveManager.Load: unknown part GUID '{guid}' — skipping.");
                    }
                }
            }

            inventory.RestoreCollectedParts(collected);

            // --- assembled slots ---
            var slots = new Dictionary<RobotPartType, RobotPartData>();
            string asmJson = PlayerPrefs.GetString(AssemblyKey, string.Empty);
            if (!string.IsNullOrEmpty(asmJson))
            {
                var wrapper = JsonUtility.FromJson<AssembledPairListWrapper>(asmJson);
                if (wrapper?.items != null)
                {
                    foreach (var pair in wrapper.items)
                    {
                        if (_partRegistry.TryGetValue(pair.partGuid, out RobotPartData part))
                            slots[(RobotPartType)pair.slotType] = part;
                        else
                            Debug.LogWarning($"SaveManager.Load: unknown assembled part GUID '{pair.partGuid}' — skipping.");
                    }
                }
            }

            inventory.RestoreAssembledSlots(slots);
        }

        // ------------------------------------------------------------------ //
        //  Helpers
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Clear all saved data (useful for "New Game" / reset).
        /// </summary>
        public static void DeleteSave()
        {
            PlayerPrefs.DeleteKey(InventoryKey);
            PlayerPrefs.DeleteKey(AssemblyKey);
            PlayerPrefs.Save();
        }

        private static string GetPartGuid(RobotPartData part)
        {
            // Use the asset name as a stable identifier.  In a full project this
            // would ideally be a pre-assigned GUID stored on the asset itself.
            return part != null ? part.name : string.Empty;
        }

        // ------------------------------------------------------------------ //
        //  Serialisation helpers (JsonUtility requires plain classes)
        // ------------------------------------------------------------------ //

        [Serializable]
        private class StringListWrapper
        {
            public List<string> items = new List<string>();
        }

        [Serializable]
        private class AssembledPair
        {
            public int    slotType;
            public string partGuid;
        }

        [Serializable]
        private class AssembledPairListWrapper
        {
            public List<AssembledPair> items = new List<AssembledPair>();
        }
    }
}
