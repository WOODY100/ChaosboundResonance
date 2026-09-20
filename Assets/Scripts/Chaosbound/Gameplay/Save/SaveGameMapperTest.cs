using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.MetaProgression.Persistent;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveGameMapperTest
        : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private ItemBaseData equipmentBaseData;

        [ContextMenu("Run Save Game Mapper Test")]
        private void RunTest()
        {
            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Missing ItemDatabase.");
                return;
            }

            if (equipmentBaseData == null)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Missing Equipment ItemBaseData.");
                return;
            }

            if (equipmentBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Test ItemBaseData is not Equipment.");
                return;
            }

            if (equipmentBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
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
                    "save-game-mapper-test-item",
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
                    "[Save Game Mapper Test] " +
                    "Failed to create test ItemInstance stat.");
                return;
            }

            if (!inventoryState.Items.TryAdd(item))
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Failed to add ItemInstance to Inventory.");
                return;
            }

            if (!inventoryState.Materials.Add(
                    "wood",
                    125))
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
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
                    "[Save Game Mapper Test] " +
                    "Failed to equip test ItemInstance.");
                return;
            }

            if (replacedItem != null)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Test equipment unexpectedly replaced another item.");
                return;
            }

            //==================================================
            // Runtime Meta
            //==================================================

            PersistentMetaState metaState =
                new PersistentMetaState();

            metaState.AddExperience(2500);

            //==================================================
            // Root Mapping
            //==================================================

            SaveGameData saveData =
                SaveGameMapper.ToSaveData(
                    1,
                    inventoryState,
                    equipmentLoadout,
                    metaState);

            if (saveData == null)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "SaveGameData is null.");
                return;
            }

            //==================================================
            // Validate Root
            //==================================================

            if (saveData.SaveVersion != 1)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    $"Invalid SaveVersion. " +
                    $"Expected=1, " +
                    $"Actual={saveData.SaveVersion}.");
                return;
            }

            if (saveData.Inventory == null)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Inventory SaveData is null.");
                return;
            }

            if (saveData.Equipment == null)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Equipment SaveData is null.");
                return;
            }

            if (saveData.MetaProgression == null)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Meta Progression SaveData is null.");
                return;
            }

            //==================================================
            // Validate Inventory
            //==================================================

            if (saveData.Inventory.Items.Count != 1)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Inventory SaveData item count mismatch.");
                return;
            }

            SaveItemInstanceData savedItem =
                saveData.Inventory.Items[0];

            if (savedItem.InstanceId !=
                item.InstanceId)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Inventory ItemInstanceId mismatch.");
                return;
            }

            if (saveData.Inventory.Materials.Materials.Count != 1)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Inventory material count mismatch.");
                return;
            }

            SaveMaterialAmountData savedMaterial =
                saveData.Inventory.Materials.Materials[0];

            if (savedMaterial.ContentId != "wood" ||
                savedMaterial.Amount != 125)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Inventory material data mismatch.");
                return;
            }

            //==================================================
            // Validate Equipment
            //==================================================

            if (saveData.Equipment.EquippedItems.Count != 1)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Equipment SaveData count mismatch.");
                return;
            }

            SaveEquippedItemData savedEquipment =
                saveData.Equipment.EquippedItems[0];

            if (savedEquipment.ItemInstanceId !=
                item.InstanceId)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Equipment ItemInstanceId mismatch.");
                return;
            }

            if (savedEquipment.EquipmentType !=
                equipmentBaseData.EquipmentType)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "EquipmentType mismatch.");
                return;
            }

            //==================================================
            // Validate Meta
            //==================================================

            if (saveData.MetaProgression.Experience !=
                2500)
            {
                Debug.LogError(
                    "[Save Game Mapper Test] " +
                    "Meta Progression Experience mismatch.");
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[Save Game Mapper Test] " +
                "✓ COMPLETE — SaveGameMapper correctly composed " +
                "Inventory, Equipment, and Meta Progression into " +
                "a single SaveGameData.");
        }
    }
}