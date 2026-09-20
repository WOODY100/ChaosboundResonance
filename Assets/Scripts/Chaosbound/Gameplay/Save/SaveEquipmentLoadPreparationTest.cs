using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveEquipmentLoadPreparationTest
        : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private ItemBaseData mainWeaponBaseData;
        [SerializeField] private ItemBaseData helmetBaseData;

        [ContextMenu("Run Save Equipment Load Preparation Test")]
        private void RunTest()
        {
            GameContentContext context =
                GameContentContext.Current;

            if (context == null)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "GameContentContext.Current is null.");
                return;
            }

            if (context.ItemContentResolver == null)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "ItemContentResolver is null.");
                return;
            }

            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Missing ItemDatabase.");
                return;
            }

            if (mainWeaponBaseData == null)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Missing Main Weapon ItemBaseData.");
                return;
            }

            if (helmetBaseData == null)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Missing Helmet ItemBaseData.");
                return;
            }

            //==================================================
            // Validate Test Content
            //==================================================

            if (!itemDatabase.TryGet(
                    mainWeaponBaseData.ContentId,
                    out ItemBaseData resolvedWeapon))
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    $"Could not resolve weapon " +
                    $"'{mainWeaponBaseData.ContentId}'.");
                return;
            }

            if (resolvedWeapon != mainWeaponBaseData)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Weapon resolved to a different ItemBaseData.");
                return;
            }

            if (!itemDatabase.TryGet(
                    helmetBaseData.ContentId,
                    out ItemBaseData resolvedHelmet))
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    $"Could not resolve helmet " +
                    $"'{helmetBaseData.ContentId}'.");
                return;
            }

            if (resolvedHelmet != helmetBaseData)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Helmet resolved to a different ItemBaseData.");
                return;
            }

            if (mainWeaponBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Main weapon is not Equipment.");
                return;
            }

            if (helmetBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Helmet is not Equipment.");
                return;
            }

            if (mainWeaponBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Main weapon has EquipmentType.None.");
                return;
            }

            if (helmetBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Helmet has EquipmentType.None.");
                return;
            }

            if (mainWeaponBaseData.EquipmentType ==
                helmetBaseData.EquipmentType)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Test items use the same EquipmentType.");
                return;
            }

            //==================================================
            // Create Inventory State
            //==================================================

            PersistentInventoryState inventoryState =
                new PersistentInventoryState();

            ItemInstance weapon =
                new ItemInstance(
                    "equipment-load-test-weapon",
                    mainWeaponBaseData.ContentId,
                    ItemTier.Rare,
                    3);

            ItemInstance helmet =
                new ItemInstance(
                    "equipment-load-test-helmet",
                    helmetBaseData.ContentId,
                    ItemTier.Uncommon,
                    2);

            if (!inventoryState.Items.TryAdd(
                    weapon))
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Failed to add weapon to Inventory.");
                return;
            }

            if (!inventoryState.Items.TryAdd(
                    helmet))
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Failed to add helmet to Inventory.");
                return;
            }

            //==================================================
            // Build Valid Equipment Save
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

            SaveInventoryLoadPlan inventoryLoadPlan =
                new SaveInventoryLoadPlan();

            inventoryLoadPlan.Items.Add(
                weapon);

            inventoryLoadPlan.Items.Add(
                helmet);

            //==================================================
            // Valid Preparation
            //==================================================

            if (!SaveEquipmentMapper.TryPrepareLoad(
                    saveData,
                    inventoryLoadPlan,
                    context.ItemContentResolver,
                    out SaveEquipmentLoadPlan plan))
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Valid Equipment Save was rejected.");
                return;
            }

            if (plan == null)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Valid Save produced a null LoadPlan.");
                return;
            }

            if (plan.EquippedItems.Count != 2)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    $"Invalid LoadPlan count. " +
                    $"Expected=2, " +
                    $"Actual={plan.EquippedItems.Count}.");
                return;
            }

            if (!plan.EquippedItems.TryGetValue(
                    mainWeaponBaseData.EquipmentType,
                    out ItemInstance preparedWeapon))
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Weapon was not prepared.");
                return;
            }

            if (preparedWeapon != weapon)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Prepared weapon is not the same ItemInstance " +
                    "owned by Inventory.");
                return;
            }

            if (!plan.EquippedItems.TryGetValue(
                    helmetBaseData.EquipmentType,
                    out ItemInstance preparedHelmet))
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Helmet was not prepared.");
                return;
            }

            if (preparedHelmet != helmet)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Prepared helmet is not the same ItemInstance " +
                    "owned by Inventory.");
                return;
            }

            //==================================================
            // Invalid Case 1:
            // InstanceId does not exist in Inventory
            //==================================================

            SaveEquipmentData missingInstanceSave =
                new SaveEquipmentData();

            missingInstanceSave.EquippedItems.Add(
                new SaveEquippedItemData
                {
                    EquipmentType =
                        mainWeaponBaseData.EquipmentType,

                    ItemInstanceId =
                        "instance-that-does-not-exist"
                });

            if (SaveEquipmentMapper.TryPrepareLoad(
                    missingInstanceSave,
                    inventoryLoadPlan,
                    context.ItemContentResolver,
                    out SaveEquipmentLoadPlan missingPlan))
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Missing ItemInstanceId was incorrectly accepted.");
                return;
            }

            if (missingPlan != null)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Invalid missing-instance Save produced a LoadPlan.");
                return;
            }

            //==================================================
            // Invalid Case 2:
            // Same ItemInstance used twice
            //==================================================

            SaveEquipmentData duplicateInstanceSave =
                new SaveEquipmentData();

            duplicateInstanceSave.EquippedItems.Add(
                new SaveEquippedItemData
                {
                    EquipmentType =
                        mainWeaponBaseData.EquipmentType,

                    ItemInstanceId =
                        weapon.InstanceId
                });

            duplicateInstanceSave.EquippedItems.Add(
                new SaveEquippedItemData
                {
                    EquipmentType =
                        helmetBaseData.EquipmentType,

                    ItemInstanceId =
                        weapon.InstanceId
                });

            if (SaveEquipmentMapper.TryPrepareLoad(
                    duplicateInstanceSave,
                    inventoryLoadPlan,
                    context.ItemContentResolver,
                    out SaveEquipmentLoadPlan duplicatePlan))
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Duplicate ItemInstanceId was incorrectly accepted.");
                return;
            }

            if (duplicatePlan != null)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Invalid duplicate-instance Save produced a LoadPlan.");
                return;
            }

            //==================================================
            // Invalid Case 3:
            // EquipmentType does not match ItemBaseData
            //==================================================

            SaveEquipmentData mismatchedTypeSave =
                new SaveEquipmentData();

            mismatchedTypeSave.EquippedItems.Add(
                new SaveEquippedItemData
                {
                    EquipmentType =
                        helmetBaseData.EquipmentType,

                    ItemInstanceId =
                        weapon.InstanceId
                });

            if (SaveEquipmentMapper.TryPrepareLoad(
                    mismatchedTypeSave,
                    inventoryLoadPlan,
                    context.ItemContentResolver,
                    out SaveEquipmentLoadPlan mismatchedPlan))
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Mismatched EquipmentType was incorrectly accepted.");
                return;
            }

            if (mismatchedPlan != null)
            {
                Debug.LogError(
                    "[Save Equipment Load Preparation Test] " +
                    "Invalid mismatched-type Save produced a LoadPlan.");
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[Save Equipment Load Preparation Test] " +
                "✓ COMPLETE — Valid equipment references were " +
                "prepared correctly and invalid references were rejected.");
        }
    }
}