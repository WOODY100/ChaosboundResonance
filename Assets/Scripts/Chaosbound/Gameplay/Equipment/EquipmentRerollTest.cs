using System.Collections.Generic;
using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentRerollTest : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private EquipmentStatDatabase statDatabase;
        [SerializeField] private EquipmentProgressionConfig progressionConfig;
        [SerializeField] private ItemBaseData equipmentBaseData;

        [ContextMenu("Run Equipment Reroll Test")]
        private void RunTest()
        {
            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Reroll Test] Missing ItemDatabase.");
                return;
            }

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Reroll Test] Missing EquipmentStatDatabase.");
                return;
            }

            if (progressionConfig == null)
            {
                Debug.LogError(
                    "[Equipment Reroll Test] Missing " +
                    "EquipmentProgressionConfig.");
                return;
            }

            if (equipmentBaseData == null)
            {
                Debug.LogError(
                    "[Equipment Reroll Test] Missing Equipment ItemBaseData.");
                return;
            }

            if (equipmentBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Reroll Test] ItemBaseData is not Equipment.");
                return;
            }

            if (equipmentBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Reroll Test] EquipmentType is None.");
                return;
            }

            if (!itemDatabase.TryGet(
                    equipmentBaseData.ContentId,
                    out ItemBaseData resolvedBaseData))
            {
                Debug.LogError(
                    $"[Equipment Reroll Test] Could not resolve " +
                    $"'{equipmentBaseData.ContentId}'.");
                return;
            }

            if (resolvedBaseData != equipmentBaseData)
            {
                Debug.LogError(
                    "[Equipment Reroll Test] ItemDatabase resolved " +
                    "a different ItemBaseData.");
                return;
            }

            EquipmentTierOptionGenerator optionGenerator =
                new EquipmentTierOptionGenerator(
                    statDatabase,
                    progressionConfig.OptionsPerTier);

            EquipmentProgressionRuntime progressionRuntime =
                new EquipmentProgressionRuntime(
                    progressionConfig,
                    optionGenerator);

            EquipmentUpgradeService service =
                new EquipmentUpgradeService(
                    itemDatabase,
                    progressionRuntime);

            ItemInstance itemInstance =
                new ItemInstance(
                    "equipment-reroll-test-instance",
                    equipmentBaseData.ContentId,
                    equipmentBaseData.BaseTier);

            Debug.Log(
                $"[Equipment Reroll Test] Initial state: " +
                $"Tier={itemInstance.CurrentTier}, " +
                $"Upgrade={itemInstance.UpgradeLevel}, " +
                $"UnlockedStats={itemInstance.UnlockedStats.Count}");

            if (!ReachTierUpState(
                    progressionRuntime,
                    itemInstance))
            {
                Debug.LogError(
                    "[Equipment Reroll Test] Failed to reach " +
                    "Tier Up state.");
                return;
            }

            Debug.Log(
                $"[Equipment Reroll Test] Tier Up state reached: " +
                $"Tier={itemInstance.CurrentTier}, " +
                $"Upgrade={itemInstance.UpgradeLevel}");

            if (!service.TryGenerateTierOptions(
                    itemInstance,
                    out List<EquipmentStatOption> initialOptions))
            {
                Debug.LogError(
                    "[Equipment Reroll Test] Failed to generate " +
                    "initial options.");
                return;
            }

            if (!ValidateOptionCount(
                    initialOptions,
                    progressionConfig.OptionsPerTier,
                    "Initial"))
            {
                return;
            }

            LogOptions(
                "Initial",
                initialOptions);

            ItemTier tierBeforeReroll =
                itemInstance.CurrentTier;

            int upgradeBeforeReroll =
                itemInstance.UpgradeLevel;

            int unlockedBeforeReroll =
                itemInstance.UnlockedStats.Count;

            if (!service.TryRerollTierOptions(
                    itemInstance,
                    out List<EquipmentStatOption> rerolledOptions))
            {
                Debug.LogError(
                    "[Equipment Reroll Test] Failed to generate " +
                    "rerolled options.");
                return;
            }

            if (!ValidateOptionCount(
                    rerolledOptions,
                    progressionConfig.OptionsPerTier,
                    "Rerolled"))
            {
                return;
            }

            LogOptions(
                "Rerolled",
                rerolledOptions);

            if (itemInstance.CurrentTier !=
                tierBeforeReroll)
            {
                Debug.LogError(
                    "[Equipment Reroll Test] Tier changed during Reroll.");
                return;
            }

            if (itemInstance.UpgradeLevel !=
                upgradeBeforeReroll)
            {
                Debug.LogError(
                    "[Equipment Reroll Test] UpgradeLevel changed " +
                    "during Reroll.");
                return;
            }

            if (itemInstance.UnlockedStats.Count !=
                unlockedBeforeReroll)
            {
                Debug.LogError(
                    "[Equipment Reroll Test] UnlockedStats changed " +
                    "during Reroll.");
                return;
            }

            if (itemInstance.InstanceId !=
                "equipment-reroll-test-instance")
            {
                Debug.LogError(
                    "[Equipment Reroll Test] InstanceId changed " +
                    "during Reroll.");
                return;
            }

            if (itemInstance.BaseDataId !=
                equipmentBaseData.ContentId)
            {
                Debug.LogError(
                    "[Equipment Reroll Test] BaseDataId changed " +
                    "during Reroll.");
                return;
            }

            Debug.Log(
                "[Equipment Reroll Test] ItemInstance remained " +
                "completely unchanged.");

            Debug.Log(
                "[Equipment Reroll Test] ✓ COMPLETE — " +
                "Reroll correctly regenerates temporary options " +
                "without modifying permanent Equipment state.");
        }

        private static bool ReachTierUpState(
            EquipmentProgressionRuntime progressionRuntime,
            ItemInstance itemInstance)
        {
            for (int i = 0; i < 5; i++)
            {
                if (!progressionRuntime.TryUpgrade(
                        itemInstance))
                {
                    return false;
                }
            }

            return progressionRuntime.IsReadyForTierUp(
                itemInstance);
        }

        private static bool ValidateOptionCount(
            List<EquipmentStatOption> options,
            int expectedCount,
            string label)
        {
            if (options == null)
            {
                Debug.LogError(
                    $"[Equipment Reroll Test] {label} options are null.");
                return false;
            }

            if (options.Count != expectedCount)
            {
                Debug.LogError(
                    $"[Equipment Reroll Test] Invalid {label} option " +
                    $"count. Expected={expectedCount}, " +
                    $"Actual={options.Count}.");
                return false;
            }

            Debug.Log(
                $"[Equipment Reroll Test] {label} options count " +
                $"verified: {options.Count}");

            return true;
        }

        private static void LogOptions(
            string label,
            List<EquipmentStatOption> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                EquipmentStatOption option =
                    options[i];

                Debug.Log(
                    $"[Equipment Reroll Test] {label} Option {i + 1}: " +
                    $"{option.StatType} | " +
                    $"{option.ModifierType} | " +
                    $"RolledValue={option.RolledValue}");
            }
        }
    }
}