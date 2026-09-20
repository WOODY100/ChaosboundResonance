using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveInventoryMapperTest : MonoBehaviour
    {
        [ContextMenu("Run Save Inventory Mapper Test")]
        private void RunTest()
        {
            PersistentInventoryState state =
                new PersistentInventoryState();

            // -------------------------------------------------
            // Item 1
            // -------------------------------------------------

            ItemInstance sword =
                new ItemInstance(
                    "save-inventory-test-sword",
                    "item_sword_test",
                    ItemTier.Rare,
                    3);

            if (!sword.TryAddUnlockedStat(
                    new EquipmentRolledStat(
                        StatType.Damage,
                        ModifierType.Flat,
                        12.5f)))
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Failed to add sword stat.");
                return;
            }

            if (!state.Items.TryAdd(sword))
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Failed to add sword.");
                return;
            }

            // -------------------------------------------------
            // Item 2
            // -------------------------------------------------

            ItemInstance helmet =
                new ItemInstance(
                    "save-inventory-test-helmet",
                    "item_helmet_test",
                    ItemTier.Uncommon,
                    2);

            if (!state.Items.TryAdd(helmet))
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Failed to add helmet.");
                return;
            }

            // -------------------------------------------------
            // Materials
            // -------------------------------------------------

            if (!state.Materials.Add(
                    "material_iron",
                    125))
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Failed to add iron material.");
                return;
            }

            if (!state.Materials.Add(
                    "material_crystal",
                    42))
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Failed to add crystal material.");
                return;
            }

            // -------------------------------------------------
            // Seen State
            // -------------------------------------------------

            state.ItemSeenState.MarkSeen(
                sword.InstanceId);

            state.MaterialSeenState.MarkSeen(
                "material_iron");

            // -------------------------------------------------
            // Map
            // -------------------------------------------------

            SaveInventoryData saveData =
                SaveInventoryMapper.ToSaveData(state);

            if (saveData == null)
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Save data is null.");
                return;
            }

            // -------------------------------------------------
            // Validate Items
            // -------------------------------------------------

            if (saveData.Items == null)
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Items list is null.");
                return;
            }

            if (saveData.Items.Count != 2)
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    $"Invalid item count. " +
                    $"Expected=2, Actual={saveData.Items.Count}.");
                return;
            }

            // -------------------------------------------------
            // Validate Materials
            // -------------------------------------------------

            if (saveData.Materials == null)
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Materials data is null.");
                return;
            }

            if (saveData.Materials.Materials.Count != 2)
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    $"Invalid material count. " +
                    $"Expected=2, " +
                    $"Actual={saveData.Materials.Materials.Count}.");
                return;
            }

            if (!ContainsMaterial(
                    saveData.Materials.Materials,
                    "material_iron",
                    125))
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Iron material data mismatch.");
                return;
            }

            if (!ContainsMaterial(
                    saveData.Materials.Materials,
                    "material_crystal",
                    42))
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Crystal material data mismatch.");
                return;
            }

            // -------------------------------------------------
            // Validate Item Seen State
            // -------------------------------------------------

            if (saveData.ItemSeenState == null)
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Item Seen State is null.");
                return;
            }

            if (!saveData.ItemSeenState.Ids.Contains(
                    sword.InstanceId))
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Sword InstanceId was not saved " +
                    "as seen.");
                return;
            }

            // -------------------------------------------------
            // Validate Material Seen State
            // -------------------------------------------------

            if (saveData.MaterialSeenState == null)
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Material Seen State is null.");
                return;
            }

            if (!saveData.MaterialSeenState.Ids.Contains(
                    "material_iron"))
            {
                Debug.LogError(
                    "[Save Inventory Mapper Test] " +
                    "Iron material was not saved as seen.");
                return;
            }

            // -------------------------------------------------
            // Complete
            // -------------------------------------------------

            Debug.Log(
                "[Save Inventory Mapper Test] ✓ COMPLETE — " +
                "PersistentInventoryState was mapped correctly " +
                "to SaveInventoryData.");
        }

        private static bool ContainsMaterial(
            List<SaveMaterialAmountData> materials,
            string contentId,
            int amount)
        {
            for (int i = 0;
                 i < materials.Count;
                 i++)
            {
                SaveMaterialAmountData material =
                    materials[i];

                if (material == null)
                    continue;

                if (material.ContentId == contentId &&
                    material.Amount == amount)
                {
                    return true;
                }
            }

            return false;
        }
    }
}