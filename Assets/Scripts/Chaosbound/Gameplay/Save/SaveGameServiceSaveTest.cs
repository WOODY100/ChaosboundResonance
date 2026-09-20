using System;
using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.MetaProgression.Persistent;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveGameServiceSaveTest
        : MonoBehaviour
    {
        private const string TestFileName =
            "chaosbound_save_service_test.json";

        [Header("Configuration")]
        [SerializeField]
        private ItemDatabase itemDatabase;

        [SerializeField]
        private ItemBaseData equipmentBaseData;

        [ContextMenu("Run Save Game Service Save Test")]
        private void RunTest()
        {
            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Missing ItemDatabase.");
                return;
            }

            if (equipmentBaseData == null)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Missing Equipment ItemBaseData.");
                return;
            }

            if (equipmentBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Test ItemBaseData is not Equipment.");
                return;
            }

            if (equipmentBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Test ItemBaseData has EquipmentType.None.");
                return;
            }

            //==================================================
            // Runtime Inventory
            //==================================================

            PersistentInventoryState inventoryState =
                new PersistentInventoryState();

            ItemInstance item =
                new ItemInstance(
                    "save-service-test-item",
                    equipmentBaseData.ContentId,
                    ItemTier.Rare,
                    3);

            if (!item.TryAddUnlockedStat(
                    new EquipmentRolledStat(
                        StatType.Damage,
                        ModifierType.Flat,
                        12.5f)))
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Failed to create test ItemInstance stat.");
                return;
            }

            if (!inventoryState.Items.TryAdd(
                    item))
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Failed to add ItemInstance to Inventory.");
                return;
            }

            if (!inventoryState.Materials.Add(
                    "wood",
                    125))
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Failed to add test material.");
                return;
            }

            inventoryState.ItemSeenState.MarkSeen(
                item.InstanceId);

            inventoryState.MaterialSeenState.MarkSeen(
                "wood");

            //==================================================
            // Runtime Equipment
            //==================================================

            EquipmentLoadoutRuntime equipmentLoadout =
                new EquipmentLoadoutRuntime();

            if (!equipmentLoadout.TryEquip(
                    item,
                    equipmentBaseData,
                    out ItemInstance replacedItem))
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Failed to equip test ItemInstance.");
                return;
            }

            if (replacedItem != null)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Unexpected equipment replacement.");
                return;
            }

            //==================================================
            // Runtime Meta
            //==================================================

            PersistentMetaState metaState =
                new PersistentMetaState();

            metaState.AddExperience(
                2500);

            //==================================================
            // Storage
            //==================================================

            FileSaveStorage storage =
                new FileSaveStorage(
                    TestFileName);

            try
            {
                storage.Delete();
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Failed to clean previous test file. " +
                    exception);
                return;
            }

            //==================================================
            // Save Service
            //==================================================

            SaveGameService saveService =
                new SaveGameService(
                    storage,
                    1);

            //==================================================
            // Save
            //==================================================

            try
            {
                saveService.Save(
                    inventoryState,
                    equipmentLoadout,
                    metaState);
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Save failed. " +
                    exception);
                return;
            }

            //==================================================
            // Validate File
            //==================================================

            if (!storage.Exists())
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Save completed but save file does not exist.");
                return;
            }

            if (!storage.TryLoad(
                    out string json))
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Could not read the saved JSON.");
                return;
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Saved JSON is empty.");
                return;
            }

            //==================================================
            // Validate JSON Can Be Deserialized
            //==================================================

            SaveGameData savedData;

            try
            {
                savedData =
                    SaveGameJsonSerializer.Deserialize(
                        json);
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Saved JSON could not be deserialized. " +
                    exception);
                return;
            }

            if (savedData == null)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Deserialized SaveGameData is null.");
                return;
            }

            //==================================================
            // Root Validation
            //==================================================

            if (savedData.SaveVersion != 1)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    $"Invalid SaveVersion. " +
                    $"Expected=1, " +
                    $"Actual={savedData.SaveVersion}.");
                return;
            }

            if (savedData.Inventory == null ||
                savedData.Equipment == null ||
                savedData.MetaProgression == null)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "One or more Save domains are missing.");
                return;
            }

            //==================================================
            // Inventory Validation
            //==================================================

            if (savedData.Inventory.Items.Count != 1)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Inventory item count mismatch.");
                return;
            }

            SaveItemInstanceData savedItem =
                savedData.Inventory.Items[0];

            if (savedItem.InstanceId !=
                item.InstanceId)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "ItemInstanceId mismatch.");
                return;
            }

            if (savedData.Inventory.Materials.Materials.Count != 1)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Material count mismatch.");
                return;
            }

            SaveMaterialAmountData savedMaterial =
                savedData.Inventory.Materials.Materials[0];

            if (savedMaterial.ContentId != "wood" ||
                savedMaterial.Amount != 125)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Material data mismatch.");
                return;
            }

            //==================================================
            // Equipment Validation
            //==================================================

            if (savedData.Equipment.EquippedItems.Count != 1)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Equipment count mismatch.");
                return;
            }

            SaveEquippedItemData savedEquipment =
                savedData.Equipment.EquippedItems[0];

            if (savedEquipment.ItemInstanceId !=
                item.InstanceId)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Equipment ItemInstanceId mismatch.");
                return;
            }

            if (savedEquipment.EquipmentType !=
                equipmentBaseData.EquipmentType)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "EquipmentType mismatch.");
                return;
            }

            //==================================================
            // Meta Validation
            //==================================================

            if (savedData.MetaProgression.Experience !=
                2500)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Meta Experience mismatch.");
                return;
            }

            //==================================================
            // Cleanup
            //==================================================

            try
            {
                storage.Delete();
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "[Save Game Service Save Test] " +
                    "Save succeeded but test cleanup failed. " +
                    exception);
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[Save Game Service Save Test] " +
                "✓ COMPLETE — Runtime data was mapped, serialized, " +
                "stored, read back, deserialized, and validated " +
                "successfully.");
        }
    }
}