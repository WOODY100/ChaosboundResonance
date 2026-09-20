using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveItemInstanceMapperTest : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private ItemBaseData equipmentBaseData;

        [ContextMenu("Run Item Instance Mapper Test")]
        private void RunTest()
        {
            GameContentContext context =
                GameContentContext.Current;

            if (context == null)
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] " +
                    "GameContentContext.Current is null.");
                return;
            }

            if (context.ItemContentResolver == null)
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] " +
                    "ItemContentResolver is null.");
                return;
            }

            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] " +
                    "Missing ItemDatabase.");
                return;
            }

            if (equipmentBaseData == null)
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] " +
                    "Missing Equipment ItemBaseData.");
                return;
            }

            if (!itemDatabase.TryGet(
                    equipmentBaseData.ContentId,
                    out ItemBaseData resolvedBaseData))
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] Could not " +
                    $"resolve '{equipmentBaseData.ContentId}'.");
                return;
            }

            if (resolvedBaseData != equipmentBaseData)
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] ItemDatabase " +
                    "resolved a different ItemBaseData.");
                return;
            }

            ItemInstance original =
                new ItemInstance(
                    "save-roundtrip-test",
                    equipmentBaseData.ContentId,
                    ItemTier.Rare,
                    3);

            bool addedFirstStat =
                original.TryAddUnlockedStat(
                    new EquipmentRolledStat(
                        StatType.Damage,
                        ModifierType.Flat,
                        12.5f));

            bool addedSecondStat =
                original.TryAddUnlockedStat(
                    new EquipmentRolledStat(
                        StatType.CritChance,
                        ModifierType.Percent,
                        0.025f));

            if (!addedFirstStat ||
                !addedSecondStat)
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] " +
                    "Failed to create test UnlockedStats.");
                return;
            }

            SaveItemInstanceData saveData =
                SaveItemInstanceMapper.ToSaveData(
                    original);

            if (saveData == null)
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] " +
                    "Save data is null.");
                return;
            }

            if (!SaveItemInstanceMapper.TryFromSaveData(
                    saveData,
                    context.ItemContentResolver,
                    out ItemInstance restored))
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] " +
                    "Failed to restore ItemInstance.");
                return;
            }

            if (restored.InstanceId != original.InstanceId)
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] " +
                    "InstanceId mismatch.");
                return;
            }

            if (restored.BaseDataId != original.BaseDataId)
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] " +
                    "BaseDataId mismatch.");
                return;
            }

            if (restored.CurrentTier != original.CurrentTier)
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] " +
                    "CurrentTier mismatch.");
                return;
            }

            if (restored.UpgradeLevel != original.UpgradeLevel)
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] " +
                    "UpgradeLevel mismatch.");
                return;
            }

            if (restored.UnlockedStats.Count !=
                original.UnlockedStats.Count)
            {
                Debug.LogError(
                    "[Save Item Instance Mapper Test] " +
                    "UnlockedStats count mismatch.");
                return;
            }

            for (int i = 0;
                 i < original.UnlockedStats.Count;
                 i++)
            {
                EquipmentRolledStat originalStat =
                    original.UnlockedStats[i];

                EquipmentRolledStat restoredStat =
                    restored.UnlockedStats[i];

                if (originalStat.StatType !=
                    restoredStat.StatType ||
                    originalStat.ModifierType !=
                    restoredStat.ModifierType ||
                    !Mathf.Approximately(
                        originalStat.RolledValue,
                        restoredStat.RolledValue))
                {
                    Debug.LogError(
                        "[Save Item Instance Mapper Test] " +
                        $"UnlockedStat mismatch at index {i}.");
                    return;
                }
            }

            Debug.Log(
                "[Save Item Instance Mapper Test] ✓ COMPLETE — " +
                "ItemInstance round-trip preserved all persistent data.");
        }
    }
}