using Chaosbound.Core.Composition;
using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentComparisonUnlockedStatRuntimeTest :
        MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField]
        private EquipmentStatDatabase statDatabase;

        [SerializeField]
        private ItemBaseData candidateItemData;

        [SerializeField]
        private ItemBaseData equippedItemData;

        [SerializeField]
        private StatType unlockedStatType =
            StatType.CritChance;

        [SerializeField]
        private ModifierType unlockedModifierType =
            ModifierType.Percent;

        [SerializeField]
        private float rolledValue = 5f;

        [SerializeField]
        private int candidateUpgradeLevel = 3;

        [ContextMenu("Run Unlocked Stat Comparison Test")]
        private void RunTest()
        {
            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Missing EquipmentStatDatabase.");

                return;
            }

            if (candidateItemData == null)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Missing Candidate ItemBaseData.");

                return;
            }

            if (equippedItemData == null)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Missing Equipped ItemBaseData.");

                return;
            }

            if (candidateItemData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Candidate item is not Equipment.");

                return;
            }

            if (equippedItemData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Equipped item is not Equipment.");

                return;
            }

            if (candidateItemData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Candidate item has EquipmentType.None.");

                return;
            }

            if (equippedItemData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Equipped item has EquipmentType.None.");

                return;
            }

            if (candidateItemData.EquipmentType !=
                equippedItemData.EquipmentType)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Candidate and equipped items must have " +
                    "the same EquipmentType.");

                return;
            }

            if (candidateUpgradeLevel < 0)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Candidate UpgradeLevel cannot be negative.");

                return;
            }

            if (candidateItemData.BaseStats != null)
            {
                for (int i = 0;
                     i < candidateItemData.BaseStats.Count;
                     i++)
                {
                    EquipmentBaseStat baseStat =
                        candidateItemData.BaseStats[i];

                    if (baseStat.StatType != unlockedStatType)
                        continue;

                    Debug.LogError(
                        "[Equipment Unlocked Stat Test] " +
                        $"Candidate already has {unlockedStatType} " +
                        "as a BaseStat. Choose a different unlocked " +
                        "stat for this test.");

                    return;
                }
            }

            if (!statDatabase.TryGetDefinition(
                    unlockedStatType,
                    out EquipmentStatDefinition definition))
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    $"No EquipmentStatDefinition found for " +
                    $"{unlockedStatType}.");

                return;
            }

            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "GameContentContext.Current is null. " +
                    "Enter Play Mode before running the test.");

                return;
            }

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            if (contentResolver == null)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "ItemContentResolver is null.");

                return;
            }

            EquipmentStatResolver statResolver =
                new EquipmentStatResolver(
                    statDatabase);

            ItemComparisonService comparisonService =
                new ItemComparisonService(
                    contentResolver,
                    statResolver);

            ItemInstance candidateItem =
                new ItemInstance(
                    "unlocked-stat-test-candidate",
                    candidateItemData.ContentId,
                    candidateItemData.BaseTier,
                    candidateUpgradeLevel);

            ItemInstance equippedItem =
                new ItemInstance(
                    "unlocked-stat-test-equipped",
                    equippedItemData.ContentId,
                    equippedItemData.BaseTier);

            EquipmentRolledStat unlockedStat =
                new EquipmentRolledStat(
                    unlockedStatType,
                    unlockedModifierType,
                    rolledValue);

            bool added =
                candidateItem.TryAddUnlockedStat(
                    unlockedStat);

            if (!added)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Failed to add unlocked stat to candidate.");

                return;
            }

            float expectedValue =
                rolledValue +
                (definition.UpgradeGrowth *
                 candidateUpgradeLevel);

            Debug.Log(
                "[Equipment Unlocked Stat Test] " +
                "========== TEST START ==========");

            Debug.Log(
                $"[Equipment Unlocked Stat Test] " +
                $"Stat: {unlockedStatType}");

            Debug.Log(
                $"[Equipment Unlocked Stat Test] " +
                $"Rolled Value: {rolledValue}");

            Debug.Log(
                $"[Equipment Unlocked Stat Test] " +
                $"Upgrade Growth: {definition.UpgradeGrowth}");

            Debug.Log(
                $"[Equipment Unlocked Stat Test] " +
                $"Upgrade Level: {candidateUpgradeLevel}");

            Debug.Log(
                $"[Equipment Unlocked Stat Test] " +
                $"Expected Resolved Value: {expectedValue}");

            bool comparisonSucceeded =
                comparisonService.TryCompare(
                    candidateItem,
                    equippedItem,
                    out ItemComparisonResult result);

            if (!comparisonSucceeded)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Comparison failed.");

                return;
            }

            if (result == null)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Comparison succeeded but result is null.");

                return;
            }

            EquipmentComparisonStat comparisonStat =
                default;

            bool found =
                false;

            for (int i = 0;
                 i < result.Stats.Count;
                 i++)
            {
                EquipmentComparisonStat stat =
                    result.Stats[i];

                if (stat.StatType != unlockedStatType)
                    continue;

                comparisonStat = stat;
                found = true;

                break;
            }

            if (!found)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    $"Comparison result does not contain " +
                    $"{unlockedStatType}.");

                return;
            }

            if (!comparisonStat.HasCandidateValue)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Candidate stat was not resolved.");

                return;
            }

            if (comparisonStat.HasEquippedValue)
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    "Equipped item unexpectedly contains " +
                    $"{unlockedStatType}.");

                return;
            }

            float actualValue =
                comparisonStat.CandidateValue;

            if (!Mathf.Approximately(
                    actualValue,
                    expectedValue))
            {
                Debug.LogError(
                    "[Equipment Unlocked Stat Test] " +
                    $"Resolved value mismatch. " +
                    $"Expected={expectedValue}, " +
                    $"Actual={actualValue}.");

                return;
            }

            Debug.Log(
                "[Equipment Unlocked Stat Test] " +
                $"Candidate resolved {unlockedStatType} " +
                $"correctly: {actualValue}");

            Debug.Log(
                "[Equipment Unlocked Stat Test] " +
                "✓ COMPLETE — Unlocked Stat, " +
                "Upgrade Growth and Item Comparison are " +
                "working correctly.");
        }
    }
}