using System.Collections.Generic;
using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentUpgradeSessionTest : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private EquipmentStatDatabase statDatabase;
        [SerializeField] private EquipmentProgressionConfig progressionConfig;
        [SerializeField] private ItemBaseData equipmentBaseData;

        [ContextMenu("Run Equipment Upgrade Session Test")]
        private void RunTest()
        {
            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Session Test] Missing ItemDatabase.");
                return;
            }

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Session Test] Missing " +
                    "EquipmentStatDatabase.");
                return;
            }

            if (progressionConfig == null)
            {
                Debug.LogError(
                    "[Equipment Session Test] Missing " +
                    "EquipmentProgressionConfig.");
                return;
            }

            if (equipmentBaseData == null)
            {
                Debug.LogError(
                    "[Equipment Session Test] Missing " +
                    "Equipment ItemBaseData.");
                return;
            }

            if (equipmentBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Session Test] ItemBaseData " +
                    "is not Equipment.");
                return;
            }

            if (equipmentBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Session Test] EquipmentType is None.");
                return;
            }

            if (!itemDatabase.TryGet(
                    equipmentBaseData.ContentId,
                    out ItemBaseData resolvedBaseData))
            {
                Debug.LogError(
                    $"[Equipment Session Test] Could not resolve " +
                    $"'{equipmentBaseData.ContentId}'.");
                return;
            }

            if (resolvedBaseData != equipmentBaseData)
            {
                Debug.LogError(
                    "[Equipment Session Test] ItemDatabase resolved " +
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

            EquipmentUpgradeService upgradeService =
                new EquipmentUpgradeService(
                    itemDatabase,
                    progressionRuntime);

            EquipmentUpgradeSession session =
                new EquipmentUpgradeSession(
                    upgradeService);

            ItemInstance itemInstance =
                new ItemInstance(
                    "equipment-session-test-instance",
                    equipmentBaseData.ContentId,
                    equipmentBaseData.BaseTier);

            Debug.Log(
                $"[Equipment Session Test] Initial state: " +
                $"Tier={itemInstance.CurrentTier}, " +
                $"Upgrade={itemInstance.UpgradeLevel}, " +
                $"UnlockedStats={itemInstance.UnlockedStats.Count}");

            if (!ReachTierUpState(
                    progressionRuntime,
                    itemInstance))
            {
                Debug.LogError(
                    "[Equipment Session Test] Failed to reach " +
                    "Tier Up state.");
                return;
            }

            Debug.Log(
                $"[Equipment Session Test] Tier Up state reached: " +
                $"Tier={itemInstance.CurrentTier}, " +
                $"Upgrade={itemInstance.UpgradeLevel}");

            if (!session.Open(itemInstance))
            {
                Debug.LogError(
                    "[Equipment Session Test] Failed to open session.");
                return;
            }

            if (!session.IsOpen)
            {
                Debug.LogError(
                    "[Equipment Session Test] Session should be open.");
                return;
            }

            if (session.ItemInstance != itemInstance)
            {
                Debug.LogError(
                    "[Equipment Session Test] Session ItemInstance " +
                    "does not match expected instance.");
                return;
            }

            ValidateOptions(
                session.Options,
                progressionConfig.OptionsPerTier,
                "Initial");

            LogOptions(
                "Initial",
                session.Options);

            ValidateItemUnchanged(
                itemInstance,
                ItemTier.Common,
                5,
                0,
                "after Open");

            if (session.HasSelection)
            {
                Debug.LogError(
                    "[Equipment Session Test] Session should not have " +
                    "a selection immediately after Open.");
                return;
            }

            EquipmentStatOption firstOption =
                session.Options[0];

            if (!session.TrySelect(firstOption))
            {
                Debug.LogError(
                    "[Equipment Session Test] Failed to select " +
                    "initial option.");
                return;
            }

            if (!session.HasSelection)
            {
                Debug.LogError(
                    "[Equipment Session Test] Selection state was not set.");
                return;
            }

            Debug.Log(
                $"[Equipment Session Test] Selected initial option: " +
                $"{session.SelectedOption.StatType}");

            ValidateItemUnchanged(
                itemInstance,
                ItemTier.Common,
                5,
                0,
                "after Select");

            if (!session.TryReroll())
            {
                Debug.LogError(
                    "[Equipment Session Test] Reroll failed.");
                return;
            }

            ValidateOptions(
                session.Options,
                progressionConfig.OptionsPerTier,
                "Rerolled");

            LogOptions(
                "Rerolled",
                session.Options);

            if (session.HasSelection)
            {
                Debug.LogError(
                    "[Equipment Session Test] Selection should be " +
                    "cleared after Reroll.");
                return;
            }

            ValidateItemUnchanged(
                itemInstance,
                ItemTier.Common,
                5,
                0,
                "after Reroll");

            EquipmentStatOption rerolledOption =
                session.Options[0];

            if (!session.TrySelect(rerolledOption))
            {
                Debug.LogError(
                    "[Equipment Session Test] Failed to select " +
                    "rerolled option.");
                return;
            }

            if (!session.TryConfirm())
            {
                Debug.LogError(
                    "[Equipment Session Test] Confirm failed.");
                return;
            }

            if (session.IsOpen)
            {
                Debug.LogError(
                    "[Equipment Session Test] Session should be closed " +
                    "after successful Confirm.");
                return;
            }

            if (itemInstance.CurrentTier !=
                ItemTier.Uncommon)
            {
                Debug.LogError(
                    $"[Equipment Session Test] Invalid Tier after Confirm. " +
                    $"Expected=Uncommon, " +
                    $"Actual={itemInstance.CurrentTier}.");
                return;
            }

            if (itemInstance.UpgradeLevel != 0)
            {
                Debug.LogError(
                    $"[Equipment Session Test] Invalid UpgradeLevel " +
                    $"after Confirm. " +
                    $"Expected=0, " +
                    $"Actual={itemInstance.UpgradeLevel}.");
                return;
            }

            if (itemInstance.UnlockedStats.Count != 1)
            {
                Debug.LogError(
                    $"[Equipment Session Test] Invalid UnlockedStats " +
                    $"count after Confirm. " +
                    $"Expected=1, " +
                    $"Actual={itemInstance.UnlockedStats.Count}.");
                return;
            }

            EquipmentRolledStat unlockedStat =
                itemInstance.UnlockedStats[0];

            if (unlockedStat.StatType !=
                rerolledOption.StatType)
            {
                Debug.LogError(
                    "[Equipment Session Test] Confirmed stat does not " +
                    "match selected rerolled option.");
                return;
            }

            if (unlockedStat.ModifierType !=
                rerolledOption.ModifierType)
            {
                Debug.LogError(
                    "[Equipment Session Test] Confirmed ModifierType " +
                    "does not match selected option.");
                return;
            }

            if (!Mathf.Approximately(
                    unlockedStat.RolledValue,
                    rerolledOption.RolledValue))
            {
                Debug.LogError(
                    "[Equipment Session Test] Confirmed RolledValue " +
                    "does not match selected option.");
                return;
            }

            Debug.Log(
                $"[Equipment Session Test] Confirmed option: " +
                $"{unlockedStat.StatType} | " +
                $"{unlockedStat.ModifierType} | " +
                $"{unlockedStat.RolledValue}");

            session.Close();

            Debug.Log(
                "[Equipment Session Test] ✓ COMPLETE — " +
                "Equipment upgrade session correctly separates " +
                "temporary options from permanent state.");
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

        private static void ValidateOptions(
            IReadOnlyList<EquipmentStatOption> options,
            int expectedCount,
            string label)
        {
            if (options == null)
            {
                Debug.LogError(
                    $"[Equipment Session Test] {label} options are null.");
                return;
            }

            if (options.Count != expectedCount)
            {
                Debug.LogError(
                    $"[Equipment Session Test] Invalid {label} option " +
                    $"count. Expected={expectedCount}, " +
                    $"Actual={options.Count}.");
                return;
            }

            Debug.Log(
                $"[Equipment Session Test] {label} option count " +
                $"verified: {options.Count}");
        }

        private static void LogOptions(
            string label,
            IReadOnlyList<EquipmentStatOption> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                EquipmentStatOption option =
                    options[i];

                Debug.Log(
                    $"[Equipment Session Test] {label} Option {i + 1}: " +
                    $"{option.StatType} | " +
                    $"{option.ModifierType} | " +
                    $"RolledValue={option.RolledValue}");
            }
        }

        private static void ValidateItemUnchanged(
            ItemInstance itemInstance,
            ItemTier expectedTier,
            int expectedUpgrade,
            int expectedUnlockedStats,
            string stage)
        {
            if (itemInstance.CurrentTier !=
                expectedTier)
            {
                Debug.LogError(
                    $"[Equipment Session Test] Tier changed {stage}. " +
                    $"Expected={expectedTier}, " +
                    $"Actual={itemInstance.CurrentTier}.");
                return;
            }

            if (itemInstance.UpgradeLevel !=
                expectedUpgrade)
            {
                Debug.LogError(
                    $"[Equipment Session Test] UpgradeLevel changed " +
                    $"{stage}. " +
                    $"Expected={expectedUpgrade}, " +
                    $"Actual={itemInstance.UpgradeLevel}.");
                return;
            }

            if (itemInstance.UnlockedStats.Count !=
                expectedUnlockedStats)
            {
                Debug.LogError(
                    $"[Equipment Session Test] UnlockedStats changed " +
                    $"{stage}. " +
                    $"Expected={expectedUnlockedStats}, " +
                    $"Actual={itemInstance.UnlockedStats.Count}.");
                return;
            }

            Debug.Log(
                $"[Equipment Session Test] ItemInstance unchanged {stage}.");
        }
    }
}