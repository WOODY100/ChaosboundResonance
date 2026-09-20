using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Shared.Content.Resolution;
using UnityEngine;

namespace Chaosbound.Gameplay.Save
{
    public sealed class EquipmentInventoryServiceTest
        : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField]
        private ExpeditionRewardItemDatabase expeditionRewardItemDatabase;
        [SerializeField] private ItemBaseData equipmentBaseData;

        [ContextMenu("Run Equipment Inventory Service Test")]
        private void RunTest()
        {
            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Missing ItemDatabase.");
                return;
            }

            if (expeditionRewardItemDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Missing ExpeditionRewardItemDatabase.");
                return;
            }

            if (equipmentBaseData == null)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Missing Equipment ItemBaseData.");
                return;
            }

            if (equipmentBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Test ItemBaseData is not Equipment.");
                return;
            }

            if (equipmentBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Test ItemBaseData has EquipmentType.None.");
                return;
            }

            //==================================================
            // Runtime Setup
            //==================================================

            PersistentInventoryRuntime inventory =
                CreateInventoryRuntime();

            EquipmentLoadoutRuntime loadout =
                new EquipmentLoadoutRuntime();

            ItemContentResolver contentResolver =
                new ItemContentResolver(
                    itemDatabase,
                    expeditionRewardItemDatabase);

            EquipmentInventoryService service =
                new EquipmentInventoryService(
                    inventory,
                    contentResolver,
                    loadout);

            //==================================================
            // Create Two Instances
            //==================================================

            ItemInstance firstItem =
                new ItemInstance(
                    "equipment-service-test-first",
                    equipmentBaseData.ContentId,
                    ItemTier.Common,
                    0);

            ItemInstance secondItem =
                new ItemInstance(
                    "equipment-service-test-second",
                    equipmentBaseData.ContentId,
                    ItemTier.Rare,
                    2);

            if (!inventory.State.Items.TryAdd(
                    firstItem))
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Failed to add first item to Inventory.");
                return;
            }

            if (!inventory.State.Items.TryAdd(
                    secondItem))
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Failed to add second item to Inventory.");
                return;
            }

            //==================================================
            // Equip First Item
            //==================================================

            if (!service.TryEquip(
                    firstItem,
                    out ItemInstance replacedFirst))
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Failed to equip first item.");
                return;
            }

            if (replacedFirst != null)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "First item unexpectedly replaced another item.");
                return;
            }

            if (inventory.State.Items.Count != 2)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Equipping removed the item from Inventory.");
                return;
            }

            if (!inventory.State.Items.Contains(
                    firstItem))
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Equipped item is no longer in Inventory.");
                return;
            }

            if (!loadout.TryGetEquipped(
                    equipmentBaseData.EquipmentType,
                    out ItemInstance equippedFirst))
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "First item was not found in Equipment Loadout.");
                return;
            }

            if (equippedFirst != firstItem)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Equipment does not reference the same ItemInstance " +
                    "owned by Inventory.");
                return;
            }

            //==================================================
            // Equip Second Item — Replacement
            //==================================================

            if (!service.TryEquip(
                    secondItem,
                    out ItemInstance replacedSecond))
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Failed to equip second item.");
                return;
            }

            if (replacedSecond != firstItem)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Replacement did not return the first ItemInstance.");
                return;
            }

            if (inventory.State.Items.Count != 2)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Replacement changed Inventory item count.");
                return;
            }

            if (!inventory.State.Items.Contains(
                    firstItem))
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "First item disappeared from Inventory after replacement.");
                return;
            }

            if (!inventory.State.Items.Contains(
                    secondItem))
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Second item disappeared from Inventory after replacement.");
                return;
            }

            if (!loadout.TryGetEquipped(
                    equipmentBaseData.EquipmentType,
                    out ItemInstance equippedSecond))
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Second item was not found in Equipment Loadout.");
                return;
            }

            if (equippedSecond != secondItem)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Equipment does not reference the second ItemInstance.");
                return;
            }

            //==================================================
            // Unequip
            //==================================================

            if (!service.TryUnequip(
                    equipmentBaseData.EquipmentType,
                    out ItemInstance removedItem))
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Failed to unequip item.");
                return;
            }

            if (removedItem != secondItem)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Unequip returned the wrong ItemInstance.");
                return;
            }

            if (inventory.State.Items.Count != 2)
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Unequipping changed Inventory item count.");
                return;
            }

            if (!inventory.State.Items.Contains(
                    firstItem) ||
                !inventory.State.Items.Contains(
                    secondItem))
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "ItemInstances were removed from Inventory " +
                    "during unequip.");
                return;
            }

            if (loadout.TryGetEquipped(
                    equipmentBaseData.EquipmentType,
                    out ItemInstance remainingEquipped))
            {
                Debug.LogError(
                    "[Equipment Inventory Service Test] " +
                    "Equipment Loadout still contains an equipped item.");
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[Equipment Inventory Service Test] " +
                "✓ COMPLETE — Equipped ItemInstances remain owned " +
                "by Inventory, Equipment references the same instances, " +
                "replacement works, and unequip does not move items.");
        }

        private static PersistentInventoryRuntime
            CreateInventoryRuntime()
        {
            GameObject inventoryObject =
                new GameObject(
                    "EquipmentInventoryServiceTest_Runtime");

            PersistentInventoryRuntime inventory =
                inventoryObject.AddComponent<
                    PersistentInventoryRuntime>();

            return inventory;
        }
    }
}