using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Items.Runtime;
using UnityEngine;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveGameServiceTryLoadTest : MonoBehaviour
    {
        [SerializeField]
        private ItemDatabase itemDatabase;

        [SerializeField]
        private ItemBaseData mainWeaponBaseData;

        [SerializeField]
        private ItemBaseData helmetBaseData;

        [ContextMenu("Run Save Game Service Try Load Test")]
        private void RunTest()
        {
            GameContentContext context =
                GameContentContext.Current;

            if (context == null)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "GameContentContext.Current is null.");
                return;
            }

            if (context.ItemContentResolver == null)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "ItemContentResolver is null.");
                return;
            }

            if (itemDatabase == null ||
                mainWeaponBaseData == null ||
                helmetBaseData == null)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "Missing test references.");
                return;
            }

            //==================================================
            // BUILD SAVE DATA
            //==================================================

            ItemInstance weapon =
                new ItemInstance(
                    "try-load-test-weapon",
                    mainWeaponBaseData.ContentId,
                    ItemTier.Rare,
                    4);

            ItemInstance helmet =
                new ItemInstance(
                    "try-load-test-helmet",
                    helmetBaseData.ContentId,
                    ItemTier.Uncommon,
                    1);

            if (!weapon.TryAddUnlockedStat(
                    new EquipmentRolledStat(
                        StatType.Damage,
                        ModifierType.Flat,
                        15f)))
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "Failed to add weapon stat.");
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
                            Experience = 4750
                        }
                };

            saveData.Inventory.Items.Add(
                SaveItemInstanceMapper.ToSaveData(
                    weapon));

            saveData.Inventory.Items.Add(
                SaveItemInstanceMapper.ToSaveData(
                    helmet));

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
            // WRITE PHYSICAL SAVE
            //==================================================

            FileSaveStorage storage =
                new FileSaveStorage(
                    "chaosbound_try_load_test.json");

            SaveGameService saveService =
                new SaveGameService(
                    storage,
                    1);

            string json =
                SaveGameJsonSerializer.Serialize(
                    saveData);

            storage.Save(json);

            if (!storage.Exists())
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "Save file was not created.");
                return;
            }

            //==================================================
            // TRY LOAD
            //==================================================

            if (!saveService.TryLoad(
                    out SaveGameLoadPlan loadPlan))
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "TryLoad failed.");
                storage.Delete();
                return;
            }

            if (loadPlan == null)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "TryLoad returned a null LoadPlan.");
                storage.Delete();
                return;
            }

            //==================================================
            // VALIDATE INVENTORY
            //==================================================

            if (loadPlan.Inventory == null)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "Inventory LoadPlan is null.");
                storage.Delete();
                return;
            }

            if (loadPlan.Inventory.Items.Count != 2)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "Inventory item count mismatch.");
                storage.Delete();
                return;
            }

            //==================================================
            // VALIDATE EQUIPMENT
            //==================================================

            if (loadPlan.Equipment == null)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "Equipment LoadPlan is null.");
                storage.Delete();
                return;
            }

            if (loadPlan.Equipment.EquippedItems.Count != 2)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "Equipment count mismatch.");
                storage.Delete();
                return;
            }

            //==================================================
            // VALIDATE META
            //==================================================

            if (loadPlan.MetaExperience != 4750)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    $"Meta experience mismatch. " +
                    $"Expected=4750, " +
                    $"Actual={loadPlan.MetaExperience}.");
                storage.Delete();
                return;
            }

            //==================================================
            // VALIDATE ITEM DATA
            //==================================================

            ItemInstance loadedWeapon = null;

            foreach (ItemInstance item in
                     loadPlan.Inventory.Items)
            {
                if (item != null &&
                    item.InstanceId == weapon.InstanceId)
                {
                    loadedWeapon = item;
                    break;
                }
            }

            if (loadedWeapon == null)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "Weapon was not restored from file.");
                storage.Delete();
                return;
            }

            if (loadedWeapon.CurrentTier !=
                ItemTier.Rare)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "Weapon tier was not restored.");
                storage.Delete();
                return;
            }

            if (loadedWeapon.UpgradeLevel != 4)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "Weapon upgrade level was not restored.");
                storage.Delete();
                return;
            }

            if (loadedWeapon.UnlockedStats.Count != 1)
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "Weapon unlocked stats were not restored.");
                storage.Delete();
                return;
            }

            //==================================================
            // CLEANUP
            //==================================================

            storage.Delete();

            if (storage.Exists())
            {
                Debug.LogError(
                    "[Save Game Service Try Load Test] " +
                    "Test save file could not be deleted.");
                return;
            }

            Debug.Log(
                "[Save Game Service Try Load Test] " +
                "✓ COMPLETE — SaveGameService successfully " +
                "read the physical save file, deserialized " +
                "SaveGameData, and prepared the complete " +
                "SaveGameLoadPlan without modifying runtime state.");
        }
    }
}