using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Items.Runtime;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    public static class SaveItemInstanceMapper
    {
        public static SaveItemInstanceData ToSaveData(
            ItemInstance itemInstance)
        {
            if (itemInstance == null)
                throw new ArgumentNullException(
                    nameof(itemInstance));

            SaveItemInstanceData data =
                new SaveItemInstanceData();

            data.InstanceId = itemInstance.InstanceId;
            data.BaseDataId = itemInstance.BaseDataId;
            data.CurrentTier = itemInstance.CurrentTier;
            data.UpgradeLevel = itemInstance.UpgradeLevel;

            data.UnlockedStats =
                new List<SaveEquipmentRolledStatData>();

            IReadOnlyList<EquipmentRolledStat> unlockedStats =
                itemInstance.UnlockedStats;

            for (int i = 0; i < unlockedStats.Count; i++)
            {
                EquipmentRolledStat stat = unlockedStats[i];

                data.UnlockedStats.Add(
                    new SaveEquipmentRolledStatData
                    {
                        StatType = stat.StatType,
                        ModifierType = stat.ModifierType,
                        RolledValue = stat.RolledValue
                    });
            }

            data.HasPendingTierUpSession =
                itemInstance.HasValidTierUpSession();

            if (data.HasPendingTierUpSession)
            {
                data.TierUpSessionTier =
                    itemInstance.TierUpSessionTier;

                data.TierUpSessionUpgradeLevel =
                    itemInstance.TierUpSessionUpgradeLevel;

                data.TierUpOptions =
                    new List<SaveEquipmentStatOptionData>();

                IReadOnlyList<EquipmentStatOption> options =
                    itemInstance.TierUpOptions;

                for (int i = 0; i < options.Count; i++)
                {
                    EquipmentStatOption option = options[i];

                    data.TierUpOptions.Add(
                        ToSaveOptionData(option));
                }

                data.HasSelectedTierUpOption =
                    itemInstance.HasSelectedTierUpOption;

                if (data.HasSelectedTierUpOption)
                {
                    data.SelectedTierUpOption =
                        ToSaveOptionData(
                            itemInstance.SelectedTierUpOption);
                }
            }

            return data;
        }

        public static bool TryFromSaveData(
            SaveItemInstanceData data,
            ItemContentResolver contentResolver,
            out ItemInstance itemInstance)
        {
            itemInstance = null;

            if (data == null || contentResolver == null)
                return false;

            if (string.IsNullOrWhiteSpace(data.InstanceId) ||
                string.IsNullOrWhiteSpace(data.BaseDataId))
            {
                return false;
            }

            if (!contentResolver.TryResolve(
                    data.BaseDataId,
                    out ItemBaseData baseData) ||
                baseData == null)
            {
                return false;
            }

            ItemInstance restoredItem;

            try
            {
                restoredItem = new ItemInstance(
                    data.InstanceId,
                    data.BaseDataId,
                    data.CurrentTier,
                    data.UpgradeLevel);
            }
            catch
            {
                return false;
            }

            if (data.UnlockedStats != null)
            {
                for (int i = 0; i < data.UnlockedStats.Count; i++)
                {
                    SaveEquipmentRolledStatData savedStat =
                        data.UnlockedStats[i];

                    if (savedStat == null)
                        return false;

                    EquipmentRolledStat stat =
                        new EquipmentRolledStat(
                            savedStat.StatType,
                            savedStat.ModifierType,
                            savedStat.RolledValue);

                    if (!restoredItem.TryAddUnlockedStat(stat))
                        return false;
                }
            }

            if (!TryRestoreTierUpSession(data, restoredItem))
                return false;

            itemInstance = restoredItem;
            return true;
        }

        private static bool TryRestoreTierUpSession(
            SaveItemInstanceData data,
            ItemInstance itemInstance)
        {
            if (!data.HasPendingTierUpSession)
            {
                return !data.HasSelectedTierUpOption &&
                       (data.TierUpOptions == null ||
                        data.TierUpOptions.Count == 0);
            }

            if (data.TierUpSessionTier != itemInstance.CurrentTier ||
                data.TierUpSessionUpgradeLevel != itemInstance.UpgradeLevel)
            {
                return false;
            }

            if (data.TierUpOptions == null ||
                data.TierUpOptions.Count != 3)
            {
                return false;
            }

            List<EquipmentStatOption> options =
                new List<EquipmentStatOption>(3);

            for (int i = 0; i < data.TierUpOptions.Count; i++)
            {
                SaveEquipmentStatOptionData savedOption =
                    data.TierUpOptions[i];

                if (savedOption == null)
                    return false;

                options.Add(
                    new EquipmentStatOption(
                        savedOption.StatType,
                        savedOption.ModifierType,
                        savedOption.RolledValue));
            }

            if (!itemInstance.TrySetTierUpSession(options))
                return false;

            if (data.HasSelectedTierUpOption)
            {
                if (data.SelectedTierUpOption == null)
                    return false;

                EquipmentStatOption selectedOption =
                    new EquipmentStatOption(
                        data.SelectedTierUpOption.StatType,
                        data.SelectedTierUpOption.ModifierType,
                        data.SelectedTierUpOption.RolledValue);

                if (!itemInstance.TrySelectTierUpOption(selectedOption))
                    return false;
            }

            return itemInstance.HasValidTierUpSession();
        }

        private static SaveEquipmentStatOptionData ToSaveOptionData(
            EquipmentStatOption option)
        {
            return new SaveEquipmentStatOptionData
            {
                StatType = option.StatType,
                ModifierType = option.ModifierType,
                RolledValue = option.RolledValue
            };
        }
    }
}