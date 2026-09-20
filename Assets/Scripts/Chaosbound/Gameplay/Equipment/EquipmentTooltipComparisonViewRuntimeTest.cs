using System.Collections;
using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.UI.Tooltip;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentTooltipComparisonViewRuntimeTest
        : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField]
        private EquipmentStatDatabase statDatabase;

        [SerializeField]
        private ItemBaseData candidateItemData;

        [SerializeField]
        private ItemBaseData equippedItemData;

        [SerializeField]
        private EquipmentTooltipStatsView statsView;

        [Header("Display Configuration")]
        [SerializeField]
        private bool showCandidateValues = true;

        [Header("Initialization")]
        [SerializeField]
        private int maxInitializationWaitFrames = 120;

        private Coroutine testCoroutine;

        [ContextMenu("Run Equipment Tooltip Comparison View Test")]
        private void RunTest()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning(
                    "[Tooltip Comparison View Test] " +
                    "The test must be executed in Play Mode.",
                    this);

                return;
            }

            if (testCoroutine != null)
            {
                Debug.LogWarning(
                    "[Tooltip Comparison View Test] " +
                    "A test is already running.",
                    this);

                return;
            }

            testCoroutine =
                StartCoroutine(
                    RunTestAfterInitialization());
        }

        private IEnumerator RunTestAfterInitialization()
        {
            int waitedFrames = 0;

            while (GameContentContext.Current == null &&
                   waitedFrames < maxInitializationWaitFrames)
            {
                waitedFrames++;

                yield return null;
            }

            if (GameContentContext.Current == null)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "GameContentContext was not available after " +
                    maxInitializationWaitFrames +
                    " frames.",
                    this);

                testCoroutine = null;
                yield break;
            }

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "GameContentContext is ready after " +
                waitedFrames +
                " frame(s).");

            RunTestInternal();

            testCoroutine = null;
        }

        private void RunTestInternal()
        {
            Debug.Log(
                "================================================");

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "START — Comparison data to tooltip view");

            Debug.Log(
                "================================================");

            int checksPassed = 0;
            const int totalChecks = 13;

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 1 FAILED — EquipmentStatDatabase is missing.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 1 PASSED — EquipmentStatDatabase assigned.");

            if (candidateItemData == null)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 2 FAILED — Candidate ItemBaseData is missing.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 2 PASSED — Candidate ItemBaseData assigned.");

            if (equippedItemData == null)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 3 FAILED — Equipped ItemBaseData is missing.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 3 PASSED — Equipped ItemBaseData assigned.");

            if (statsView == null)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 4 FAILED — EquipmentTooltipStatsView is missing.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 4 PASSED — EquipmentTooltipStatsView assigned.");

            if (candidateItemData.Category != ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 5 FAILED — Candidate is not Equipment.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 5 PASSED — Candidate is Equipment.");

            if (equippedItemData.Category != ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 6 FAILED — Equipped item is not Equipment.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 6 PASSED — Equipped item is Equipment.");

            if (candidateItemData.EquipmentType == EquipmentType.None)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 7 FAILED — Candidate has invalid EquipmentType.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 7 PASSED — Candidate has valid EquipmentType.");

            if (equippedItemData.EquipmentType == EquipmentType.None)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 8 FAILED — Equipped item has invalid EquipmentType.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 8 PASSED — Equipped item has valid EquipmentType.");

            if (candidateItemData.EquipmentType !=
                equippedItemData.EquipmentType)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 9 FAILED — Candidate and equipped items " +
                    "must use the same EquipmentType.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 9 PASSED — Both items use " +
                candidateItemData.EquipmentType +
                ".");

            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 10 FAILED — GameContentContext unavailable.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 10 PASSED — GameContentContext available.");

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            if (contentResolver == null)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 11 FAILED — ItemContentResolver unavailable.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 11 PASSED — ItemContentResolver available.");

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
                "[Tooltip Comparison View Test] " +
                "Comparison and display builders created.");

            ItemInstance candidateItem =
                new ItemInstance(
                    "tooltip-view-test-candidate",
                    candidateItemData.ContentId,
                    candidateItemData.BaseTier);

            ItemInstance equippedItem =
                new ItemInstance(
                    "tooltip-view-test-equipped",
                    equippedItemData.ContentId,
                    equippedItemData.BaseTier);

            Debug.Log(
                "[Tooltip Comparison View Test] Candidate: " +
                candidateItemData.DisplayName);

            Debug.Log(
                "[Tooltip Comparison View Test] Equipped: " +
                equippedItemData.DisplayName);

            if (!comparisonService.TryCompare(
                    candidateItem,
                    equippedItem,
                    out ItemComparisonResult comparisonResult))
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 12 FAILED — Item comparison failed.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 12 PASSED — Item comparison succeeded.");

            EquipmentComparisonDisplayData displayData =
                displayBuilder.Build(
                    comparisonResult);

            if (displayData == null)
            {
                Debug.LogError(
                    "[Tooltip Comparison View Test] " +
                    "CHECK 13 FAILED — DisplayData is null.",
                    this);

                return;
            }

            checksPassed++;

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "CHECK 13 PASSED — DisplayData created.");

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "Display entries: " +
                displayData.Count);

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "----------------------------------------");

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "DISPLAY MODE: " +
                (showCandidateValues
                    ? "CANDIDATE"
                    : "EQUIPPED"));

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "----------------------------------------");

            for (int i = 0;
                 i < displayData.Entries.Count;
                 i++)
            {
                EquipmentComparisonDisplayEntry entry =
                    displayData.Entries[i];

                bool hasValue =
                    showCandidateValues
                        ? entry.HasCandidateValue
                        : entry.HasEquippedValue;

                if (!hasValue)
                {
                    Debug.Log(
                        "[Tooltip Comparison View Test] " +
                        entry.StatType +
                        " → Not present in this item.");

                    continue;
                }

                ModifierType modifierType =
                    showCandidateValues
                        ? entry.CandidateModifierType
                        : entry.EquippedModifierType;

                float value =
                    showCandidateValues
                        ? entry.CandidateValue
                        : entry.EquippedValue;

                Debug.Log(
                    "[Tooltip Comparison View Test] " +
                    entry.StatType +
                    " | Value=" +
                    FormatValue(
                        modifierType,
                        value) +
                    " | State=" +
                    entry.State +
                    " | HasDifference=" +
                    entry.HasDifference +
                    " | Difference=" +
                    entry.Difference);
            }

            statsView.ShowComparison(
                displayData.Entries,
                showCandidateValues);

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "StatsView.ShowComparison executed.");

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "========================================");

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "VALIDATION RESULT: " +
                checksPassed +
                "/" +
                totalChecks +
                " checks passed.");

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "Rendered rows: " +
                statsView.RowCount);

            Debug.Log(
                "[Tooltip Comparison View Test] " +
                "========================================");

            if (checksPassed == totalChecks)
            {
                Debug.Log(
                    "[Tooltip Comparison View Test] " +
                    "✓ TEST PASSED — Comparison data is being " +
                    "sent correctly to EquipmentTooltipStatsView.");
            }
        }

        private static string FormatValue(
            ModifierType modifierType,
            float value)
        {
            switch (modifierType)
            {
                case ModifierType.Flat:
                    return value.ToString("0.##");

                case ModifierType.Percent:
                    return (value * 100f).ToString("0.##") + "%";

                case ModifierType.FinalMultiplier:
                    return "x" + value.ToString("0.##");

                default:
                    return value.ToString("0.##");
            }
        }
    }
}