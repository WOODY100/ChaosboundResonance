using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentComparisonDisplayRuntimeTest :
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

        [Header("Scenario A — Expected Stats")]
        [SerializeField]
        private StatType sharedStatType =
            StatType.Damage;

        [SerializeField]
        private StatType candidateOnlyStatType =
            StatType.AttackSpeed;

        [SerializeField]
        private StatType equippedOnlyStatType =
            StatType.Luck;

        [Header("Scenario B — Expected Stat")]
        [SerializeField]
        private StatType modifierTestStatType =
            StatType.Damage;

        [ContextMenu("Run Equipment Comparison Display Test")]
        private void RunTest()
        {
            int passedChecks = 0;
            int totalChecks = 0;

            Debug.Log(
                "[Display Test] " +
                "==================================================");

            Debug.Log(
                "[Display Test] " +
                "START — Equipment comparison display validation");

            Debug.Log(
                "[Display Test] " +
                "==================================================");

            //==========================================================
            // References
            //==========================================================

            totalChecks++;

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 1 FAILED — EquipmentStatDatabase is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 1 PASSED — EquipmentStatDatabase assigned.");

            totalChecks++;

            if (normalCandidateItemData == null)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 2 FAILED — Normal candidate ItemBaseData " +
                    "is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 2 PASSED — Normal candidate assigned.");

            totalChecks++;

            if (normalEquippedItemData == null)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 3 FAILED — Normal equipped ItemBaseData " +
                    "is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 3 PASSED — Normal equipped item assigned.");

            totalChecks++;

            if (modifierCandidateItemData == null)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 4 FAILED — Modifier candidate ItemBaseData " +
                    "is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 4 PASSED — Modifier candidate assigned.");

            totalChecks++;

            if (modifierEquippedItemData == null)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 5 FAILED — Modifier equipped ItemBaseData " +
                    "is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
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
                    "[Display Test] " +
                    "CHECK 6 FAILED — GameContentContext.Current is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 6 PASSED — GameContentContext is available.");

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            totalChecks++;

            if (contentResolver == null)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 7 FAILED — ItemContentResolver is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 7 PASSED — ItemContentResolver is available.");

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

            EquipmentComparisonDisplayBuilder displayBuilder =
                new EquipmentComparisonDisplayBuilder();

            Debug.Log(
                "[Display Test] " +
                "Comparison and display services created successfully.");

            //==========================================================
            // SCENARIO A
            //==========================================================

            Debug.Log(
                "[Display Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Display Test] " +
                "SCENARIO A — Normal comparison");

            Debug.Log(
                "[Display Test] " +
                $"Candidate: " +
                $"{normalCandidateItemData.DisplayName}");

            Debug.Log(
                "[Display Test] " +
                $"Equipped: " +
                $"{normalEquippedItemData.DisplayName}");

            ItemInstance normalCandidateItem =
                new ItemInstance(
                    "display-test-normal-candidate",
                    normalCandidateItemData.ContentId,
                    normalCandidateItemData.BaseTier);

            ItemInstance normalEquippedItem =
                new ItemInstance(
                    "display-test-normal-equipped",
                    normalEquippedItemData.ContentId,
                    normalEquippedItemData.BaseTier);

            bool normalComparisonSucceeded =
                comparisonService.TryCompare(
                    normalCandidateItem,
                    normalEquippedItem,
                    out ItemComparisonResult normalResult);

            totalChecks++;

            if (!normalComparisonSucceeded)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 8 FAILED — Scenario A comparison failed.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 8 PASSED — Scenario A comparison succeeded.");

            totalChecks++;

            if (normalResult == null)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 9 FAILED — Scenario A result is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 9 PASSED — Scenario A result created.");

            EquipmentComparisonDisplayData normalDisplayData =
                displayBuilder.Build(normalResult);

            totalChecks++;

            if (normalDisplayData == null)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 10 FAILED — Scenario A display data is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 10 PASSED — Scenario A display data created.");

            Debug.Log(
                "[Display Test] " +
                $"Scenario A display entries: " +
                $"{normalDisplayData.Count}");

            //==========================================================
            // Scenario A — Shared
            //==========================================================

            totalChecks++;

            if (!TryFindEntry(
                    normalDisplayData,
                    sharedStatType,
                    out EquipmentComparisonDisplayEntry sharedEntry))
            {
                Debug.LogError(
                    "[Display Test] " +
                    $"CHECK 11 FAILED — Shared stat " +
                    $"{sharedStatType} was not found.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                $"CHECK 11 PASSED — Shared stat " +
                $"{sharedStatType} found.");

            Debug.Log(
                "[Display Test] " +
                $"Shared State = {sharedEntry.State}");

            Debug.Log(
                "[Display Test] " +
                $"Shared Difference = {sharedEntry.Difference}");

            totalChecks++;

            if (!sharedEntry.HasEquippedValue ||
                !sharedEntry.HasCandidateValue)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 12 FAILED — Shared stat should exist " +
                    "on both items.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 12 PASSED — Shared stat exists on both items.");

            totalChecks++;

            if (!sharedEntry.IsDirectlyComparable)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 13 FAILED — Shared stat should be " +
                    "directly comparable.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 13 PASSED — Shared stat is directly comparable.");

            totalChecks++;

            if (!sharedEntry.HasDifference)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 14 FAILED — Shared stat should expose " +
                    "a difference.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 14 PASSED — Shared stat exposes a difference.");

            totalChecks++;

            if (sharedEntry.State !=
                EquipmentComparisonDisplayState.Reduced)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 15 FAILED — Expected shared state " +
                    "to be Reduced.");

                Debug.LogError(
                    "[Display Test] " +
                    $"Actual state = {sharedEntry.State}");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 15 PASSED — Shared state is Reduced.");

            totalChecks++;

            if (!Mathf.Approximately(
                    sharedEntry.Difference,
                    -5f))
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 16 FAILED — Expected shared difference = -5.");

                Debug.LogError(
                    "[Display Test] " +
                    $"Actual = {sharedEntry.Difference}");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 16 PASSED — Shared difference = -5.");

            //==========================================================
            // Scenario A — Candidate Only
            //==========================================================

            totalChecks++;

            if (!TryFindEntry(
                    normalDisplayData,
                    candidateOnlyStatType,
                    out EquipmentComparisonDisplayEntry candidateOnlyEntry))
            {
                Debug.LogError(
                    "[Display Test] " +
                    $"CHECK 17 FAILED — Candidate-only stat " +
                    $"{candidateOnlyStatType} was not found.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                $"CHECK 17 PASSED — Candidate-only stat " +
                $"{candidateOnlyStatType} found.");

            Debug.Log(
                "[Display Test] " +
                $"Candidate-only State = " +
                $"{candidateOnlyEntry.State}");

            totalChecks++;

            if (candidateOnlyEntry.HasEquippedValue ||
                !candidateOnlyEntry.HasCandidateValue)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 18 FAILED — Candidate-only flags " +
                    "are incorrect.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 18 PASSED — Candidate-only flags are correct.");

            totalChecks++;

            if (candidateOnlyEntry.IsDirectlyComparable)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 19 FAILED — Candidate-only stat must " +
                    "not be directly comparable.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 19 PASSED — Candidate-only stat is not comparable.");

            totalChecks++;

            if (candidateOnlyEntry.HasDifference)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 20 FAILED — Candidate-only stat must " +
                    "not expose a difference.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 20 PASSED — Candidate-only stat has no difference.");

            totalChecks++;

            if (candidateOnlyEntry.State !=
                EquipmentComparisonDisplayState.CandidateOnly)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 21 FAILED — Expected CandidateOnly state.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 21 PASSED — State is CandidateOnly.");

            //==========================================================
            // Scenario A — Equipped Only
            //==========================================================

            totalChecks++;

            if (!TryFindEntry(
                    normalDisplayData,
                    equippedOnlyStatType,
                    out EquipmentComparisonDisplayEntry equippedOnlyEntry))
            {
                Debug.LogError(
                    "[Display Test] " +
                    $"CHECK 22 FAILED — Equipped-only stat " +
                    $"{equippedOnlyStatType} was not found.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                $"CHECK 22 PASSED — Equipped-only stat " +
                $"{equippedOnlyStatType} found.");

            Debug.Log(
                "[Display Test] " +
                $"Equipped-only State = " +
                $"{equippedOnlyEntry.State}");

            totalChecks++;

            if (!equippedOnlyEntry.HasEquippedValue ||
                equippedOnlyEntry.HasCandidateValue)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 23 FAILED — Equipped-only flags " +
                    "are incorrect.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 23 PASSED — Equipped-only flags are correct.");

            totalChecks++;

            if (equippedOnlyEntry.IsDirectlyComparable)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 24 FAILED — Equipped-only stat must " +
                    "not be directly comparable.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 24 PASSED — Equipped-only stat is not comparable.");

            totalChecks++;

            if (equippedOnlyEntry.HasDifference)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 25 FAILED — Equipped-only stat must " +
                    "not expose a difference.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 25 PASSED — Equipped-only stat has no difference.");

            totalChecks++;

            if (equippedOnlyEntry.State !=
                EquipmentComparisonDisplayState.EquippedOnly)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 26 FAILED — Expected EquippedOnly state.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 26 PASSED — State is EquippedOnly.");

            //==========================================================
            // SCENARIO B
            //==========================================================

            Debug.Log(
                "[Display Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Display Test] " +
                "SCENARIO B — Different ModifierType");

            Debug.Log(
                "[Display Test] " +
                $"Candidate: " +
                $"{modifierCandidateItemData.DisplayName}");

            Debug.Log(
                "[Display Test] " +
                $"Equipped: " +
                $"{modifierEquippedItemData.DisplayName}");

            ItemInstance modifierCandidateItem =
                new ItemInstance(
                    "display-test-modifier-candidate",
                    modifierCandidateItemData.ContentId,
                    modifierCandidateItemData.BaseTier);

            ItemInstance modifierEquippedItem =
                new ItemInstance(
                    "display-test-modifier-equipped",
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
                    "[Display Test] " +
                    "CHECK 27 FAILED — Scenario B comparison failed.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 27 PASSED — Scenario B comparison succeeded.");

            totalChecks++;

            if (modifierResult == null)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 28 FAILED — Scenario B result is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 28 PASSED — Scenario B result created.");

            EquipmentComparisonDisplayData modifierDisplayData =
                displayBuilder.Build(modifierResult);

            totalChecks++;

            if (modifierDisplayData == null)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 29 FAILED — Scenario B display data is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 29 PASSED — Scenario B display data created.");

            totalChecks++;

            if (!TryFindEntry(
                    modifierDisplayData,
                    modifierTestStatType,
                    out EquipmentComparisonDisplayEntry modifierEntry))
            {
                Debug.LogError(
                    "[Display Test] " +
                    $"CHECK 30 FAILED — Modifier test stat " +
                    $"{modifierTestStatType} was not found.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                $"CHECK 30 PASSED — Modifier test stat " +
                $"{modifierTestStatType} found.");

            Debug.Log(
                "[Display Test] " +
                $"Modifier State = {modifierEntry.State}");

            Debug.Log(
                "[Display Test] " +
                $"Modifier Difference = {modifierEntry.Difference}");

            Debug.Log(
                "[Display Test] " +
                $"Modifier Equipped Value = " +
                $"{modifierEntry.EquippedValue}");

            Debug.Log(
                "[Display Test] " +
                $"Modifier Equipped ModifierType = " +
                $"{modifierEntry.EquippedModifierType}");

            Debug.Log(
                "[Display Test] " +
                $"Modifier Candidate Value = " +
                $"{modifierEntry.CandidateValue}");

            Debug.Log(
                "[Display Test] " +
                $"Modifier Candidate ModifierType = " +
                $"{modifierEntry.CandidateModifierType}");

            Debug.Log(
                "[Display Test] " +
                $"Modifier HasSameModifierType = " +
                $"{modifierEntry.EquippedModifierType == modifierEntry.CandidateModifierType}");

            Debug.Log(
                "[Display Test] " +
                $"Modifier IsDirectlyComparable = " +
                $"{modifierEntry.IsDirectlyComparable}");

            totalChecks++;

            if (modifierEntry.IsDirectlyComparable)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 31 FAILED — Different ModifierTypes " +
                    "must not be directly comparable.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 31 PASSED — Different ModifierTypes " +
                "are not directly comparable.");

            totalChecks++;

            if (modifierEntry.HasDifference)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 32 FAILED — Different ModifierTypes " +
                    "must not expose a display difference.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 32 PASSED — No display difference exposed.");

            totalChecks++;

            if (modifierEntry.State !=
                EquipmentComparisonDisplayState
                    .DifferentModifierType)
            {
                Debug.LogError(
                    "[Display Test] " +
                    "CHECK 33 FAILED — Expected " +
                    "DifferentModifierType state.");

                Debug.LogError(
                    "[Display Test] " +
                    $"Actual state = {modifierEntry.State}");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Display Test] " +
                "CHECK 33 PASSED — State is " +
                "DifferentModifierType.");

            //==========================================================
            // Final Summary
            //==========================================================

            Debug.Log(
                "[Display Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Display Test] EXPECTED DISPLAY MODEL:");

            Debug.Log(
                "[Display Test] " +
                "Damage → Reduced → Difference = -5");

            Debug.Log(
                "[Display Test] " +
                "AttackSpeed → CandidateOnly → No Difference");

            Debug.Log(
                "[Display Test] " +
                "Luck → EquippedOnly → No Difference");

            Debug.Log(
                "[Display Test] " +
                "Different ModifierType → No Difference");

            Debug.Log(
                "[Display Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Display Test] ACTUAL DISPLAY MODEL:");

            Debug.Log(
                "[Display Test] " +
                $"Damage → {sharedEntry.State} → " +
                $"Difference={sharedEntry.Difference}");

            Debug.Log(
                "[Display Test] " +
                $"AttackSpeed → {candidateOnlyEntry.State} → " +
                $"HasDifference={candidateOnlyEntry.HasDifference}");

            Debug.Log(
                "[Display Test] " +
                $"Luck → {equippedOnlyEntry.State} → " +
                $"HasDifference={equippedOnlyEntry.HasDifference}");

            Debug.Log(
                "[Display Test] " +
                $"Modifier Damage → {modifierEntry.State} → " +
                $"HasDifference={modifierEntry.HasDifference}");

            Debug.Log(
                "[Display Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Display Test] " +
                $"VALIDATION RESULT: " +
                $"{passedChecks}/{totalChecks} checks passed.");

            if (passedChecks == totalChecks)
            {
                Debug.Log(
                    "[Display Test] " +
                    "RULE VERIFIED: Display data exposes differences " +
                    "only for directly comparable stats.");

                Debug.Log(
                    "[Display Test] " +
                    "RULE VERIFIED: Exclusive stats remain neutral " +
                    "and visible without comparison differences.");

                Debug.Log(
                    "[Display Test] " +
                    "RULE VERIFIED: Different ModifierTypes remain " +
                    "visible without a numeric comparison.");

                Debug.Log(
                    "[Display Test] " +
                    "✓ TEST PASSED — Equipment comparison display " +
                    "model is working correctly.");
            }
            else
            {
                Debug.LogError(
                    "[Display Test] " +
                    "TEST FAILED — One or more validation checks failed.");
            }

            Debug.Log(
                "[Display Test] " +
                "==================================================");
        }

        private bool TryFindEntry(
            EquipmentComparisonDisplayData displayData,
            StatType statType,
            out EquipmentComparisonDisplayEntry entry)
        {
            entry = default;

            if (displayData == null)
                return false;

            for (int i = 0;
                 i < displayData.Entries.Count;
                 i++)
            {
                EquipmentComparisonDisplayEntry current =
                    displayData.Entries[i];

                if (current.StatType != statType)
                    continue;

                entry = current;

                return true;
            }

            return false;
        }
    }
}