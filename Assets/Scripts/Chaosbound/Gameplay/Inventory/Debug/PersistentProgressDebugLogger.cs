using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Inventory.Runtime;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.MetaProgression.Persistent;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.Debug
{
    public sealed class PersistentProgressDebugLogger : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField]
        private bool logOnStart = false;

        private BootstrapContext bootstrapContext;

        private void Awake()
        {
            bootstrapContext = FindFirstObjectByType<BootstrapContext>();

            if (bootstrapContext == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersistentProgressDebug] " +
                    "BootstrapContext could not be found.",
                    this);
            }
        }

        private void Start()
        {
            if (logOnStart)
            {
                LogPersistentState();
            }
        }

        [ContextMenu("Log Persistent State")]
        public void LogPersistentState()
        {
            if (bootstrapContext == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersistentProgressDebug] " +
                    "BootstrapContext is not available.",
                    this);

                return;
            }

            PersistentInventoryRuntime inventoryRuntime =
                bootstrapContext.PersistentInventoryRuntime;

            PersistentMetaRuntime metaRuntime =
                bootstrapContext.PersistentMetaRuntime;

            if (inventoryRuntime == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersistentProgressDebug] " +
                    "PersistentInventoryRuntime is not assigned in BootstrapContext.",
                    this);

                return;
            }

            if (metaRuntime == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersistentProgressDebug] " +
                    "PersistentMetaRuntime is not assigned in BootstrapContext.",
                    this);

                return;
            }

            PersistentInventoryState inventoryState =
                inventoryRuntime.State;

            PersistentMetaState metaState =
                metaRuntime.State;

            if (inventoryState == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersistentProgressDebug] " +
                    "PersistentInventoryState is null.",
                    this);

                return;
            }

            if (metaState == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersistentProgressDebug] " +
                    "PersistentMetaState is null.",
                    this);

                return;
            }

            LogHeader();

            LogItems(inventoryState.Items);
            LogMaterials(inventoryState.Materials);
            LogSecureInventory(inventoryState.SecureInventory);
            LogMeta(metaState);

            LogFooter();
        }

        private void LogItems(
            PersistentItemInventoryState items)
        {
            UnityEngine.Debug.Log(
                "[PersistentProgressDebug] " +
                $"ITEMS — Count: {items.Count}");

            IReadOnlyList<ItemInstance> itemList =
                items.GetItems();

            if (itemList.Count == 0)
            {
                UnityEngine.Debug.Log(
                    "[PersistentProgressDebug] " +
                    "  No persistent items.");

                return;
            }

            for (int i = 0; i < itemList.Count; i++)
            {
                ItemInstance item = itemList[i];

                if (item == null)
                {
                    UnityEngine.Debug.LogWarning(
                        "[PersistentProgressDebug] " +
                        $"  [{i}] NULL ITEM");

                    continue;
                }

                UnityEngine.Debug.Log(
                    "[PersistentProgressDebug] " +
                    $"  [{i}] " +
                    $"BaseDataId={item.BaseDataId} | " +
                    $"InstanceId={item.InstanceId} | " +
                    $"Tier={item.CurrentTier} | " +
                    $"UpgradeLevel={item.UpgradeLevel}");
            }
        }

        private void LogMaterials(
            PersistentMaterialsState materials)
        {
            IReadOnlyDictionary<string, int> amounts =
                materials.GetAmounts();

            UnityEngine.Debug.Log(
                "[PersistentProgressDebug] " +
                $"MATERIALS — Unique Types: {amounts.Count}");

            if (amounts.Count == 0)
            {
                UnityEngine.Debug.Log(
                    "[PersistentProgressDebug] " +
                    "  No persistent materials.");

                return;
            }

            foreach (
                KeyValuePair<string, int> entry
                in amounts)
            {
                UnityEngine.Debug.Log(
                    "[PersistentProgressDebug] " +
                    $"  {entry.Key}: {entry.Value}");
            }
        }

        private void LogSecureInventory(
            SecureInventoryState secureInventory)
        {
            UnityEngine.Debug.Log(
                "[PersistentProgressDebug] " +
                "SECURE INVENTORY");

            IReadOnlyList<InventorySlot> slots =
                secureInventory.GetSlots();

            for (int i = 0; i < slots.Count; i++)
            {
                InventorySlot slot = slots[i];

                if (!secureInventory.IsUnlocked(i))
                {
                    UnityEngine.Debug.Log(
                        "[PersistentProgressDebug] " +
                        $"  [{i}] LOCKED");

                    continue;
                }

                if (slot == null || !slot.IsOccupied)
                {
                    UnityEngine.Debug.Log(
                        "[PersistentProgressDebug] " +
                        $"  [{i}] EMPTY");

                    continue;
                }

                ItemInstance item = slot.Item;

                if (item == null)
                {
                    UnityEngine.Debug.LogWarning(
                        "[PersistentProgressDebug] " +
                        $"  [{i}] NULL ITEM");

                    continue;
                }

                UnityEngine.Debug.Log(
                    "[PersistentProgressDebug] " +
                    $"  [{i}] " +
                    $"BaseDataId={item.BaseDataId} | " +
                    $"InstanceId={item.InstanceId} | " +
                    $"Tier={item.CurrentTier}");
            }
        }

        private void LogMeta(
            PersistentMetaState meta)
        {
            UnityEngine.Debug.Log(
                "[PersistentProgressDebug] " +
                $"META EXPERIENCE — {meta.Experience}");
        }

        private void LogHeader()
        {
            UnityEngine.Debug.Log(
                "========================================\n" +
                " CHAOSBOUND PERSISTENT STATE DEBUG\n" +
                "========================================");
        }

        private void LogFooter()
        {
            UnityEngine.Debug.Log(
                "========================================\n" +
                " END PERSISTENT STATE DEBUG\n" +
                "========================================");
        }
    }
}