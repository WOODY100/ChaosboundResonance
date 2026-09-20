using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveInventoryLoadCommitTest
        : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private ItemBaseData equipmentBaseData;

        [ContextMenu("Run Save Inventory Load Commit Test")]
        private void RunTest()
        {
            GameContentContext context =
                GameContentContext.Current;

            if (context == null)
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "GameContentContext.Current is null.");
                return;
            }

            if (context.ItemContentResolver == null)
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "ItemContentResolver is null.");
                return;
            }

            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Missing ItemDatabase.");
                return;
            }

            if (equipmentBaseData == null)
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Missing Equipment ItemBaseData.");
                return;
            }

            //==================================================
            // Create Runtime State
            //==================================================

            PersistentInventoryState state =
                new PersistentInventoryState();

            //==================================================
            // Existing Inventory State
            //==================================================

            ItemInstance oldItem =
                new ItemInstance(
                    "old-item",
                    equipmentBaseData.ContentId,
                    ItemTier.Common,
                    1);

            if (!state.Items.TryAdd(oldItem))
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Failed to add old item.");
                return;
            }

            if (!state.Materials.Add(
                    "old_material",
                    999))
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Failed to add old material.");
                return;
            }

            state.ItemSeenState.MarkSeen(
                oldItem.InstanceId);

            state.MaterialSeenState.MarkSeen(
                "old_material");

            //==================================================
            // Existing Secure Inventory
            //==================================================

            ItemInstance secureItem =
                new ItemInstance(
                    "secure-item",
                    equipmentBaseData.ContentId,
                    ItemTier.Rare,
                    2);

            if (!state.SecureInventory.TryStore(
                    0,
                    secureItem))
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Failed to store secure item.");
                return;
            }

            //==================================================
            // Save Data To Load
            //==================================================

            ItemInstance newItem =
                new ItemInstance(
                    "new-item",
                    equipmentBaseData.ContentId,
                    ItemTier.Epic,
                    4);

            if (!newItem.TryAddUnlockedStat(
                    new EquipmentRolledStat(
                        StatType.Damage,
                        ModifierType.Flat,
                        15f)))
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Failed to add new item stat.");
                return;
            }

            SaveInventoryData saveData =
                new SaveInventoryData();

            saveData.Items.Add(
                SaveItemInstanceMapper.ToSaveData(
                    newItem));

            saveData.Materials =
                new SaveMaterialsData();

            saveData.Materials.Materials.Add(
                new SaveMaterialAmountData
                {
                    ContentId = "wood",
                    Amount = 125
                });

            saveData.ItemSeenState =
                new SaveSeenIdsData();

            saveData.ItemSeenState.Ids.Add(
                newItem.InstanceId);

            saveData.MaterialSeenState =
                new SaveSeenIdsData();

            saveData.MaterialSeenState.Ids.Add(
                "wood");

            //==================================================
            // Prepare
            //==================================================

            if (!SaveInventoryMapper.TryPrepareLoad(
                    saveData,
                    context.ItemContentResolver,
                    out SaveInventoryLoadPlan loadPlan))
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Failed to prepare valid SaveInventoryData.");
                return;
            }

            //==================================================
            // Commit
            //==================================================

            SaveInventoryMapper.ApplyLoadPlan(
                loadPlan,
                state);

            //==================================================
            // Validate Inventory Replacement
            //==================================================

            if (state.Items.Count != 1)
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    $"Invalid item count. " +
                    $"Expected=1, Actual={state.Items.Count}.");
                return;
            }

            if (!state.Items.TryGetAt(
                    0,
                    out ItemInstance restoredItem))
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Could not retrieve restored item.");
                return;
            }

            if (restoredItem.InstanceId !=
                newItem.InstanceId)
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Inventory was not replaced correctly.");
                return;
            }

            //==================================================
            // Validate Old Material Was Removed
            //==================================================

            if (state.Materials.GetAmount(
                    "old_material") != 0)
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Old material was not removed.");
                return;
            }

            //==================================================
            // Validate New Material
            //==================================================

            if (state.Materials.GetAmount(
                    "wood") != 125)
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Wood amount was not restored correctly.");
                return;
            }

            //==================================================
            // Validate Seen State
            //==================================================

            if (!state.ItemSeenState.IsSeen(
                    newItem.InstanceId))
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "New item Seen State was not restored.");
                return;
            }

            if (state.ItemSeenState.IsSeen(
                    oldItem.InstanceId))
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Old item Seen State was not cleared.");
                return;
            }

            if (!state.MaterialSeenState.IsSeen(
                    "wood"))
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Wood Seen State was not restored.");
                return;
            }

            if (state.MaterialSeenState.IsSeen(
                    "old_material"))
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Old material Seen State was not cleared.");
                return;
            }

            //==================================================
            // Validate Secure Inventory Was NOT Touched
            //==================================================

            if (!state.SecureInventory.TryGetAt(
                    0,
                    out ItemInstance restoredSecureItem))
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Secure Inventory was unexpectedly cleared.");
                return;
            }

            if (restoredSecureItem.InstanceId !=
                secureItem.InstanceId)
            {
                Debug.LogError(
                    "[Save Inventory Load Commit Test] " +
                    "Secure Inventory item was modified.");
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[Save Inventory Load Commit Test] " +
                "✓ COMPLETE — Inventory, Materials, and Seen State " +
                "were replaced correctly while Secure Inventory " +
                "remained untouched.");
        }
    }
}