using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Equipment;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Items.Runtime
{
    [Serializable]
    public sealed class ItemInstance
    {
        private readonly List<EquipmentRolledStat> unlockedStats =
            new List<EquipmentRolledStat>();

        public string InstanceId { get; }

        public string BaseDataId { get; }

        public ItemTier CurrentTier { get; private set; }

        public int UpgradeLevel { get; private set; }

        public IReadOnlyList<EquipmentRolledStat> UnlockedStats =>
            unlockedStats;

        private readonly List<EquipmentStatOption> tierUpOptions =
            new List<EquipmentStatOption>();

        private bool hasPendingTierUpSession;
        private ItemTier tierUpSessionTier;
        private int tierUpSessionUpgradeLevel;

        private bool hasSelectedTierUpOption;
        private EquipmentStatOption selectedTierUpOption;

        public bool HasPendingTierUpSession =>
            hasPendingTierUpSession;

        public ItemTier TierUpSessionTier =>
            tierUpSessionTier;

        public int TierUpSessionUpgradeLevel =>
            tierUpSessionUpgradeLevel;

        public IReadOnlyList<EquipmentStatOption> TierUpOptions =>
            tierUpOptions;

        public bool HasSelectedTierUpOption =>
            hasSelectedTierUpOption;

        public EquipmentStatOption SelectedTierUpOption =>
            selectedTierUpOption;

        public ItemInstance(
            string instanceId,
            string baseDataId,
            ItemTier currentTier,
            int upgradeLevel = 0)
        {
            if (string.IsNullOrWhiteSpace(instanceId))
                throw new ArgumentException(
                    "InstanceId cannot be null, empty, or whitespace.",
                    nameof(instanceId));

            if (string.IsNullOrWhiteSpace(baseDataId))
                throw new ArgumentException(
                    "BaseDataId cannot be null, empty, or whitespace.",
                    nameof(baseDataId));

            if (upgradeLevel < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(upgradeLevel));

            InstanceId = instanceId.Trim();
            BaseDataId = baseDataId.Trim();
            CurrentTier = currentTier;
            UpgradeLevel = upgradeLevel;
        }

        public bool TryIncreaseUpgradeLevel()
        {
            if (UpgradeLevel == int.MaxValue)
                return false;

            UpgradeLevel++;

            ClearTierUpSession();

            return true;
        }

        public bool TryAdvanceTier()
        {
            if (CurrentTier == ItemTier.Legendary)
                return false;

            CurrentTier++;
            UpgradeLevel = 0;

            ClearTierUpSession();

            return true;
        }

        public bool TryAddUnlockedStat(
            EquipmentRolledStat stat)
        {
            if (HasUnlockedStat(stat.StatType))
                return false;

            unlockedStats.Add(stat);
            return true;
        }

        public bool TryApplyTierUp(EquipmentRolledStat stat)
        {
            if (CurrentTier == ItemTier.Legendary)
                return false;

            if (HasUnlockedStat(stat.StatType))
                return false;

            unlockedStats.Add(stat);

            CurrentTier++;
            UpgradeLevel = 0;

            ClearTierUpSession();

            return true;
        }

        public bool HasUnlockedStat(
            StatType statType)
        {
            for (int i = 0; i < unlockedStats.Count; i++)
            {
                if (unlockedStats[i].StatType == statType)
                    return true;
            }

            return false;
        }

        public bool TrySetTierUpSession(
            IReadOnlyList<EquipmentStatOption> options)
        {
            if (options == null || options.Count != 3)
                return false;

            if (CurrentTier == ItemTier.Legendary)
                return false;

            if (tierUpOptions.Count > 0)
                return false;

            for (int i = 0; i < options.Count; i++)
            {
                if (HasUnlockedStat(options[i].StatType))
                    return false;

                for (int j = 0; j < i; j++)
                {
                    if (options[j].StatType == options[i].StatType)
                        return false;
                }
            }

            tierUpOptions.Clear();

            for (int i = 0; i < options.Count; i++)
                tierUpOptions.Add(options[i]);

            tierUpSessionTier = CurrentTier;
            tierUpSessionUpgradeLevel = UpgradeLevel;

            hasPendingTierUpSession = true;

            hasSelectedTierUpOption = false;
            selectedTierUpOption = default;

            return true;
        }

        public bool TryReplaceTierUpOptions(
            IReadOnlyList<EquipmentStatOption> options)
        {
            // A valid session must already exist.
            if (!HasValidTierUpSession())
                return false;

            if (options == null || options.Count != 3)
                return false;

            // Validate every option before modifying existing data.
            for (int i = 0; i < options.Count; i++)
            {
                EquipmentStatOption option = options[i];

                // Base or previously unlocked stats cannot appear.
                if (HasUnlockedStat(option.StatType))
                    return false;

                // The three options must be different.
                for (int j = 0; j < i; j++)
                {
                    if (options[j].StatType == option.StatType)
                        return false;
                }
            }

            // Replace the previous options only after validation.
            tierUpOptions.Clear();

            for (int i = 0; i < options.Count; i++)
                tierUpOptions.Add(options[i]);

            // A previous selection is no longer valid.
            hasSelectedTierUpOption = false;
            selectedTierUpOption = default;

            return true;
        }

        public bool TrySelectTierUpOption(
            EquipmentStatOption option)
        {
            if (!HasValidTierUpSession())
                return false;

            if (HasUnlockedStat(option.StatType))
                return false;

            bool available = false;

            for (int i = 0; i < tierUpOptions.Count; i++)
            {
                EquipmentStatOption availableOption =
                    tierUpOptions[i];

                if (availableOption.StatType == option.StatType &&
                    availableOption.ModifierType == option.ModifierType &&
                    Mathf.Approximately(
                        availableOption.RolledValue,
                        option.RolledValue))
                {
                    selectedTierUpOption = availableOption;
                    hasSelectedTierUpOption = true;
                    available = true;
                    break;
                }
            }

            return available;
        }

        public bool HasValidTierUpSession()
        {
            if (!hasPendingTierUpSession)
                return false;

            if (tierUpOptions.Count != 3)
                return false;

            if (CurrentTier != tierUpSessionTier)
                return false;

            if (UpgradeLevel != tierUpSessionUpgradeLevel)
                return false;

            if (CurrentTier == ItemTier.Legendary)
                return false;

            return true;
        }

        public void ClearTierUpSession()
        {
            tierUpOptions.Clear();

            hasPendingTierUpSession = false;

            tierUpSessionTier = ItemTier.Common;
            tierUpSessionUpgradeLevel = 0;

            hasSelectedTierUpOption = false;
            selectedTierUpOption = default;
        }
    }
}