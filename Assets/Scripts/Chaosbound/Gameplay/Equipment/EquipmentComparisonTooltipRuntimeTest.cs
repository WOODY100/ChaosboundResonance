using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentComparisonTooltipRuntimeTest :
        MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField]
        private EquipmentStatDatabase statDatabase;

        [Header("Scenario A — Normal Comparison")]
        [SerializeField]
        private ItemBaseData normalCandidateItemData;

        [SerializeField]
        private ItemBaseData normalEquippedItemData;

        [Header("Scenario B — Different ModifierType")]
        [SerializeField]
        private ItemBaseData modifierCandidateItemData;

        [SerializeField]
        private ItemBaseData modifierEquippedItemData;

        [Header("Expected Stats")]
        [SerializeField]
        private StatType sharedStatType =
            StatType.Damage;

        [SerializeField]
        private StatType candidateOnlyStatType =
            StatType.AttackSpeed;

        [SerializeField]
        private StatType equippedOnlyStatType =
            StatType.Luck;

        [SerializeField]
        private StatType modifierTestStatType =
            StatType.Damage;

        [ContextMenu("Run Equipment Comparison Tooltip Test")]
        private void RunTest()
        {
            int passedChecks = 0;
            int totalChecks = 0;

            Debug.Log(
                "[Tooltip Test] " +
                "==================================================");

            Debug.Log(
                "[Tooltip Test] " +
                "START — Equipment comparison tooltip validation");

            Debug.Log(
                "[Tooltip Test] " +
                "==================================================");

            //==========================================================
            // Configuration
            //==========================================================

            totalChecks++;

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 1 FAILED — EquipmentStatDatabase is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 1 PASSED — EquipmentStatDatabase assigned.");

            totalChecks++;

            if (normalCandidateItemData == null)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 2 FAILED — Normal candidate is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 2 PASSED — Normal candidate assigned.");

            totalChecks++;

            if (normalEquippedItemData == null)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 3 FAILED — Normal equipped item is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 3 PASSED — Normal equipped item assigned.");

            totalChecks++;

            if (modifierCandidateItemData == null)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 4 FAILED — Modifier candidate is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 4 PASSED — Modifier candidate assigned.");

            totalChecks++;

            if (modifierEquippedItemData == null)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 5 FAILED — Modifier equipped item is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 5 PASSED — Modifier equipped item assigned.");

            //==========================================================
            // Runtime Dependencies
            //==========================================================

            GameContentContext contentContext =
                GameContentContext.Current;

            totalChecks++;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 6 FAILED — GameContentContext is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 6 PASSED — GameContentContext is available.");

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            totalChecks++;

            if (contentResolver == null)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 7 FAILED — ItemContentResolver is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 7 PASSED — ItemContentResolver is available.");

            EquipmentStatResolver statResolver =
                new EquipmentStatResolver(
                    statDatabase);

            ItemComparisonService comparisonService =
                new ItemComparisonService(
                    contentResolver,
                    statResolver);

            EquipmentComparisonDisplayBuilder displayBuilder =
                new EquipmentComparisonDisplayBuilder();

            EquipmentComparisonTooltipBuilder tooltipBuilder =
                new EquipmentComparisonTooltipBuilder();

            Debug.Log(
                "[Tooltip Test] " +
                "Comparison, display and tooltip builders " +
                "created successfully.");

            //==========================================================
            // SCENARIO A
            //==========================================================

            Debug.Log(
                "[Tooltip Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Tooltip Test] " +
                "SCENARIO A — Normal comparison");

            ItemInstance candidateItem =
                new ItemInstance(
                    "tooltip-test-candidate",
                    normalCandidateItemData.ContentId,
                    normalCandidateItemData.BaseTier);

            ItemInstance equippedItem =
                new ItemInstance(
                    "tooltip-test-equipped",
                    normalEquippedItemData.ContentId,
                    normalEquippedItemData.BaseTier);

            bool comparisonSucceeded =
                comparisonService.TryCompare(
                    candidateItem,
                    equippedItem,
                    out ItemComparisonResult comparisonResult);

            totalChecks++;

            if (!comparisonSucceeded)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 8 FAILED — Scenario A comparison failed.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 8 PASSED — Scenario A comparison succeeded.");

            totalChecks++;

            if (comparisonResult == null)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 9 FAILED — ComparisonResult is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 9 PASSED — ItemComparisonResult created.");

            EquipmentComparisonDisplayData displayData =
                displayBuilder.Build(
                    comparisonResult);

            totalChecks++;

            if (displayData == null)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 10 FAILED — DisplayData is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 10 PASSED — DisplayData created.");

            EquipmentComparisonTooltipData tooltipData =
                tooltipBuilder.Build(
                    displayData,
                    comparisonResult);

            totalChecks++;

            if (tooltipData == null)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 11 FAILED — TooltipData is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 11 PASSED — TooltipData created.");

            //==========================================================
            // Object References
            //==========================================================

            totalChecks++;

            if (tooltipData.CandidateItem != candidateItem)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 12 FAILED — Candidate ItemInstance " +
                    "was not preserved.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 12 PASSED — Candidate ItemInstance preserved.");

            totalChecks++;

            if (tooltipData.EquippedItem != equippedItem)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 13 FAILED — Equipped ItemInstance " +
                    "was not preserved.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 13 PASSED — Equipped ItemInstance preserved.");

            totalChecks++;

            if (tooltipData.CandidateBaseData !=
                normalCandidateItemData)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 14 FAILED — Candidate BaseData " +
                    "was not preserved.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 14 PASSED — Candidate BaseData preserved.");

            totalChecks++;

            if (tooltipData.EquippedBaseData !=
                normalEquippedItemData)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 15 FAILED — Equipped BaseData " +
                    "was not preserved.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 15 PASSED — Equipped BaseData preserved.");

            totalChecks++;

            if (tooltipData.EquipmentType !=
                normalCandidateItemData.EquipmentType)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 16 FAILED — EquipmentType was not preserved.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 16 PASSED — EquipmentType preserved.");

            //==========================================================
            // Row Count
            //==========================================================

            Debug.Log(
                "[Tooltip Test] " +
                $"Scenario A Tooltip Rows = " +
                $"{tooltipData.RowCount}");

            totalChecks++;

            if (tooltipData.RowCount !=
                displayData.Count)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 17 FAILED — Tooltip row count does not " +
                    "match DisplayData.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 17 PASSED — All display entries became " +
                "tooltip rows.");

            //==========================================================
            // Shared Stat
            //==========================================================

            totalChecks++;

            if (!TryFindRow(
                    tooltipData,
                    sharedStatType,
                    out EquipmentComparisonTooltipRowData sharedRow))
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    $"CHECK 18 FAILED — Shared stat " +
                    $"{sharedStatType} was not found.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                $"CHECK 18 PASSED — Shared stat " +
                $"{sharedStatType} found.");

            Debug.Log(
                "[Tooltip Test] " +
                $"Shared State = {sharedRow.State}");

            Debug.Log(
                "[Tooltip Test] " +
                $"Shared Equipped = " +
                $"{sharedRow.EquippedValue} " +
                $"({sharedRow.EquippedModifierType})");

            Debug.Log(
                "[Tooltip Test] " +
                $"Shared Candidate = " +
                $"{sharedRow.CandidateValue} " +
                $"({sharedRow.CandidateModifierType})");

            Debug.Log(
                "[Tooltip Test] " +
                $"Shared Difference = " +
                $"{sharedRow.Difference}");

            totalChecks++;

            if (!sharedRow.HasEquippedValue ||
                !sharedRow.HasCandidateValue)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 19 FAILED — Shared row values are invalid.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 19 PASSED — Shared values are present.");

            totalChecks++;

            if (!sharedRow.HasDifference)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 20 FAILED — Shared row should expose " +
                    "a difference.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 20 PASSED — Shared row exposes a difference.");

            totalChecks++;

            if (sharedRow.State !=
                EquipmentComparisonDisplayState.Reduced)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 21 FAILED — Shared row should be Reduced.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 21 PASSED — Shared row state is Reduced.");

            totalChecks++;

            if (!Mathf.Approximately(
                    sharedRow.Difference,
                    -5f))
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 22 FAILED — Expected difference = -5.");

                Debug.LogError(
                    "[Tooltip Test] " +
                    $"Actual difference = " +
                    $"{sharedRow.Difference}");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 22 PASSED — Shared difference = -5.");

            //==========================================================
            // Candidate Only
            //==========================================================

            totalChecks++;

            if (!TryFindRow(
                    tooltipData,
                    candidateOnlyStatType,
                    out EquipmentComparisonTooltipRowData candidateOnlyRow))
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    $"CHECK 23 FAILED — Candidate-only stat " +
                    $"{candidateOnlyStatType} was not found.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                $"CHECK 23 PASSED — Candidate-only stat " +
                $"{candidateOnlyStatType} found.");

            totalChecks++;

            if (candidateOnlyRow.HasEquippedValue ||
                !candidateOnlyRow.HasCandidateValue)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 24 FAILED — Candidate-only flags are invalid.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 24 PASSED — Candidate-only flags are correct.");

            totalChecks++;

            if (candidateOnlyRow.HasDifference ||
                candidateOnlyRow.IsDirectlyComparable)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 25 FAILED — Candidate-only row must " +
                    "not have a comparison difference.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 25 PASSED — Candidate-only row has no difference.");

            totalChecks++;

            if (candidateOnlyRow.State !=
                EquipmentComparisonDisplayState.CandidateOnly)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 26 FAILED — Candidate-only state is invalid.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 26 PASSED — Candidate-only state preserved.");

            //==========================================================
            // Equipped Only
            //==========================================================

            totalChecks++;

            if (!TryFindRow(
                    tooltipData,
                    equippedOnlyStatType,
                    out EquipmentComparisonTooltipRowData equippedOnlyRow))
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    $"CHECK 27 FAILED — Equipped-only stat " +
                    $"{equippedOnlyStatType} was not found.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                $"CHECK 27 PASSED — Equipped-only stat " +
                $"{equippedOnlyStatType} found.");

            totalChecks++;

            if (!equippedOnlyRow.HasEquippedValue ||
                equippedOnlyRow.HasCandidateValue)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 28 FAILED — Equipped-only flags are invalid.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 28 PASSED — Equipped-only flags are correct.");

            totalChecks++;

            if (equippedOnlyRow.HasDifference ||
                equippedOnlyRow.IsDirectlyComparable)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 29 FAILED — Equipped-only row must " +
                    "not have a comparison difference.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 29 PASSED — Equipped-only row has no difference.");

            totalChecks++;

            if (equippedOnlyRow.State !=
                EquipmentComparisonDisplayState.EquippedOnly)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 30 FAILED — Equipped-only state is invalid.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 30 PASSED — Equipped-only state preserved.");

            //==========================================================
            // SCENARIO B — Different ModifierType
            //==========================================================

            Debug.Log(
                "[Tooltip Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Tooltip Test] " +
                "SCENARIO B — Different ModifierType");

            ItemInstance modifierCandidateItem =
                new ItemInstance(
                    "tooltip-test-modifier-candidate",
                    modifierCandidateItemData.ContentId,
                    modifierCandidateItemData.BaseTier);

            ItemInstance modifierEquippedItem =
                new ItemInstance(
                    "tooltip-test-modifier-equipped",
                    modifierEquippedItemData.ContentId,
                    modifierEquippedItemData.BaseTier);

            bool modifierComparisonSucceeded =
                comparisonService.TryCompare(
                    modifierCandidateItem,
                    modifierEquippedItem,
                    out ItemComparisonResult modifierResult);

            totalChecks++;

            if (!modifierComparisonSucceeded)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 31 FAILED — Scenario B comparison failed.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 31 PASSED — Scenario B comparison succeeded.");

            EquipmentComparisonDisplayData modifierDisplayData =
                displayBuilder.Build(
                    modifierResult);

            totalChecks++;

            if (modifierDisplayData == null)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 32 FAILED — Scenario B DisplayData is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 32 PASSED — Scenario B DisplayData created.");

            EquipmentComparisonTooltipData modifierTooltipData =
                tooltipBuilder.Build(
                    modifierDisplayData,
                    modifierResult);

            totalChecks++;

            if (modifierTooltipData == null)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 33 FAILED — Scenario B TooltipData is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 33 PASSED — Scenario B TooltipData created.");

            totalChecks++;

            if (!TryFindRow(
                    modifierTooltipData,
                    modifierTestStatType,
                    out EquipmentComparisonTooltipRowData modifierRow))
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    $"CHECK 34 FAILED — Modifier test stat " +
                    $"{modifierTestStatType} was not found.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                $"CHECK 34 PASSED — Modifier test stat " +
                $"{modifierTestStatType} found.");

            Debug.Log(
                "[Tooltip Test] " +
                $"Modifier Equipped = " +
                $"{modifierRow.EquippedValue} " +
                $"({modifierRow.EquippedModifierType})");

            Debug.Log(
                "[Tooltip Test] " +
                $"Modifier Candidate = " +
                $"{modifierRow.CandidateValue} " +
                $"({modifierRow.CandidateModifierType})");

            Debug.Log(
                "[Tooltip Test] " +
                $"Modifier State = " +
                $"{modifierRow.State}");

            totalChecks++;

            if (modifierRow.State !=
                EquipmentComparisonDisplayState
                    .DifferentModifierType)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 35 FAILED — Expected " +
                    "DifferentModifierType state.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 35 PASSED — DifferentModifierType state preserved.");

            totalChecks++;

            if (modifierRow.HasDifference ||
                modifierRow.IsDirectlyComparable)
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "CHECK 36 FAILED — Different ModifierTypes " +
                    "must not expose a comparison difference.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Tooltip Test] " +
                "CHECK 36 PASSED — Different ModifierTypes " +
                "have no comparison difference.");

            //==========================================================
            // Final Summary
            //==========================================================

            Debug.Log(
                "[Tooltip Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Tooltip Test] EXPECTED TOOLTIP MODEL:");

            Debug.Log(
                "[Tooltip Test] " +
                "Damage → Reduced → Difference = -5");

            Debug.Log(
                "[Tooltip Test] " +
                "AttackSpeed → CandidateOnly → No Difference");

            Debug.Log(
                "[Tooltip Test] " +
                "Luck → EquippedOnly → No Difference");

            Debug.Log(
                "[Tooltip Test] " +
                "Different ModifierType → No Difference");

            Debug.Log(
                "[Tooltip Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Tooltip Test] ACTUAL TOOLTIP MODEL:");

            Debug.Log(
                "[Tooltip Test] " +
                $"Damage → {sharedRow.State} → " +
                $"Difference={sharedRow.Difference}");

            Debug.Log(
                "[Tooltip Test] " +
                $"AttackSpeed → {candidateOnlyRow.State} → " +
                $"HasDifference={candidateOnlyRow.HasDifference}");

            Debug.Log(
                "[Tooltip Test] " +
                $"Luck → {equippedOnlyRow.State} → " +
                $"HasDifference={equippedOnlyRow.HasDifference}");

            Debug.Log(
                "[Tooltip Test] " +
                $"Modifier Damage → {modifierRow.State} → " +
                $"HasDifference={modifierRow.HasDifference}");

            Debug.Log(
                "[Tooltip Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Tooltip Test] " +
                $"VALIDATION RESULT: " +
                $"{passedChecks}/{totalChecks} checks passed.");

            if (passedChecks == totalChecks)
            {
                Debug.Log(
                    "[Tooltip Test] " +
                    "RULE VERIFIED: All comparison display entries " +
                    "are represented as tooltip rows.");

                Debug.Log(
                    "[Tooltip Test] " +
                    "RULE VERIFIED: Comparable stats preserve " +
                    "their difference and state.");

                Debug.Log(
                    "[Tooltip Test] " +
                    "RULE VERIFIED: Exclusive stats remain visible " +
                    "without comparison differences.");

                Debug.Log(
                    "[Tooltip Test] " +
                    "RULE VERIFIED: Different ModifierTypes remain " +
                    "visible without numeric comparison.");

                Debug.Log(
                    "[Tooltip Test] " +
                    "RULE VERIFIED: Candidate and equipped item " +
                    "references are preserved.");

                Debug.Log(
                    "[Tooltip Test] " +
                    "✓ TEST PASSED — Equipment comparison tooltip " +
                    "data is working correctly.");
            }
            else
            {
                Debug.LogError(
                    "[Tooltip Test] " +
                    "TEST FAILED — One or more validation checks failed.");
            }

            Debug.Log(
                "[Tooltip Test] " +
                "==================================================");
        }

        private bool TryFindRow(
            EquipmentComparisonTooltipData tooltipData,
            StatType statType,
            out EquipmentComparisonTooltipRowData row)
        {
            row = default;

            if (tooltipData == null)
                return false;

            for (int i = 0;
                 i < tooltipData.Rows.Count;
                 i++)
            {
                EquipmentComparisonTooltipRowData current =
                    tooltipData.Rows[i];

                if (current.StatType != statType)
                    continue;

                row = current;

                return true;
            }

            return false;
        }
    }
}