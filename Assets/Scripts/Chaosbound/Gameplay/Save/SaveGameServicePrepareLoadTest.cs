using System;
using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.MetaProgression.Persistent;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveGameServicePrepareLoadTest
        : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField]
        private ItemDatabase itemDatabase;

        [SerializeField]
        private ItemBaseData mainWeaponBaseData;

        [SerializeField]
        private ItemBaseData helmetBaseData;

        [ContextMenu("Run Save Game Service Prepare Load Test")]
        private void RunTest()
        {
            GameContentContext context =
                GameContentContext.Current;

            if (context == null)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "GameContentContext.Current is null.");
                return;
            }

            if (context.ItemContentResolver == null)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "ItemContentResolver is null.");
                return;
            }

            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Missing ItemDatabase.");
                return;
            }

            if (mainWeaponBaseData == null)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Missing Main Weapon ItemBaseData.");
                return;
            }

            if (helmetBaseData == null)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Missing Helmet ItemBaseData.");
                return;
            }

            if (mainWeaponBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Main weapon is not Equipment.");
                return;
            }

            if (helmetBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Helmet is not Equipment.");
                return;
            }

            if (mainWeaponBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Main weapon has EquipmentType.None.");
                return;
            }

            if (helmetBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Helmet has EquipmentType.None.");
                return;
            }

            if (mainWeaponBaseData.EquipmentType ==
                helmetBaseData.EquipmentType)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Test items use the same EquipmentType.");
                return;
            }

            //==================================================
            // Build Save Data
            //==================================================

            ItemInstance weapon =
                new ItemInstance(
                    "service-prepare-test-weapon",
                    mainWeaponBaseData.ContentId,
                    ItemTier.Rare,
                    3);

            ItemInstance helmet =
                new ItemInstance(
                    "service-prepare-test-helmet",
                    helmetBaseData.ContentId,
                    ItemTier.Uncommon,
                    2);

            if (!weapon.TryAddUnlockedStat(
                    new EquipmentRolledStat(
                        StatType.Damage,
                        ModifierType.Flat,
                        12.5f)))
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Failed to create weapon stat.");
                return;
            }

            SaveGameData saveData =
                new SaveGameData
                {
                    SaveVersion = 1,

                    Inventory =
                        new SaveInventoryData(),

                    Equipment =
                        new SaveEquipmentData(),

                    MetaProgression =
                        new SaveMetaProgressionData
                        {
                            Experience = 2500
                        }
                };

            //==================================================
            // Inventory Save Data
            //==================================================

            SaveItemInstanceData weaponSaveData =
                SaveItemInstanceMapper.ToSaveData(
                    weapon);

            SaveItemInstanceData helmetSaveData =
                SaveItemInstanceMapper.ToSaveData(
                    helmet);

            saveData.Inventory.Items.Add(
                weaponSaveData);

            saveData.Inventory.Items.Add(
                helmetSaveData);

            //==================================================
            // Equipment Save Data
            //==================================================

            saveData.Equipment.EquippedItems.Add(
                new SaveEquippedItemData
                {
                    EquipmentType =
                        mainWeaponBaseData.EquipmentType,

                    ItemInstanceId =
                        weapon.InstanceId
                });

            saveData.Equipment.EquippedItems.Add(
                new SaveEquippedItemData
                {
                    EquipmentType =
                        helmetBaseData.EquipmentType,

                    ItemInstanceId =
                        helmet.InstanceId
                });

            //==================================================
            // Storage / Service
            //==================================================

            FileSaveStorage storage =
                new FileSaveStorage(
                    "chaosbound_prepare_load_test.json");

            SaveGameService saveService =
                new SaveGameService(
                    storage,
                    1);

            //==================================================
            // PREPARE
            //==================================================

            if (!saveService.TryPrepareLoad(
                    saveData,
                    out SaveGameLoadPlan loadPlan))
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Failed to prepare valid SaveGameData.");
                return;
            }

            if (loadPlan == null)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "LoadPlan is null.");
                return;
            }

            //==================================================
            // Validate Inventory Plan
            //==================================================

            if (loadPlan.Inventory == null)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Inventory LoadPlan is null.");
                return;
            }

            if (loadPlan.Inventory.Items.Count != 2)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Inventory LoadPlan item count mismatch.");
                return;
            }

            ItemInstance preparedWeapon =
                null;

            ItemInstance preparedHelmet =
                null;

            for (int i = 0;
                 i < loadPlan.Inventory.Items.Count;
                 i++)
            {
                ItemInstance item =
                    loadPlan.Inventory.Items[i];

                if (item == null)
                    continue;

                if (item.InstanceId ==
                    weapon.InstanceId)
                {
                    preparedWeapon = item;
                }

                if (item.InstanceId ==
                    helmet.InstanceId)
                {
                    preparedHelmet = item;
                }
            }

            if (preparedWeapon == null ||
                preparedHelmet == null)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Inventory LoadPlan did not restore both ItemInstances.");
                return;
            }

            //==================================================
            // Validate Equipment Plan
            //==================================================

            if (loadPlan.Equipment == null)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Equipment LoadPlan is null.");
                return;
            }

            if (loadPlan.Equipment.EquippedItems.Count != 2)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Equipment LoadPlan count mismatch.");
                return;
            }

            if (!loadPlan.Equipment.EquippedItems.TryGetValue(
                    mainWeaponBaseData.EquipmentType,
                    out ItemInstance preparedWeaponEquipment))
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Main weapon was not prepared.");
                return;
            }

            if (!loadPlan.Equipment.EquippedItems.TryGetValue(
                    helmetBaseData.EquipmentType,
                    out ItemInstance preparedHelmetEquipment))
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Helmet was not prepared.");
                return;
            }

            if (preparedWeaponEquipment != preparedWeapon)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Equipment does not reference the same " +
                    "weapon ItemInstance prepared by Inventory.");
                return;
            }

            if (preparedHelmetEquipment != preparedHelmet)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    "Equipment does not reference the same " +
                    "helmet ItemInstance prepared by Inventory.");
                return;
            }

            //==================================================
            // Validate Meta
            //==================================================

            if (loadPlan.MetaExperience != 2500)
            {
                Debug.LogError(
                    "[Save Game Service Prepare Load Test] " +
                    $"Invalid MetaExperience. " +
                    $"Expected=2500, " +
                    $"Actual={loadPlan.MetaExperience}.");
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[Save Game Service Prepare Load Test] " +
                "✓ COMPLETE — SaveGameService successfully prepared " +
                "Inventory, Equipment, and Meta without modifying " +
                "runtime state.");
        }
    }
}