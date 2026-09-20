using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.MetaProgression.Persistent;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveGameServiceLoadTest : MonoBehaviour
    {
        [SerializeField]
        private ItemDatabase itemDatabase;

        [SerializeField]
        private ItemBaseData mainWeaponBaseData;

        [SerializeField]
        private ItemBaseData helmetBaseData;

        [ContextMenu("Run Save Game Service Load Test")]
        private void RunTest()
        {
            GameContentContext context =
                GameContentContext.Current;

            if (context == null)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "GameContentContext.Current is null.");
                return;
            }

            if (context.ItemContentResolver == null)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "ItemContentResolver is null.");
                return;
            }

            if (itemDatabase == null ||
                mainWeaponBaseData == null ||
                helmetBaseData == null)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Missing test references.");
                return;
            }

            if (mainWeaponBaseData.Category != ItemCategory.Equipment ||
                helmetBaseData.Category != ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Test items must be Equipment.");
                return;
            }

            if (mainWeaponBaseData.EquipmentType == EquipmentType.None ||
                helmetBaseData.EquipmentType == EquipmentType.None)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Test items require valid EquipmentTypes.");
                return;
            }

            if (mainWeaponBaseData.EquipmentType ==
                helmetBaseData.EquipmentType)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Test equipment must use different slots.");
                return;
            }

            //==================================================
            // SAVE DATA
            //==================================================

            ItemInstance savedWeapon =
                new ItemInstance(
                    "service-load-test-weapon",
                    mainWeaponBaseData.ContentId,
                    ItemTier.Rare,
                    3);

            ItemInstance savedHelmet =
                new ItemInstance(
                    "service-load-test-helmet",
                    helmetBaseData.ContentId,
                    ItemTier.Uncommon,
                    2);

            if (!savedWeapon.TryAddUnlockedStat(
                    new EquipmentRolledStat(
                        StatType.Damage,
                        ModifierType.Flat,
                        12.5f)))
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
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
                            Experience = 3500
                        }
                };

            saveData.Inventory.Items.Add(
                SaveItemInstanceMapper.ToSaveData(
                    savedWeapon));

            saveData.Inventory.Items.Add(
                SaveItemInstanceMapper.ToSaveData(
                    savedHelmet));

            saveData.Equipment.EquippedItems.Add(
                new SaveEquippedItemData
                {
                    EquipmentType =
                        mainWeaponBaseData.EquipmentType,

                    ItemInstanceId =
                        savedWeapon.InstanceId
                });

            saveData.Equipment.EquippedItems.Add(
                new SaveEquippedItemData
                {
                    EquipmentType =
                        helmetBaseData.EquipmentType,

                    ItemInstanceId =
                        savedHelmet.InstanceId
                });

            //==================================================
            // RUNTIME DESTINATION
            //==================================================

            PersistentInventoryState inventoryState =
                new PersistentInventoryState();

            EquipmentLoadoutRuntime equipmentLoadout =
                new EquipmentLoadoutRuntime();

            PersistentMetaState metaState =
                new PersistentMetaState();

            // Deliberately different initial state.
            ItemInstance oldItem =
                new ItemInstance(
                    "old-runtime-item",
                    mainWeaponBaseData.ContentId,
                    ItemTier.Common,
                    0);

            if (!inventoryState.Items.TryAdd(oldItem))
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Failed to create initial runtime state.");
                return;
            }

            metaState.AddExperience(25);

            //==================================================
            // SERVICE
            //==================================================

            FileSaveStorage storage =
                new FileSaveStorage(
                    "chaosbound_load_test.json");

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
                    "[Save Game Service Load Test] " +
                    "TryPrepareLoad failed.");
                return;
            }

            //==================================================
            // APPLY
            //==================================================

            saveService.ApplyLoadPlan(
                loadPlan,
                inventoryState,
                equipmentLoadout,
                metaState);

            //==================================================
            // INVENTORY
            //==================================================

            if (inventoryState.Items.Count != 2)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    $"Inventory count mismatch. " +
                    $"Expected=2, Actual={inventoryState.Items.Count}.");
                return;
            }

            if (inventoryState.Items.Contains(oldItem))
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Old runtime item was not replaced.");
                return;
            }

            ItemInstance loadedWeapon = null;

            foreach (ItemInstance item in inventoryState.Items.GetItems())
            {
                if (item != null &&
                    item.InstanceId == savedWeapon.InstanceId)
                {
                    loadedWeapon = item;
                    break;
                }
            }

            if (loadedWeapon == null)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Weapon was not restored.");
                return;
            }

            ItemInstance loadedHelmet = null;

            foreach (ItemInstance item in inventoryState.Items.GetItems())
            {
                if (item != null &&
                    item.InstanceId == savedHelmet.InstanceId)
                {
                    loadedHelmet = item;
                    break;
                }
            }

            if (loadedHelmet == null)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Helmet was not restored.");
                return;
            }

            //==================================================
            // EQUIPMENT
            //==================================================

            if (!equipmentLoadout.TryGetEquipped(
                    mainWeaponBaseData.EquipmentType,
                    out ItemInstance equippedWeapon))
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Weapon was not equipped.");
                return;
            }

            if (!equipmentLoadout.TryGetEquipped(
                    helmetBaseData.EquipmentType,
                    out ItemInstance equippedHelmet))
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Helmet was not equipped.");
                return;
            }

            if (equippedWeapon != loadedWeapon)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Equipment weapon is not the same ItemInstance " +
                    "held by Inventory.");
                return;
            }

            if (equippedHelmet != loadedHelmet)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Equipment helmet is not the same ItemInstance " +
                    "held by Inventory.");
                return;
            }

            //==================================================
            // META
            //==================================================

            if (metaState.Experience != 3500)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    $"Meta experience mismatch. " +
                    $"Expected=3500, " +
                    $"Actual={metaState.Experience}.");
                return;
            }

            //==================================================
            // ITEM DATA
            //==================================================

            if (loadedWeapon.CurrentTier != ItemTier.Rare)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Weapon tier was not restored.");
                return;
            }

            if (loadedWeapon.UpgradeLevel != 3)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Weapon upgrade level was not restored.");
                return;
            }

            if (loadedWeapon.UnlockedStats.Count != 1)
            {
                Debug.LogError(
                    "[Save Game Service Load Test] " +
                    "Weapon unlocked stats were not restored.");
                return;
            }

            //==================================================
            // COMPLETE
            //==================================================

            Debug.Log(
                "[Save Game Service Load Test] " +
                "✓ COMPLETE — SaveGameService successfully " +
                "prepared and applied Inventory, Equipment, " +
                "and Meta. Equipment references the same " +
                "ItemInstances held by Inventory.");
        }
    }
}