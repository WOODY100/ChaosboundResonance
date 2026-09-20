using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentComparisonExclusiveStatsRuntimeTest :
        MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField]
        private EquipmentStatDatabase statDatabase;

        [SerializeField]
        private ItemBaseData candidateItemData;

        [SerializeField]
        private ItemBaseData equippedItemData;

        [Header("Expected Shared Stat")]
        [SerializeField]
        private StatType sharedStatType =
            StatType.Damage;

        [Header("Expected Candidate-Only Stat")]
        [SerializeField]
        private StatType candidateOnlyStatType =
            StatType.AttackSpeed;

        [Header("Expected Equipped-Only Stat")]
        [SerializeField]
        private StatType equippedOnlyStatType =
            StatType.Luck;

        [ContextMenu("Run Exclusive Stats Comparison Test")]
        private void RunTest()
        {
            int passedChecks = 0;
            int totalChecks = 0;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "==================================================");

            Debug.Log(
                "[Exclusive Stats Test] " +
                "START — Shared and exclusive stat validation");

            Debug.Log(
                "[Exclusive Stats Test] " +
                "==================================================");

            //==========================================================
            // Reference Validation
            //==========================================================

            totalChecks++;

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 1 FAILED — EquipmentStatDatabase is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 1 PASSED — EquipmentStatDatabase assigned.");

            totalChecks++;

            if (candidateItemData == null)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 2 FAILED — Candidate ItemBaseData is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 2 PASSED — Candidate ItemBaseData assigned.");

            totalChecks++;

            if (equippedItemData == null)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 3 FAILED — Equipped ItemBaseData is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 3 PASSED — Equipped ItemBaseData assigned.");

            //==========================================================
            // Classification
            //==========================================================

            totalChecks++;

            if (candidateItemData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 4 FAILED — Candidate is not Equipment.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 4 PASSED — Candidate is Equipment.");

            totalChecks++;

            if (equippedItemData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 5 FAILED — Equipped item is not Equipment.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 5 PASSED — Equipped item is Equipment.");

            //==========================================================
            // Equipment Type
            //==========================================================

            totalChecks++;

            if (candidateItemData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 6 FAILED — Candidate has EquipmentType.None.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 6 PASSED — Candidate has valid EquipmentType.");

            totalChecks++;

            if (equippedItemData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 7 FAILED — Equipped item has " +
                    "EquipmentType.None.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 7 PASSED — Equipped item has valid EquipmentType.");

            totalChecks++;

            if (candidateItemData.EquipmentType !=
                equippedItemData.EquipmentType)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 8 FAILED — Candidate and equipped items " +
                    "must have the same EquipmentType.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 8 PASSED — Both items use " +
                $"{candidateItemData.EquipmentType}.");

            //==========================================================
            // Configuration
            //==========================================================

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Candidate: {candidateItemData.DisplayName}");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Equipped: {equippedItemData.DisplayName}");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Shared Stat: {sharedStatType}");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Candidate-Only Stat: {candidateOnlyStatType}");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Equipped-Only Stat: {equippedOnlyStatType}");

            //==========================================================
            // Validate Test Data
            //==========================================================

            totalChecks++;

            if (!TryGetBaseStat(
                    candidateItemData,
                    sharedStatType,
                    out EquipmentBaseStat candidateSharedStat))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 9 FAILED — Candidate does not contain " +
                    $"shared stat {sharedStatType}.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"CHECK 9 PASSED — Candidate contains " +
                $"{sharedStatType}: " +
                $"{candidateSharedStat.Value} " +
                $"({candidateSharedStat.ModifierType}).");

            totalChecks++;

            if (!TryGetBaseStat(
                    equippedItemData,
                    sharedStatType,
                    out EquipmentBaseStat equippedSharedStat))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 10 FAILED — Equipped item does not contain " +
                    $"shared stat {sharedStatType}.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"CHECK 10 PASSED — Equipped contains " +
                $"{sharedStatType}: " +
                $"{equippedSharedStat.Value} " +
                $"({equippedSharedStat.ModifierType}).");

            totalChecks++;

            if (!TryGetBaseStat(
                    candidateItemData,
                    candidateOnlyStatType,
                    out EquipmentBaseStat candidateOnlyStat))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 11 FAILED — Candidate does not contain " +
                    $"candidate-only stat {candidateOnlyStatType}.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"CHECK 11 PASSED — Candidate contains exclusive stat " +
                $"{candidateOnlyStatType}: " +
                $"{candidateOnlyStat.Value} " +
                $"({candidateOnlyStat.ModifierType}).");

            totalChecks++;

            if (TryGetBaseStat(
                    equippedItemData,
                    candidateOnlyStatType,
                    out EquipmentBaseStat unexpectedEquippedCandidateStat))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 12 FAILED — Equipped item unexpectedly " +
                    $"contains {candidateOnlyStatType}.");

                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    $"Unexpected value: " +
                    $"{unexpectedEquippedCandidateStat.Value}");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 12 PASSED — Candidate-only stat is absent " +
                "from equipped item.");

            totalChecks++;

            if (!TryGetBaseStat(
                    equippedItemData,
                    equippedOnlyStatType,
                    out EquipmentBaseStat equippedOnlyStat))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 13 FAILED — Equipped item does not contain " +
                    $"equipped-only stat {equippedOnlyStatType}.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"CHECK 13 PASSED — Equipped contains exclusive stat " +
                $"{equippedOnlyStatType}: " +
                $"{equippedOnlyStat.Value} " +
                $"({equippedOnlyStat.ModifierType}).");

            totalChecks++;

            if (TryGetBaseStat(
                    candidateItemData,
                    equippedOnlyStatType,
                    out EquipmentBaseStat unexpectedCandidateEquippedStat))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 14 FAILED — Candidate unexpectedly " +
                    $"contains {equippedOnlyStatType}.");

                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    $"Unexpected value: " +
                    $"{unexpectedCandidateEquippedStat.Value}");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 14 PASSED — Equipped-only stat is absent " +
                "from candidate item.");

            //==========================================================
            // Runtime Dependencies
            //==========================================================

            GameContentContext contentContext =
                GameContentContext.Current;

            totalChecks++;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 15 FAILED — GameContentContext.Current " +
                    "is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 15 PASSED — GameContentContext is available.");

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            totalChecks++;

            if (contentResolver == null)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 16 FAILED — ItemContentResolver is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 16 PASSED — ItemContentResolver is available.");

            //==========================================================
            // Services
            //==========================================================

            EquipmentStatResolver statResolver =
                new EquipmentStatResolver(
                    statDatabase);

            ItemComparisonService comparisonService =
                new ItemComparisonService(
                    contentResolver,
                    statResolver);

            Debug.Log(
                "[Exclusive Stats Test] " +
                "ItemComparisonService created successfully.");

            //==========================================================
            // Item Instances
            //==========================================================

            ItemInstance candidateItem =
                new ItemInstance(
                    "exclusive-stats-test-candidate",
                    candidateItemData.ContentId,
                    candidateItemData.BaseTier);

            ItemInstance equippedItem =
                new ItemInstance(
                    "exclusive-stats-test-equipped",
                    equippedItemData.ContentId,
                    equippedItemData.BaseTier);

            //==========================================================
            // Comparison
            //==========================================================

            Debug.Log(
                "[Exclusive Stats Test] " +
                "Executing ItemComparisonService.TryCompare...");

            bool comparisonSucceeded =
                comparisonService.TryCompare(
                    candidateItem,
                    equippedItem,
                    out ItemComparisonResult result);

            totalChecks++;

            if (!comparisonSucceeded)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 17 FAILED — Comparison failed.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 17 PASSED — Comparison succeeded.");

            totalChecks++;

            if (result == null)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 18 FAILED — Result is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 18 PASSED — ItemComparisonResult created.");

            //==========================================================
            // Shared Stat
            //==========================================================

            if (!TryFindComparisonStat(
                    result,
                    sharedStatType,
                    out EquipmentComparisonStat sharedComparison))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    $"Shared stat {sharedStatType} was not found " +
                    "in comparison result.");

                return;
            }

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"SHARED STAT [{sharedStatType}]");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Equipped = {sharedComparison.EquippedValue} " +
                $"({sharedComparison.EquippedModifierType})");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Candidate = {sharedComparison.CandidateValue} " +
                $"({sharedComparison.CandidateModifierType})");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Difference = {sharedComparison.Difference}");

            totalChecks++;

            if (!sharedComparison.HasEquippedValue ||
                !sharedComparison.HasCandidateValue)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 19 FAILED — Shared stat does not exist " +
                    "on both items.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 19 PASSED — Shared stat exists on both items.");

            totalChecks++;

            totalChecks++;

            if (!sharedComparison.IsDirectlyComparable)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 20 FAILED — Shared stat should be directly comparable.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 20 PASSED — Shared stat is directly comparable.");

            if (!Mathf.Approximately(
                    sharedComparison.EquippedValue,
                    equippedSharedStat.Value))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 21 FAILED — Equipped shared value mismatch.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 21 PASSED — Equipped shared value is correct.");

            totalChecks++;

            if (!Mathf.Approximately(
                    sharedComparison.CandidateValue,
                    candidateSharedStat.Value))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 22 FAILED — Candidate shared value mismatch.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 22 PASSED — Candidate shared value is correct.");

            float expectedSharedDifference =
                candidateSharedStat.Value -
                equippedSharedStat.Value;

            totalChecks++;

            if (!Mathf.Approximately(
                    sharedComparison.Difference,
                    expectedSharedDifference))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 23 FAILED — Shared stat difference mismatch.");

                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    $"Expected = {expectedSharedDifference}, " +
                    $"Actual = {sharedComparison.Difference}");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"CHECK 23 PASSED — Difference is correct: " +
                $"{sharedComparison.Difference}");

            //==========================================================
            // Candidate-Only Stat
            //==========================================================

            if (!TryFindComparisonStat(
                    result,
                    candidateOnlyStatType,
                    out EquipmentComparisonStat candidateOnlyComparison))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    $"Candidate-only stat {candidateOnlyStatType} " +
                    "was not found in comparison result.");

                return;
            }

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"CANDIDATE-ONLY STAT [{candidateOnlyStatType}]");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Equipped HasValue = " +
                $"{candidateOnlyComparison.HasEquippedValue}");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Candidate HasValue = " +
                $"{candidateOnlyComparison.HasCandidateValue}");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Candidate Value = " +
                $"{candidateOnlyComparison.CandidateValue}");

            totalChecks++;

            if (candidateOnlyComparison.HasEquippedValue ||
                !candidateOnlyComparison.HasCandidateValue)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 23 FAILED — Candidate-only stat flags " +
                    "are incorrect.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 23 PASSED — Candidate-only flags are correct.");

            totalChecks++;

            if (!Mathf.Approximately(
                    candidateOnlyComparison.CandidateValue,
                    candidateOnlyStat.Value))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 24 FAILED — Candidate-only value mismatch.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 24 PASSED — Candidate-only value is correct.");

            totalChecks++;

            if (candidateOnlyComparison.HasEquippedValue)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 25 FAILED — Candidate-only stat " +
                    "should not have an equipped value.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 25 PASSED — Candidate-only stat has no " +
                "equipped value.");

            //==========================================================
            // Equipped-Only Stat
            //==========================================================

            if (!TryFindComparisonStat(
                    result,
                    equippedOnlyStatType,
                    out EquipmentComparisonStat equippedOnlyComparison))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    $"Equipped-only stat {equippedOnlyStatType} " +
                    "was not found in comparison result.");

                return;
            }

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"EQUIPPED-ONLY STAT [{equippedOnlyStatType}]");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Equipped HasValue = " +
                $"{equippedOnlyComparison.HasEquippedValue}");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Candidate HasValue = " +
                $"{equippedOnlyComparison.HasCandidateValue}");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"Equipped Value = " +
                $"{equippedOnlyComparison.EquippedValue}");

            totalChecks++;

            if (!equippedOnlyComparison.HasEquippedValue ||
                equippedOnlyComparison.HasCandidateValue)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 26 FAILED — Equipped-only stat flags " +
                    "are incorrect.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 26 PASSED — Equipped-only flags are correct.");

            totalChecks++;

            if (!Mathf.Approximately(
                    equippedOnlyComparison.EquippedValue,
                    equippedOnlyStat.Value))
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 27 FAILED — Equipped-only value mismatch.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 27 PASSED — Equipped-only value is correct.");

            totalChecks++;

            if (equippedOnlyComparison.HasCandidateValue)
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "CHECK 28 FAILED — Equipped-only stat " +
                    "should not have a candidate value.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Exclusive Stats Test] " +
                "CHECK 28 PASSED — Equipped-only stat has no " +
                "candidate value.");

            //==========================================================
            // Final Summary
            //==========================================================

            Debug.Log(
                "[Exclusive Stats Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Exclusive Stats Test] " +
                "EXPECTED STAT MODEL:");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"{sharedStatType} → BOTH ITEMS");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"{candidateOnlyStatType} → CANDIDATE ONLY");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"{equippedOnlyStatType} → EQUIPPED ONLY");

            Debug.Log(
                "[Exclusive Stats Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Exclusive Stats Test] " +
                "ACTUAL COMPARISON RESULT:");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"{sharedStatType}: " +
                $"Equipped={sharedComparison.EquippedValue}, " +
                $"Candidate={sharedComparison.CandidateValue}, " +
                $"Difference={sharedComparison.Difference}");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"{candidateOnlyStatType}: " +
                $"Equipped=NONE, " +
                $"Candidate={candidateOnlyComparison.CandidateValue}");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"{equippedOnlyStatType}: " +
                $"Equipped={equippedOnlyComparison.EquippedValue}, " +
                $"Candidate=NONE");

            Debug.Log(
                "[Exclusive Stats Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Exclusive Stats Test] " +
                $"VALIDATION RESULT: " +
                $"{passedChecks}/{totalChecks} checks passed.");

            if (passedChecks == totalChecks)
            {
                Debug.Log(
                    "[Exclusive Stats Test] " +
                    "RULE VERIFIED: Shared stats, candidate-only " +
                    "stats and equipped-only stats are represented " +
                    "correctly.");

                Debug.Log(
                    "[Exclusive Stats Test] " +
                    "✓ TEST PASSED — Exclusive stat comparison " +
                    "behavior is working correctly.");
            }
            else
            {
                Debug.LogError(
                    "[Exclusive Stats Test] " +
                    "TEST FAILED — One or more validation checks failed.");
            }

            Debug.Log(
                "[Exclusive Stats Test] " +
                "==================================================");
        }

        private bool TryGetBaseStat(
            ItemBaseData itemData,
            StatType statType,
            out EquipmentBaseStat result)
        {
            result = default;

            if (itemData == null)
                return false;

            if (itemData.BaseStats == null)
                return false;

            for (int i = 0;
                 i < itemData.BaseStats.Count;
                 i++)
            {
                EquipmentBaseStat stat =
                    itemData.BaseStats[i];

                if (stat.StatType != statType)
                    continue;

                result = stat;

                return true;
            }

            return false;
        }

        private bool TryFindComparisonStat(
            ItemComparisonResult result,
            StatType statType,
            out EquipmentComparisonStat comparisonStat)
        {
            comparisonStat = default;

            if (result == null)
                return false;

            for (int i = 0;
                 i < result.Stats.Count;
                 i++)
            {
                EquipmentComparisonStat stat =
                    result.Stats[i];

                if (stat.StatType != statType)
                    continue;

                comparisonStat = stat;

                return true;
            }

            return false;
        }
    }
}