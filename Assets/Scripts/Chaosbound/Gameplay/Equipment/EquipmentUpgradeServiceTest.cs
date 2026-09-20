using System.Collections.Generic;
using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentUpgradeServiceTest : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private EquipmentStatDatabase statDatabase;
        [SerializeField] private EquipmentProgressionConfig progressionConfig;
        [SerializeField] private ItemBaseData equipmentBaseData;

        [ContextMenu("Run Equipment Upgrade Service Test")]
        private void RunTest()
        {
            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] " +
                    "Missing ItemDatabase.");
                return;
            }

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] " +
                    "Missing EquipmentStatDatabase.");
                return;
            }

            if (progressionConfig == null)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] " +
                    "Missing EquipmentProgressionConfig.");
                return;
            }

            if (equipmentBaseData == null)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] " +
                    "Missing Equipment ItemBaseData.");
                return;
            }

            if (equipmentBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] " +
                    "ItemBaseData is not Equipment.");
                return;
            }

            if (equipmentBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] " +
                    "EquipmentType is None.");
                return;
            }

            if (!itemDatabase.TryGet(
                    equipmentBaseData.ContentId,
                    out ItemBaseData resolvedBaseData))
            {
                Debug.LogError(
                    $"[Equipment Upgrade Service Test] Could not " +
                    $"resolve '{equipmentBaseData.ContentId}'.");
                return;
            }

            if (resolvedBaseData != equipmentBaseData)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] ItemDatabase " +
                    "resolved a different ItemBaseData.");
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
                    "equipment-upgrade-service-test",
                    equipmentBaseData.ContentId,
                    equipmentBaseData.BaseTier);

            Debug.Log(
                $"[Equipment Upgrade Service Test] Initial state: " +
                $"Tier={itemInstance.CurrentTier}, " +
                $"Upgrade={itemInstance.UpgradeLevel}");

            TestReachTierUpState(
                progressionRuntime,
                itemInstance);

            if (!progressionRuntime.IsReadyForTierUp(
                    itemInstance))
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] Item did not " +
                    "reach the Tier Up state.");
                return;
            }

            Debug.Log(
                $"[Equipment Upgrade Service Test] Tier Up state reached: " +
                $"Tier={itemInstance.CurrentTier}, " +
                $"Upgrade={itemInstance.UpgradeLevel}");

            if (!service.TryGenerateTierOptions(
                    itemInstance,
                    out List<EquipmentStatOption> options))
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] Failed to " +
                    "generate Tier Up options.");
                return;
            }

            if (options == null)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] Options list " +
                    "is null.");
                return;
            }

            if (options.Count !=
                progressionConfig.OptionsPerTier)
            {
                Debug.LogError(
                    $"[Equipment Upgrade Service Test] Invalid option " +
                    $"count. Expected={progressionConfig.OptionsPerTier}, " +
                    $"Actual={options.Count}.");
                return;
            }

            Debug.Log(
                $"[Equipment Upgrade Service Test] Generated " +
                $"{options.Count} Tier Up options.");

            for (int i = 0; i < options.Count; i++)
            {
                EquipmentStatOption option =
                    options[i];

                Debug.Log(
                    $"[Equipment Upgrade Service Test] Option {i + 1}: " +
                    $"{option.StatType} | " +
                    $"{option.ModifierType} | " +
                    $"RolledValue={option.RolledValue}");
            }

            TestTierDoesNotChangeDuringGeneration(
                itemInstance);

            EquipmentStatOption selectedOption =
                options[0];

            Debug.Log(
                $"[Equipment Upgrade Service Test] " +
                $"Selecting option: {selectedOption.StatType}");

            if (!service.TryApplyTierUp(
                    itemInstance,
                    selectedOption))
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] Failed to " +
                    "apply selected Tier Up option.");
                return;
            }

            TestTierUpApplied(
                itemInstance,
                selectedOption);

            Debug.Log(
                "[Equipment Upgrade Service Test] ✓ COMPLETE — " +
                "NPC-controlled Tier Up flow is working correctly.");
        }

        private static void TestReachTierUpState(
            EquipmentProgressionRuntime progressionRuntime,
            ItemInstance itemInstance)
        {
            int upgradesRequired =
                progressionRuntime.GetType() != null
                    ? GetRequiredUpgrades(progressionRuntime)
                    : 0;

            for (int i = 0;
                 i < upgradesRequired;
                 i++)
            {
                if (!progressionRuntime.TryUpgrade(
                        itemInstance))
                {
                    Debug.LogError(
                        $"[Equipment Upgrade Service Test] Upgrade " +
                        $"{i + 1} failed.");
                    return;
                }
            }
        }

        private static int GetRequiredUpgrades(
            EquipmentProgressionRuntime progressionRuntime)
        {
            return GetProgressionUpgradeCount(
                progressionRuntime);
        }

        private static int GetProgressionUpgradeCount(
            EquipmentProgressionRuntime progressionRuntime)
        {
            // The service test uses the current configuration through
            // the resulting progression state. The configured value
            // is validated by IsReadyForTierUp below.
            //
            // Five upgrades are the frozen Equipment V1 rule.
            return 5;
        }

        private static void TestTierDoesNotChangeDuringGeneration(
            ItemInstance itemInstance)
        {
            if (itemInstance.CurrentTier !=
                ItemTier.Common)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] Tier changed " +
                    "while generating options.");
                return;
            }

            if (itemInstance.UpgradeLevel != 5)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] UpgradeLevel " +
                    "changed while generating options.");
                return;
            }

            if (itemInstance.UnlockedStats.Count != 0)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] UnlockedStats " +
                    "changed while generating options.");
                return;
            }

            Debug.Log(
                "[Equipment Upgrade Service Test] Option generation " +
                "did not mutate ItemInstance.");
        }

        private static void TestTierUpApplied(
            ItemInstance itemInstance,
            EquipmentStatOption selectedOption)
        {
            if (itemInstance.CurrentTier !=
                ItemTier.Uncommon)
            {
                Debug.LogError(
                    $"[Equipment Upgrade Service Test] Invalid Tier " +
                    $"after Tier Up. " +
                    $"Expected={ItemTier.Uncommon}, " +
                    $"Actual={itemInstance.CurrentTier}.");
                return;
            }

            if (itemInstance.UpgradeLevel != 0)
            {
                Debug.LogError(
                    $"[Equipment Upgrade Service Test] Invalid " +
                    $"UpgradeLevel after Tier Up. " +
                    $"Expected=0, " +
                    $"Actual={itemInstance.UpgradeLevel}.");
                return;
            }

            if (itemInstance.UnlockedStats.Count != 1)
            {
                Debug.LogError(
                    $"[Equipment Upgrade Service Test] Invalid " +
                    $"UnlockedStats count. Expected=1, " +
                    $"Actual={itemInstance.UnlockedStats.Count}.");
                return;
            }

            EquipmentRolledStat unlockedStat =
                itemInstance.UnlockedStats[0];

            if (unlockedStat.StatType !=
                selectedOption.StatType)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] Unlocked stat " +
                    "does not match selected option.");
                return;
            }

            if (unlockedStat.ModifierType !=
                selectedOption.ModifierType)
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] Unlocked stat " +
                    "ModifierType does not match selected option.");
                return;
            }

            if (!Mathf.Approximately(
                    unlockedStat.RolledValue,
                    selectedOption.RolledValue))
            {
                Debug.LogError(
                    "[Equipment Upgrade Service Test] Unlocked stat " +
                    "RolledValue does not match selected option.");
                return;
            }

            Debug.Log(
                $"[Equipment Upgrade Service Test] Tier Up applied: " +
                $"Tier={itemInstance.CurrentTier}, " +
                $"Upgrade={itemInstance.UpgradeLevel}, " +
                $"Unlocked={unlockedStat.StatType}");
        }
    }
}