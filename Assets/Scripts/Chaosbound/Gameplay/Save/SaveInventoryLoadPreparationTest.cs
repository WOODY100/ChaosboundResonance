using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveInventoryLoadPreparationTest
        : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private ItemBaseData equipmentBaseData;

        [ContextMenu("Run Save Inventory Load Preparation Test")]
        private void RunTest()
        {
            GameContentContext context =
                GameContentContext.Current;

            if (context == null)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "GameContentContext.Current is null.");
                return;
            }

            if (context.ItemContentResolver == null)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "ItemContentResolver is null.");
                return;
            }

            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Missing ItemDatabase.");
                return;
            }

            if (equipmentBaseData == null)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Missing Equipment ItemBaseData.");
                return;
            }

            if (!itemDatabase.TryGet(
                    equipmentBaseData.ContentId,
                    out ItemBaseData resolvedBaseData))
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    $"Could not resolve " +
                    $"'{equipmentBaseData.ContentId}'.");
                return;
            }

            if (resolvedBaseData != equipmentBaseData)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "ItemDatabase resolved a different " +
                    "ItemBaseData.");
                return;
            }

            //==================================================
            // Build valid SaveInventoryData
            //==================================================

            ItemInstance originalItem =
                new ItemInstance(
                    "load-preparation-test-item",
                    equipmentBaseData.ContentId,
                    ItemTier.Rare,
                    3);

            if (!originalItem.TryAddUnlockedStat(
                    new EquipmentRolledStat(
                        StatType.Damage,
                        ModifierType.Flat,
                        12.5f)))
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Failed to create test stat.");
                return;
            }

            SaveInventoryData saveData =
                new SaveInventoryData();

            saveData.Items.Add(
                SaveItemInstanceMapper.ToSaveData(
                    originalItem));

            saveData.Materials =
                new SaveMaterialsData();

            saveData.Materials.Materials.Add(
                new SaveMaterialAmountData
                {
                    ContentId = "wood",
                    Amount = 125
                });

            saveData.ItemSeenState =
                new SaveSeenIdsData();

            saveData.ItemSeenState.Ids.Add(
                originalItem.InstanceId);

            saveData.MaterialSeenState =
                new SaveSeenIdsData();

            saveData.MaterialSeenState.Ids.Add(
                "wood");

            //==================================================
            // Prepare
            //==================================================

            if (!SaveInventoryMapper.TryPrepareLoad(
                    saveData,
                    context.ItemContentResolver,
                    out SaveInventoryLoadPlan plan))
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Valid SaveInventoryData was rejected.");
                return;
            }

            if (plan == null)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Load plan is null.");
                return;
            }

            //==================================================
            // Validate Items
            //==================================================

            if (plan.Items.Count != 1)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    $"Invalid item count. " +
                    $"Expected=1, Actual={plan.Items.Count}.");
                return;
            }

            ItemInstance preparedItem =
                plan.Items[0];

            if (preparedItem == null)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Prepared ItemInstance is null.");
                return;
            }

            if (preparedItem.InstanceId !=
                originalItem.InstanceId)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Prepared InstanceId mismatch.");
                return;
            }

            if (preparedItem.BaseDataId !=
                originalItem.BaseDataId)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Prepared BaseDataId mismatch.");
                return;
            }

            if (preparedItem.CurrentTier !=
                originalItem.CurrentTier)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Prepared CurrentTier mismatch.");
                return;
            }

            if (preparedItem.UpgradeLevel !=
                originalItem.UpgradeLevel)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Prepared UpgradeLevel mismatch.");
                return;
            }

            if (preparedItem.UnlockedStats.Count !=
                originalItem.UnlockedStats.Count)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Prepared UnlockedStats count mismatch.");
                return;
            }

            //==================================================
            // Validate Materials
            //==================================================

            if (!plan.Materials.TryGetValue(
                    "wood",
                    out int woodAmount))
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Wood material was not prepared.");
                return;
            }

            if (woodAmount != 125)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    $"Invalid wood amount. " +
                    $"Expected=125, Actual={woodAmount}.");
                return;
            }

            //==================================================
            // Validate Seen State
            //==================================================

            if (!plan.SeenItemInstanceIds.Contains(
                    originalItem.InstanceId))
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Item Seen State was not prepared.");
                return;
            }

            if (!plan.SeenMaterialIds.Contains(
                    "wood"))
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Material Seen State was not prepared.");
                return;
            }

            //==================================================
            // Invalid Save: duplicate InstanceId
            //==================================================

            SaveInventoryData invalidSave =
                new SaveInventoryData();

            SaveItemInstanceData firstItem =
                SaveItemInstanceMapper.ToSaveData(
                    originalItem);

            SaveItemInstanceData duplicateItem =
                SaveItemInstanceMapper.ToSaveData(
                    originalItem);

            invalidSave.Items.Add(firstItem);
            invalidSave.Items.Add(duplicateItem);

            if (SaveInventoryMapper.TryPrepareLoad(
                    invalidSave,
                    context.ItemContentResolver,
                    out SaveInventoryLoadPlan invalidPlan))
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Duplicate InstanceId was incorrectly accepted.");
                return;
            }

            if (invalidPlan != null)
            {
                Debug.LogError(
                    "[Save Inventory Load Preparation Test] " +
                    "Invalid Save produced a LoadPlan.");
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[Save Inventory Load Preparation Test] " +
                "✓ COMPLETE — Valid inventory data was prepared " +
                "correctly and duplicate InstanceIds were rejected.");
        }
    }
}