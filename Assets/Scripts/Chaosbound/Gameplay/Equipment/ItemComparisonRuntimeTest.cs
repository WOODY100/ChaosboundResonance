using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class ItemComparisonRuntimeTest : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private EquipmentStatDatabase statDatabase;
        [SerializeField] private ItemBaseData candidateItemData;
        [SerializeField] private ItemBaseData equippedItemData;

        [ContextMenu("Run Item Comparison Test")]
        private void RunTest()
        {
            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Comparison Test] " +
                    "Missing EquipmentStatDatabase.");

                return;
            }

            if (candidateItemData == null)
            {
                Debug.LogError(
                    "[Equipment Comparison Test] " +
                    "Missing Candidate ItemBaseData.");

                return;
            }

            if (equippedItemData == null)
            {
                Debug.LogError(
                    "[Equipment Comparison Test] " +
                    "Missing Equipped ItemBaseData.");

                return;
            }

            if (candidateItemData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Comparison Test] " +
                    "Candidate item is not Equipment.");

                return;
            }

            if (equippedItemData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Comparison Test] " +
                    "Equipped item is not Equipment.");

                return;
            }

            if (candidateItemData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Comparison Test] " +
                    "Candidate item has EquipmentType.None.");

                return;
            }

            if (equippedItemData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Comparison Test] " +
                    "Equipped item has EquipmentType.None.");

                return;
            }

            if (candidateItemData.EquipmentType !=
                equippedItemData.EquipmentType)
            {
                Debug.LogError(
                    "[Equipment Comparison Test] " +
                    "Candidate and equipped items must have " +
                    "the same EquipmentType.");

                return;
            }

            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[Equipment Comparison Test] " +
                    "GameContentContext.Current is null. " +
                    "Enter Play Mode before running the test.");

                return;
            }

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            if (contentResolver == null)
            {
                Debug.LogError(
                    "[Equipment Comparison Test] " +
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
                    "comparison-test-candidate",
                    candidateItemData.ContentId,
                    candidateItemData.BaseTier);

            ItemInstance equippedItem =
                new ItemInstance(
                    "comparison-test-equipped",
                    equippedItemData.ContentId,
                    equippedItemData.BaseTier);

            Debug.Log(
                "[Equipment Comparison Test] " +
                "========== TEST START ==========");

            Debug.Log(
                $"[Equipment Comparison Test] Candidate: " +
                $"{candidateItemData.DisplayName} | " +
                $"{candidateItemData.EquipmentType}");

            Debug.Log(
                $"[Equipment Comparison Test] Equipped: " +
                $"{equippedItemData.DisplayName} | " +
                $"{equippedItemData.EquipmentType}");

            bool comparisonSucceeded =
                comparisonService.TryCompare(
                    candidateItem,
                    equippedItem,
                    out ItemComparisonResult result);

            if (!comparisonSucceeded)
            {
                Debug.LogError(
                    "[Equipment Comparison Test] " +
                    "Comparison failed.");

                return;
            }

            if (result == null)
            {
                Debug.LogError(
                    "[Equipment Comparison Test] " +
                    "Comparison succeeded but result is null.");

                return;
            }

            Debug.Log(
                "[Equipment Comparison Test] " +
                "Comparison succeeded.");

            Debug.Log(
                $"[Equipment Comparison Test] " +
                $"Equipment Type: {result.EquipmentType}");

            Debug.Log(
                $"[Equipment Comparison Test] " +
                $"Comparison Stats: {result.Stats.Count}");

            for (int i = 0; i < result.Stats.Count; i++)
            {
                EquipmentComparisonStat stat =
                    result.Stats[i];

                string equippedValue =
                    stat.HasEquippedValue
                        ? $"{stat.EquippedValue} " +
                          $"({stat.EquippedModifierType})"
                        : "NONE";

                string candidateValue =
                    stat.HasCandidateValue
                        ? $"{stat.CandidateValue} " +
                          $"({stat.CandidateModifierType})"
                        : "NONE";

                string difference =
                    stat.HasEquippedValue &&
                    stat.HasCandidateValue
                        ? stat.Difference.ToString()
                        : "N/A";

                Debug.Log(
                    $"[Equipment Comparison Test] " +
                    $"STAT [{stat.StatType}] | " +
                    $"Equipped: {equippedValue} | " +
                    $"Candidate: {candidateValue} | " +
                    $"Difference: {difference}");
            }

            Debug.Log(
                "[Equipment Comparison Test] " +
                "✓ COMPLETE — Item comparison is working correctly.");
        }
    }
}