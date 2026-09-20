using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentComparisonModifierTypeRuntimeTest :
        MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField]
        private EquipmentStatDatabase statDatabase;

        [SerializeField]
        private ItemBaseData candidateItemData;

        [SerializeField]
        private ItemBaseData equippedItemData;

        [Header("Expected Configuration")]
        [SerializeField]
        private StatType testedStatType =
            StatType.Damage;

        [SerializeField]
        private ModifierType expectedEquippedModifierType =
            ModifierType.Flat;

        [SerializeField]
        private ModifierType expectedCandidateModifierType =
            ModifierType.Percent;

        [SerializeField]
        private float expectedEquippedValue =
            10f;

        [SerializeField]
        private float expectedCandidateValue =
            10f;

        [ContextMenu("Run Modifier Type Comparison Test")]
        private void RunTest()
        {
            int passedChecks = 0;
            int totalChecks = 0;

            Debug.Log(
                "[Modifier Type Test] " +
                "==================================================");

            Debug.Log(
                "[Modifier Type Test] " +
                "START — ModifierType compatibility validation");

            Debug.Log(
                "[Modifier Type Test] " +
                "==================================================");

            //==========================================================
            // References
            //==========================================================

            totalChecks++;

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 1 FAILED — EquipmentStatDatabase is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 1 PASSED — EquipmentStatDatabase assigned.");

            totalChecks++;

            if (candidateItemData == null)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 2 FAILED — Candidate ItemBaseData is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 2 PASSED — Candidate ItemBaseData assigned.");

            totalChecks++;

            if (equippedItemData == null)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 3 FAILED — Equipped ItemBaseData is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 3 PASSED — Equipped ItemBaseData assigned.");

            //==========================================================
            // Classification
            //==========================================================

            totalChecks++;

            if (candidateItemData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 4 FAILED — Candidate is not Equipment.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 4 PASSED — Candidate is Equipment.");

            totalChecks++;

            if (equippedItemData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 5 FAILED — Equipped item is not Equipment.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 5 PASSED — Equipped item is Equipment.");

            //==========================================================
            // Equipment Type
            //==========================================================

            totalChecks++;

            if (candidateItemData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 6 FAILED — Candidate has EquipmentType.None.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 6 PASSED — Candidate has valid EquipmentType.");

            totalChecks++;

            if (equippedItemData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 7 FAILED — Equipped item has " +
                    "EquipmentType.None.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 7 PASSED — Equipped item has valid EquipmentType.");

            totalChecks++;

            if (candidateItemData.EquipmentType !=
                equippedItemData.EquipmentType)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 8 FAILED — Candidate and equipped items " +
                    "must have the same EquipmentType.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 8 PASSED — Both items use " +
                $"{candidateItemData.EquipmentType}.");

            //==========================================================
            // Test Configuration
            //==========================================================

            Debug.Log(
                "[Modifier Type Test] " +
                $"Candidate: {candidateItemData.DisplayName}");

            Debug.Log(
                "[Modifier Type Test] " +
                $"Equipped: {equippedItemData.DisplayName}");

            Debug.Log(
                "[Modifier Type Test] " +
                $"Tested Stat: {testedStatType}");

            //==========================================================
            // Read Base Stats
            //==========================================================

            totalChecks++;

            if (!TryGetBaseStat(
                    equippedItemData,
                    testedStatType,
                    out EquipmentBaseStat equippedBaseStat))
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    $"CHECK 9 FAILED — Equipped item does not contain " +
                    $"{testedStatType}.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                $"CHECK 9 PASSED — Equipped contains " +
                $"{testedStatType}: " +
                $"{equippedBaseStat.Value} " +
                $"({equippedBaseStat.ModifierType}).");

            totalChecks++;

            if (!TryGetBaseStat(
                    candidateItemData,
                    testedStatType,
                    out EquipmentBaseStat candidateBaseStat))
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    $"CHECK 10 FAILED — Candidate does not contain " +
                    $"{testedStatType}.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                $"CHECK 10 PASSED — Candidate contains " +
                $"{testedStatType}: " +
                $"{candidateBaseStat.Value} " +
                $"({candidateBaseStat.ModifierType}).");

            //==========================================================
            // Modifier Type Validation
            //==========================================================

            totalChecks++;

            if (equippedBaseStat.ModifierType !=
                expectedEquippedModifierType)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 11 FAILED — Equipped ModifierType mismatch.");

                Debug.LogError(
                    "[Modifier Type Test] " +
                    $"Expected = {expectedEquippedModifierType}, " +
                    $"Actual = {equippedBaseStat.ModifierType}");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 11 PASSED — Equipped ModifierType is " +
                $"{equippedBaseStat.ModifierType}.");

            totalChecks++;

            if (candidateBaseStat.ModifierType !=
                expectedCandidateModifierType)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 12 FAILED — Candidate ModifierType mismatch.");

                Debug.LogError(
                    "[Modifier Type Test] " +
                    $"Expected = {expectedCandidateModifierType}, " +
                    $"Actual = {candidateBaseStat.ModifierType}");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 12 PASSED — Candidate ModifierType is " +
                $"{candidateBaseStat.ModifierType}.");

            totalChecks++;

            if (equippedBaseStat.ModifierType ==
                candidateBaseStat.ModifierType)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 13 FAILED — Test configuration does not " +
                    "contain different ModifierTypes.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 13 PASSED — ModifierTypes are intentionally different.");

            //==========================================================
            // Value Validation
            //==========================================================

            totalChecks++;

            if (!Mathf.Approximately(
                    equippedBaseStat.Value,
                    expectedEquippedValue))
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 14 FAILED — Equipped value mismatch.");

                Debug.LogError(
                    "[Modifier Type Test] " +
                    $"Expected = {expectedEquippedValue}, " +
                    $"Actual = {equippedBaseStat.Value}");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                $"CHECK 14 PASSED — Equipped value = " +
                $"{equippedBaseStat.Value}.");

            totalChecks++;

            if (!Mathf.Approximately(
                    candidateBaseStat.Value,
                    expectedCandidateValue))
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 15 FAILED — Candidate value mismatch.");

                Debug.LogError(
                    "[Modifier Type Test] " +
                    $"Expected = {expectedCandidateValue}, " +
                    $"Actual = {candidateBaseStat.Value}");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                $"CHECK 15 PASSED — Candidate value = " +
                $"{candidateBaseStat.Value}.");

            //==========================================================
            // Runtime Dependencies
            //==========================================================

            GameContentContext contentContext =
                GameContentContext.Current;

            totalChecks++;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 16 FAILED — GameContentContext.Current " +
                    "is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 16 PASSED — GameContentContext is available.");

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            totalChecks++;

            if (contentResolver == null)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 17 FAILED — ItemContentResolver is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 17 PASSED — ItemContentResolver is available.");

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
                "[Modifier Type Test] " +
                "ItemComparisonService created successfully.");

            //==========================================================
            // Item Instances
            //==========================================================

            ItemInstance candidateItem =
                new ItemInstance(
                    "modifier-type-test-candidate",
                    candidateItemData.ContentId,
                    candidateItemData.BaseTier);

            ItemInstance equippedItem =
                new ItemInstance(
                    "modifier-type-test-equipped",
                    equippedItemData.ContentId,
                    equippedItemData.BaseTier);

            //==========================================================
            // Comparison
            //==========================================================

            Debug.Log(
                "[Modifier Type Test] " +
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
                    "[Modifier Type Test] " +
                    "CHECK 18 FAILED — Comparison failed.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 18 PASSED — Comparison succeeded.");

            totalChecks++;

            if (result == null)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 19 FAILED — ItemComparisonResult is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 19 PASSED — ItemComparisonResult created.");

            //==========================================================
            // Find Comparison Stat
            //==========================================================

            totalChecks++;

            if (!TryFindComparisonStat(
                    result,
                    testedStatType,
                    out EquipmentComparisonStat comparisonStat))
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    $"CHECK 20 FAILED — {testedStatType} was not " +
                    "found in comparison result.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                $"CHECK 20 PASSED — {testedStatType} found " +
                "in comparison result.");

            //==========================================================
            // Presence
            //==========================================================

            totalChecks++;

            if (!comparisonStat.HasEquippedValue ||
                !comparisonStat.HasCandidateValue)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 21 FAILED — Both items should contain " +
                    $"{testedStatType}.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 21 PASSED — Both items have the tested stat.");

            //==========================================================
            // Modifier Types
            //==========================================================

            Debug.Log(
                "[Modifier Type Test] " +
                $"Equipped ModifierType = " +
                $"{comparisonStat.EquippedModifierType}");

            Debug.Log(
                "[Modifier Type Test] " +
                $"Candidate ModifierType = " +
                $"{comparisonStat.CandidateModifierType}");

            totalChecks++;

            if (comparisonStat.EquippedModifierType !=
                expectedEquippedModifierType)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 22 FAILED — Result equipped ModifierType " +
                    "is incorrect.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 22 PASSED — Result equipped ModifierType is correct.");

            totalChecks++;

            if (comparisonStat.CandidateModifierType !=
                expectedCandidateModifierType)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 23 FAILED — Result candidate ModifierType " +
                    "is incorrect.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 23 PASSED — Result candidate ModifierType is correct.");

            //==========================================================
            // HasSameModifierType
            //==========================================================

            Debug.Log(
                "[Modifier Type Test] " +
                $"HasSameModifierType = " +
                $"{comparisonStat.HasSameModifierType}");

            totalChecks++;

            if (comparisonStat.HasSameModifierType)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 24 FAILED — HasSameModifierType should " +
                    "be FALSE.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 24 PASSED — HasSameModifierType is FALSE.");

            totalChecks++;

            if (comparisonStat.IsDirectlyComparable)
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 25 FAILED — Stats with different ModifierTypes " +
                    "must not be directly comparable.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 25 PASSED — Different ModifierTypes are not " +
                "directly comparable.");

            //==========================================================
            // Numeric Values
            //==========================================================

            totalChecks++;

            if (!Mathf.Approximately(
                    comparisonStat.EquippedValue,
                    expectedEquippedValue))
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 26 FAILED — Equipped comparison value mismatch.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 26 PASSED — Equipped comparison value is correct.");

            totalChecks++;

            if (!Mathf.Approximately(
                    comparisonStat.CandidateValue,
                    expectedCandidateValue))
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 27 FAILED — Candidate comparison value mismatch.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                "CHECK 27 PASSED — Candidate comparison value is correct.");

            //==========================================================
            // Difference
            //==========================================================

            float expectedDifference =
                expectedCandidateValue -
                expectedEquippedValue;

            Debug.Log(
                "[Modifier Type Test] " +
                $"Expected Difference = {expectedDifference}");

            Debug.Log(
                "[Modifier Type Test] " +
                $"Actual Difference = {comparisonStat.Difference}");

            totalChecks++;

            if (!Mathf.Approximately(
                    comparisonStat.Difference,
                    expectedDifference))
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "CHECK 28 FAILED — Difference mismatch.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Modifier Type Test] " +
                $"CHECK 28 PASSED — Difference = " +
                $"{comparisonStat.Difference}.");

            //==========================================================
            // Final Summary
            //==========================================================

            Debug.Log(
                "[Modifier Type Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Modifier Type Test] EXPECTED:");

            Debug.Log(
                "[Modifier Type Test] " +
                $"Equipped = {expectedEquippedValue} " +
                $"({expectedEquippedModifierType})");

            Debug.Log(
                "[Modifier Type Test] " +
                $"Candidate = {expectedCandidateValue} " +
                $"({expectedCandidateModifierType})");

            Debug.Log(
                "[Modifier Type Test] " +
                "HasSameModifierType = FALSE");

            Debug.Log(
                "[Modifier Type Test] " +
                $"Difference = {expectedDifference}");

            Debug.Log(
                "[Modifier Type Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Modifier Type Test] ACTUAL:");

            Debug.Log(
                "[Modifier Type Test] " +
                $"Equipped = {comparisonStat.EquippedValue} " +
                $"({comparisonStat.EquippedModifierType})");

            Debug.Log(
                "[Modifier Type Test] " +
                $"Candidate = {comparisonStat.CandidateValue} " +
                $"({comparisonStat.CandidateModifierType})");

            Debug.Log(
                "[Modifier Type Test] " +
                $"HasSameModifierType = " +
                $"{comparisonStat.HasSameModifierType}");

            Debug.Log(
                "[Modifier Type Test] " +
                $"Difference = {comparisonStat.Difference}");

            Debug.Log(
                "[Modifier Type Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Modifier Type Test] " +
                $"VALIDATION RESULT: " +
                $"{passedChecks}/{totalChecks} checks passed.");

            if (passedChecks == totalChecks)
            {
                Debug.Log(
                    "[Modifier Type Test] " +
                    "RULE VERIFIED: ModifierType is preserved " +
                    "independently for equipped and candidate stats.");

                Debug.Log(
                    "[Modifier Type Test] " +
                    "RULE VERIFIED: Different ModifierTypes are " +
                    "correctly detected through HasSameModifierType.");

                Debug.Log(
                    "[Modifier Type Test] " +
                    "✓ TEST PASSED — ModifierType comparison behavior " +
                    "is working correctly.");
            }
            else
            {
                Debug.LogError(
                    "[Modifier Type Test] " +
                    "TEST FAILED — One or more validation checks failed.");
            }

            Debug.Log(
                "[Modifier Type Test] " +
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