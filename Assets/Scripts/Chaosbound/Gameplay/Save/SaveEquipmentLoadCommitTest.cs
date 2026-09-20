using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveEquipmentLoadCommitTest
        : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private ItemBaseData mainWeaponBaseData;
        [SerializeField] private ItemBaseData helmetBaseData;

        [ContextMenu("Run Save Equipment Load Commit Test")]
        private void RunTest()
        {
            GameContentContext context =
                GameContentContext.Current;

            if (context == null)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "GameContentContext.Current is null.");
                return;
            }

            if (context.ItemContentResolver == null)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "ItemContentResolver is null.");
                return;
            }

            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Missing ItemDatabase.");
                return;
            }

            if (mainWeaponBaseData == null)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Missing Main Weapon ItemBaseData.");
                return;
            }

            if (helmetBaseData == null)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Missing Helmet ItemBaseData.");
                return;
            }

            if (mainWeaponBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Main weapon is not Equipment.");
                return;
            }

            if (helmetBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Helmet is not Equipment.");
                return;
            }

            if (mainWeaponBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Main weapon has EquipmentType.None.");
                return;
            }

            if (helmetBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Helmet has EquipmentType.None.");
                return;
            }

            if (mainWeaponBaseData.EquipmentType ==
                helmetBaseData.EquipmentType)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Test items use the same EquipmentType.");
                return;
            }

            //==================================================
            // Runtime Inventory
            //==================================================

            PersistentInventoryState inventoryState =
                new PersistentInventoryState();

            ItemInstance weapon =
                new ItemInstance(
                    "equipment-commit-test-weapon",
                    mainWeaponBaseData.ContentId,
                    ItemTier.Rare,
                    3);

            ItemInstance helmet =
                new ItemInstance(
                    "equipment-commit-test-helmet",
                    helmetBaseData.ContentId,
                    ItemTier.Uncommon,
                    2);

            if (!inventoryState.Items.TryAdd(
                weapon))
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Failed to add weapon to Inventory.");
                return;
            }

            if (!inventoryState.Items.TryAdd(
                    helmet))
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Failed to add helmet to Inventory.");
                return;
            }

            SaveInventoryLoadPlan inventoryLoadPlan =
                new SaveInventoryLoadPlan();

            inventoryLoadPlan.Items.Add(
                weapon);

            inventoryLoadPlan.Items.Add(
                helmet);

            //==================================================
            // Existing Equipment State
            //==================================================

            EquipmentLoadoutRuntime loadout =
                new EquipmentLoadoutRuntime();

            ItemInstance oldWeapon =
                new ItemInstance(
                    "equipment-commit-test-old-weapon",
                    mainWeaponBaseData.ContentId,
                    ItemTier.Common,
                    0);

            if (!loadout.TryEquip(
                    oldWeapon,
                    mainWeaponBaseData,
                    out ItemInstance replacedOld))
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Failed to create old equipment state.");
                return;
            }

            if (replacedOld != null)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Old equipment unexpectedly replaced another item.");
                return;
            }

            //==================================================
            // Save Data To Load
            //==================================================

            SaveEquipmentData saveData =
                new SaveEquipmentData();

            saveData.EquippedItems.Add(
                new SaveEquippedItemData
                {
                    EquipmentType =
                        mainWeaponBaseData.EquipmentType,

                    ItemInstanceId =
                        weapon.InstanceId
                });

            saveData.EquippedItems.Add(
                new SaveEquippedItemData
                {
                    EquipmentType =
                        helmetBaseData.EquipmentType,

                    ItemInstanceId =
                        helmet.InstanceId
                });

            //==================================================
            // Prepare
            //==================================================

            if (!SaveEquipmentMapper.TryPrepareLoad(
                saveData,
                inventoryLoadPlan,
                context.ItemContentResolver,
                out SaveEquipmentLoadPlan loadPlan))
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Failed to prepare valid Equipment Save.");
                return;
            }

            //==================================================
            // Commit
            //==================================================

            SaveEquipmentMapper.ApplyLoadPlan(
                loadPlan,
                loadout);

            //==================================================
            // Validate Old Equipment Was Removed
            //==================================================

            if (loadout.IsEquipped(
                    oldWeapon))
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Old equipment reference was not cleared.");
                return;
            }

            //==================================================
            // Validate Main Weapon
            //==================================================

            if (!loadout.TryGetEquipped(
                    mainWeaponBaseData.EquipmentType,
                    out ItemInstance restoredWeapon))
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Main weapon was not restored.");
                return;
            }

            if (restoredWeapon != weapon)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Restored weapon is not the same ItemInstance " +
                    "owned by Inventory.");
                return;
            }

            //==================================================
            // Validate Helmet
            //==================================================

            if (!loadout.TryGetEquipped(
                    helmetBaseData.EquipmentType,
                    out ItemInstance restoredHelmet))
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Helmet was not restored.");
                return;
            }

            if (restoredHelmet != helmet)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Restored helmet is not the same ItemInstance " +
                    "owned by Inventory.");
                return;
            }

            //==================================================
            // Validate Equipment Count
            //==================================================

            if (loadout.EquippedCount != 2)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    $"Invalid EquippedCount. " +
                    $"Expected=2, " +
                    $"Actual={loadout.EquippedCount}.");
                return;
            }

            //==================================================
            // Validate Inventory Was Not Modified
            //==================================================

            if (inventoryState.Items.Count != 2)
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Equipment Commit modified Inventory count.");
                return;
            }

            if (!inventoryState.Items.Contains(
                    weapon) ||
                !inventoryState.Items.Contains(
                    helmet))
            {
                Debug.LogError(
                    "[Save Equipment Load Commit Test] " +
                    "Equipment Commit modified Inventory contents.");
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[Save Equipment Load Commit Test] " +
                "✓ COMPLETE — Equipment Loadout was replaced " +
                "correctly using the same ItemInstances owned by " +
                "Inventory, without modifying Inventory.");
        }
    }
}