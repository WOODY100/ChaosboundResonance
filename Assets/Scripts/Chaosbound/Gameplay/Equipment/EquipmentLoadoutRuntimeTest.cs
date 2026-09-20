using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentLoadoutRuntimeTest : MonoBehaviour
    {
        [Header("Valid Equipment")]
        [SerializeField] private ItemBaseData equipmentBaseData;

        [Header("Optional Invalid Equipment")]
        [SerializeField] private ItemBaseData nonEquipmentBaseData;
        [SerializeField] private ItemBaseData noneTypeEquipmentBaseData;

        [ContextMenu("Run Equipment Loadout Test")]
        private void RunTest()
        {
            if (equipmentBaseData == null)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Missing valid " +
                    "Equipment ItemBaseData.");
                return;
            }

            if (equipmentBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Assigned valid " +
                    "ItemBaseData is not Equipment.");
                return;
            }

            if (equipmentBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Assigned valid " +
                    "ItemBaseData has EquipmentType.None.");
                return;
            }

            EquipmentLoadoutRuntime loadout =
                new EquipmentLoadoutRuntime();

            ItemInstance firstItem =
                new ItemInstance(
                    "equipment-loadout-test-01",
                    equipmentBaseData.ContentId,
                    equipmentBaseData.BaseTier);

            ItemInstance secondItem =
                new ItemInstance(
                    "equipment-loadout-test-02",
                    equipmentBaseData.ContentId,
                    equipmentBaseData.BaseTier);

            EquipmentType equipmentType =
                equipmentBaseData.EquipmentType;

            Debug.Log(
                $"[Equipment Loadout Test] Testing slot: " +
                $"{equipmentType}");

            TestInitialEquip(
                loadout,
                firstItem,
                equipmentBaseData);

            TestGetEquipped(
                loadout,
                equipmentType,
                firstItem);

            TestDuplicateEquip(
                loadout,
                firstItem,
                equipmentBaseData);

            TestReplacement(
                loadout,
                secondItem,
                equipmentBaseData,
                firstItem,
                equipmentType);

            TestUnequip(
                loadout,
                secondItem,
                equipmentType);

            TestEmptySlot(
                loadout,
                equipmentType);

            TestInvalidEquipment(
                loadout,
                equipmentType);

            Debug.Log(
                "[Equipment Loadout Test] ✓ COMPLETE — " +
                "Equipment loadout is working correctly.");
        }

        private static void TestInitialEquip(
            EquipmentLoadoutRuntime loadout,
            ItemInstance itemInstance,
            ItemBaseData baseData)
        {
            bool equipped =
                loadout.TryEquip(
                    itemInstance,
                    baseData,
                    out ItemInstance replacedItem);

            if (!equipped)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Initial equip failed.");
                return;
            }

            if (replacedItem != null)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Initial equip should " +
                    "not replace an item.");
                return;
            }

            Debug.Log(
                "[Equipment Loadout Test] Initial equip successful.");
        }

        private static void TestGetEquipped(
            EquipmentLoadoutRuntime loadout,
            EquipmentType equipmentType,
            ItemInstance expectedItem)
        {
            bool found =
                loadout.TryGetEquipped(
                    equipmentType,
                    out ItemInstance equippedItem);

            if (!found)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Could not retrieve " +
                    "equipped item.");
                return;
            }

            if (equippedItem != expectedItem)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Retrieved item does " +
                    "not match expected instance.");
                return;
            }

            Debug.Log(
                "[Equipment Loadout Test] GetEquipped successful.");
        }

        private static void TestDuplicateEquip(
            EquipmentLoadoutRuntime loadout,
            ItemInstance itemInstance,
            ItemBaseData baseData)
        {
            bool equipped =
                loadout.TryEquip(
                    itemInstance,
                    baseData,
                    out _);

            if (equipped)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Same ItemInstance " +
                    "should not be equipped twice.");
                return;
            }

            Debug.Log(
                "[Equipment Loadout Test] Duplicate equip correctly rejected.");
        }

        private static void TestReplacement(
            EquipmentLoadoutRuntime loadout,
            ItemInstance replacementItem,
            ItemBaseData baseData,
            ItemInstance expectedReplacedItem,
            EquipmentType equipmentType)
        {
            bool equipped =
                loadout.TryEquip(
                    replacementItem,
                    baseData,
                    out ItemInstance replacedItem);

            if (!equipped)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Replacement equip failed.");
                return;
            }

            if (replacedItem != expectedReplacedItem)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Replaced item does " +
                    "not match expected instance.");
                return;
            }

            if (!loadout.TryGetEquipped(
                    equipmentType,
                    out ItemInstance currentItem))
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Replacement item could " +
                    "not be retrieved.");
                return;
            }

            if (currentItem != replacementItem)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Replacement item is " +
                    "not occupying the slot.");
                return;
            }

            Debug.Log(
                "[Equipment Loadout Test] Replacement successful.");
        }

        private static void TestUnequip(
            EquipmentLoadoutRuntime loadout,
            ItemInstance expectedItem,
            EquipmentType equipmentType)
        {
            bool unequipped =
                loadout.TryUnequip(
                    equipmentType,
                    out ItemInstance removedItem);

            if (!unequipped)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Unequip failed.");
                return;
            }

            if (removedItem != expectedItem)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Removed item does not " +
                    "match expected instance.");
                return;
            }

            if (loadout.EquippedCount != 0)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Loadout should be empty " +
                    "after unequipping the only item.");
                return;
            }

            Debug.Log(
                "[Equipment Loadout Test] Unequip successful.");
        }

        private static void TestEmptySlot(
            EquipmentLoadoutRuntime loadout,
            EquipmentType equipmentType)
        {
            bool found =
                loadout.TryGetEquipped(
                    equipmentType,
                    out _);

            if (found)
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Empty slot should not " +
                    "return an equipped item.");
                return;
            }

            Debug.Log(
                "[Equipment Loadout Test] Empty slot correctly detected.");
        }

        private static void TestInvalidEquipment(
            EquipmentLoadoutRuntime loadout,
            EquipmentType equipmentType)
        {
            ItemInstance invalidInstance =
                new ItemInstance(
                    "equipment-loadout-invalid-test",
                    "invalid-test",
                    ItemTier.Common);

            if (loadout.TryEquip(
                    invalidInstance,
                    null,
                    out _))
            {
                Debug.LogError(
                    "[Equipment Loadout Test] Null BaseData should " +
                    "be rejected.");
                return;
            }

            if (loadout.TryUnequip(
                    EquipmentType.None,
                    out _))
            {
                Debug.LogError(
                    "[Equipment Loadout Test] EquipmentType.None " +
                    "should be rejected.");
                return;
            }

            if (loadout.TryGetEquipped(
                    EquipmentType.None,
                    out _))
            {
                Debug.LogError(
                    "[Equipment Loadout Test] EquipmentType.None " +
                    "should not return an equipped item.");
                return;
            }

            Debug.Log(
                "[Equipment Loadout Test] Invalid operations correctly rejected.");
        }
    }
}