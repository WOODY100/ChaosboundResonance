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

            data.InstanceId =
                itemInstance.InstanceId;

            data.BaseDataId =
                itemInstance.BaseDataId;

            data.CurrentTier =
                itemInstance.CurrentTier;

            data.UpgradeLevel =
                itemInstance.UpgradeLevel;

            data.UnlockedStats =
                new List<SaveEquipmentRolledStatData>();

            IReadOnlyList<EquipmentRolledStat>
                unlockedStats =
                    itemInstance.UnlockedStats;

            for (int i = 0;
                 i < unlockedStats.Count;
                 i++)
            {
                EquipmentRolledStat stat =
                    unlockedStats[i];

                data.UnlockedStats.Add(
                    new SaveEquipmentRolledStatData
                    {
                        StatType = stat.StatType,
                        ModifierType = stat.ModifierType,
                        RolledValue = stat.RolledValue
                    });
            }

            return data;
        }

        public static bool TryFromSaveData(
            SaveItemInstanceData data,
            ItemContentResolver contentResolver,
            out ItemInstance itemInstance)
        {
            itemInstance = null;

            if (data == null)
                return false;

            if (contentResolver == null)
                return false;

            if (string.IsNullOrWhiteSpace(data.InstanceId))
                return false;

            if (string.IsNullOrWhiteSpace(data.BaseDataId))
                return false;

            if (!contentResolver.TryResolve(
                data.BaseDataId,
                out ItemBaseData baseData))
            {
                return false;
            }

            if (baseData == null)
                return false;

            ItemInstance restoredItem;

            try
            {
                restoredItem =
                    new ItemInstance(
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
                for (int i = 0;
                     i < data.UnlockedStats.Count;
                     i++)
                {
                    SaveEquipmentRolledStatData
                        savedStat =
                            data.UnlockedStats[i];

                    EquipmentRolledStat stat =
                        new EquipmentRolledStat(
                            savedStat.StatType,
                            savedStat.ModifierType,
                            savedStat.RolledValue);

                    if (!restoredItem.TryAddUnlockedStat(stat))
                    {
                        return false;
                    }
                }
            }

            itemInstance = restoredItem;
            return true;
        }
    }
}