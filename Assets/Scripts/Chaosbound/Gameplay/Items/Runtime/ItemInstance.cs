using System;
using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Equipment;

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
            return true;
        }

        public bool TryAdvanceTier()
        {
            if (CurrentTier == ItemTier.Legendary)
                return false;

            CurrentTier++;
            UpgradeLevel = 0;

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
    }
}