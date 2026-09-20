using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentComparisonTypeValidationRuntimeTest :
        MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField]
        private EquipmentStatDatabase statDatabase;

        [SerializeField]
        private ItemBaseData candidateItemData;

        [SerializeField]
        private ItemBaseData equippedItemData;

        [ContextMenu("Run Equipment Type Validation Test")]
        private void RunTest()
        {
            int passedChecks = 0;
            int totalChecks = 0;

            Debug.Log(
                "[Equipment Type Test] " +
                "==================================================");

            Debug.Log(
                "[Equipment Type Test] " +
                "START — EquipmentType compatibility validation");

            Debug.Log(
                "[Equipment Type Test] " +
                "==================================================");

            //==========================================================
            // References
            //==========================================================

            totalChecks++;

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Type Test] " +
                    "CHECK 1 FAILED — EquipmentStatDatabase is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Equipment Type Test] " +
                "CHECK 1 PASSED — EquipmentStatDatabase assigned.");

            totalChecks++;

            if (candidateItemData == null)
            {
                Debug.LogError(
                    "[Equipment Type Test] " +
                    "CHECK 2 FAILED — Candidate ItemBaseData is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Equipment Type Test] " +
                "CHECK 2 PASSED — Candidate ItemBaseData assigned.");

            totalChecks++;

            if (equippedItemData == null)
            {
                Debug.LogError(
                    "[Equipment Type Test] " +
                    "CHECK 3 FAILED — Equipped ItemBaseData is missing.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Equipment Type Test] " +
                "CHECK 3 PASSED — Equipped ItemBaseData assigned.");

            //==========================================================
            // Item Classification
            //==========================================================

            totalChecks++;

            if (candidateItemData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Type Test] " +
                    "CHECK 4 FAILED — Candidate is not Equipment.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Equipment Type Test] " +
                "CHECK 4 PASSED — Candidate is Equipment.");

            totalChecks++;

            if (equippedItemData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Type Test] " +
                    "CHECK 5 FAILED — Equipped item is not Equipment.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Equipment Type Test] " +
                "CHECK 5 PASSED — Equipped item is Equipment.");

            //==========================================================
            // EquipmentType
            //==========================================================

            totalChecks++;

            if (candidateItemData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Type Test] " +
                    "CHECK 6 FAILED — Candidate has EquipmentType.None.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Equipment Type Test] " +
                "CHECK 6 PASSED — Candidate has a valid EquipmentType.");

            totalChecks++;

            if (equippedItemData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Type Test] " +
                    "CHECK 7 FAILED — Equipped item has " +
                    "EquipmentType.None.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Equipment Type Test] " +
                "CHECK 7 PASSED — Equipped item has a valid EquipmentType.");

            //==========================================================
            // Test Configuration
            //==========================================================

            Debug.Log(
                "[Equipment Type Test] " +
                $"Candidate: {candidateItemData.DisplayName}");

            Debug.Log(
                "[Equipment Type Test] " +
                $"Candidate EquipmentType: " +
                $"{candidateItemData.EquipmentType}");

            Debug.Log(
                "[Equipment Type Test] " +
                $"Equipped: {equippedItemData.DisplayName}");

            Debug.Log(
                "[Equipment Type Test] " +
                $"Equipped EquipmentType: " +
                $"{equippedItemData.EquipmentType}");

            totalChecks++;

            if (candidateItemData.EquipmentType ==
                equippedItemData.EquipmentType)
            {
                Debug.LogError(
                    "[Equipment Type Test] " +
                    "CHECK 8 FAILED — Test configuration is invalid. " +
                    "Candidate and equipped items have the same " +
                    "EquipmentType. Assign different equipment types.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Equipment Type Test] " +
                "CHECK 8 PASSED — Candidate and equipped items " +
                "have different EquipmentTypes.");

            Debug.Log(
                "[Equipment Type Test] " +
                "Expected behavior: ItemComparisonService must " +
                "REJECT this comparison.");

            //==========================================================
            // Runtime Dependencies
            //==========================================================

            GameContentContext contentContext =
                GameContentContext.Current;

            totalChecks++;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[Equipment Type Test] " +
                    "CHECK 9 FAILED — GameContentContext.Current is null. " +
                    "Enter Play Mode before running the test.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Equipment Type Test] " +
                "CHECK 9 PASSED — GameContentContext is available.");

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            totalChecks++;

            if (contentResolver == null)
            {
                Debug.LogError(
                    "[Equipment Type Test] " +
                    "CHECK 10 FAILED — ItemContentResolver is null.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Equipment Type Test] " +
                "CHECK 10 PASSED — ItemContentResolver is available.");

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
                "[Equipment Type Test] " +
                "ItemComparisonService created successfully.");

            //==========================================================
            // Item Instances
            //==========================================================

            ItemInstance candidateItem =
                new ItemInstance(
                    "equipment-type-test-candidate",
                    candidateItemData.ContentId,
                    candidateItemData.BaseTier);

            ItemInstance equippedItem =
                new ItemInstance(
                    "equipment-type-test-equipped",
                    equippedItemData.ContentId,
                    equippedItemData.BaseTier);

            Debug.Log(
                "[Equipment Type Test] " +
                $"Candidate InstanceId: {candidateItem.InstanceId}");

            Debug.Log(
                "[Equipment Type Test] " +
                $"Equipped InstanceId: {equippedItem.InstanceId}");

            //==========================================================
            // Comparison
            //==========================================================

            Debug.Log(
                "[Equipment Type Test] " +
                "Executing ItemComparisonService.TryCompare...");

            bool comparisonSucceeded =
                comparisonService.TryCompare(
                    candidateItem,
                    equippedItem,
                    out ItemComparisonResult result);

            //==========================================================
            // Expected Result #1
            //==========================================================

            totalChecks++;

            if (comparisonSucceeded)
            {
                Debug.LogError(
                    "[Equipment Type Test] " +
                    "CHECK 11 FAILED — TryCompare returned TRUE.");

                Debug.LogError(
                    "[Equipment Type Test] " +
                    "The service incorrectly allowed comparison " +
                    "between different EquipmentTypes.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Equipment Type Test] " +
                "CHECK 11 PASSED — TryCompare returned FALSE.");

            Debug.Log(
                "[Equipment Type Test] " +
                "Comparison was correctly rejected.");

            //==========================================================
            // Expected Result #2
            //==========================================================

            totalChecks++;

            if (result != null)
            {
                Debug.LogError(
                    "[Equipment Type Test] " +
                    "CHECK 12 FAILED — Result is not null.");

                Debug.LogError(
                    "[Equipment Type Test] " +
                    "A rejected comparison must not produce " +
                    "an ItemComparisonResult.");

                return;
            }

            passedChecks++;

            Debug.Log(
                "[Equipment Type Test] " +
                "CHECK 12 PASSED — ItemComparisonResult is null.");

            //==========================================================
            // Final Result
            //==========================================================

            Debug.Log(
                "[Equipment Type Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Equipment Type Test] " +
                "EXPECTED:");

            Debug.Log(
                "[Equipment Type Test] " +
                $"Candidate Type = {candidateItemData.EquipmentType}");

            Debug.Log(
                "[Equipment Type Test] " +
                $"Equipped Type = {equippedItemData.EquipmentType}");

            Debug.Log(
                "[Equipment Type Test] " +
                "TryCompare() = FALSE");

            Debug.Log(
                "[Equipment Type Test] " +
                "ItemComparisonResult = NULL");

            Debug.Log(
                "[Equipment Type Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Equipment Type Test] " +
                "ACTUAL:");

            Debug.Log(
                "[Equipment Type Test] " +
                $"TryCompare() = {comparisonSucceeded}");

            Debug.Log(
                "[Equipment Type Test] " +
                $"ItemComparisonResult = " +
                $"{(result == null ? "NULL" : "NOT NULL")}");

            Debug.Log(
                "[Equipment Type Test] " +
                "--------------------------------------------------");

            Debug.Log(
                "[Equipment Type Test] " +
                $"VALIDATION RESULT: " +
                $"{passedChecks}/{totalChecks} checks passed.");

            Debug.Log(
                "[Equipment Type Test] " +
                "RULE VERIFIED: EquipmentComparisonService " +
                "rejects comparisons between different EquipmentTypes.");

            Debug.Log(
                "[Equipment Type Test] " +
                "✓ TEST PASSED — EquipmentType validation is working correctly.");

            Debug.Log(
                "[Equipment Type Test] " +
                "==================================================");
        }
    }
}