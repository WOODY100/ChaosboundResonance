using System.Collections.Generic;
using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentProgressionRuntimeTest : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private EquipmentProgressionConfig progressionConfig;
        [SerializeField] private EquipmentStatDatabase statDatabase;
        [SerializeField] private ItemBaseData testBaseData;

        [ContextMenu("Run Equipment Progression Test")]
        private void RunTest()
        {
            if (progressionConfig == null)
            {
                Debug.LogError(
                    "[Equipment Test] Missing EquipmentProgressionConfig.");
                return;
            }

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Test] Missing EquipmentStatDatabase.");
                return;
            }

            if (testBaseData == null)
            {
                Debug.LogError(
                    "[Equipment Test] Missing ItemBaseData.");
                return;
            }

            EquipmentTierOptionGenerator generator =
                new EquipmentTierOptionGenerator(
                    statDatabase,
                    progressionConfig.OptionsPerTier);

            EquipmentProgressionRuntime runtime =
                new EquipmentProgressionRuntime(
                    progressionConfig,
                    generator);

            ItemInstance itemInstance =
                new ItemInstance(
                    "equipment-test-instance",
                    testBaseData.ContentId,
                    ItemTier.Common);

            Debug.Log(
                $"[Equipment Test] Initial state: " +
                $"Tier={itemInstance.CurrentTier}, " +
                $"Upgrade={itemInstance.UpgradeLevel}");

            for (int i = 0;
                 i < progressionConfig.UpgradesPerTier;
                 i++)
            {
                bool upgraded =
                    runtime.TryUpgrade(itemInstance);

                if (!upgraded)
                {
                    Debug.LogError(
                        $"[Equipment Test] Upgrade {i + 1} failed.");
                    return;
                }

                Debug.Log(
                    $"[Equipment Test] Upgrade successful: " +
                    $"Tier={itemInstance.CurrentTier}, " +
                    $"Upgrade={itemInstance.UpgradeLevel}");
            }

            if (!runtime.IsReadyForTierUp(itemInstance))
            {
                Debug.LogError(
                    "[Equipment Test] Item should be ready for Tier Up.");
                return;
            }

            Debug.Log(
                "[Equipment Test] Item correctly reached " +
                "the Tier Up state.");

            bool generated =
                runtime.TryGenerateTierOptions(
                    testBaseData,
                    itemInstance,
                    out List<EquipmentStatOption> options);

            if (!generated)
            {
                Debug.LogError(
                    "[Equipment Test] Failed to generate Tier options.");
                return;
            }

            if (options.Count != progressionConfig.OptionsPerTier)
            {
                Debug.LogError(
                    $"[Equipment Test] Expected " +
                    $"{progressionConfig.OptionsPerTier} options, " +
                    $"but received {options.Count}.");
                return;
            }

            Debug.Log(
                $"[Equipment Test] Generated {options.Count} options:");

            for (int i = 0; i < options.Count; i++)
            {
                EquipmentStatOption option = options[i];

                Debug.Log(
                    $"[Equipment Test] Option {i + 1}: " +
                    $"{option.StatType} | " +
                    $"{option.ModifierType} | " +
                    $"{option.RolledValue}");
            }

            EquipmentStatOption selectedOption =
                options[0];

            Debug.Log(
                $"[Equipment Test] Selecting option: " +
                $"{selectedOption.StatType}");

            bool tierUp =
                runtime.TryApplyTierUp(
                    itemInstance,
                    selectedOption);

            if (!tierUp)
            {
                Debug.LogError(
                    "[Equipment Test] Failed to apply Tier Up.");
                return;
            }

            Debug.Log(
                $"[Equipment Test] Tier Up successful: " +
                $"Tier={itemInstance.CurrentTier}, " +
                $"Upgrade={itemInstance.UpgradeLevel}");

            Debug.Log(
                $"[Equipment Test] Permanent unlocked stats: " +
                $"{itemInstance.UnlockedStats.Count}");

            for (int i = 0;
                 i < itemInstance.UnlockedStats.Count;
                 i++)
            {
                EquipmentRolledStat stat =
                    itemInstance.UnlockedStats[i];

                Debug.Log(
                    $"[Equipment Test] Unlocked {i + 1}: " +
                    $"{stat.StatType} | " +
                    $"{stat.ModifierType} | " +
                    $"{stat.RolledValue}");
            }

            if (itemInstance.CurrentTier != ItemTier.Uncommon)
            {
                Debug.LogError(
                    "[Equipment Test] Tier should now be Uncommon.");
                return;
            }

            if (itemInstance.UpgradeLevel != 0)
            {
                Debug.LogError(
                    "[Equipment Test] UpgradeLevel should reset to 0.");
                return;
            }

            if (itemInstance.UnlockedStats.Count != 1)
            {
                Debug.LogError(
                    "[Equipment Test] Exactly one stat should " +
                    "have been permanently unlocked.");
                return;
            }

            if (!itemInstance.HasUnlockedStat(
                    selectedOption.StatType))
            {
                Debug.LogError(
                    "[Equipment Test] Selected stat was not preserved.");
                return;
            }

            Debug.Log(
                "[Equipment Test] ✓ COMPLETE — " +
                "Equipment progression flow is working correctly.");
        }
    }
}