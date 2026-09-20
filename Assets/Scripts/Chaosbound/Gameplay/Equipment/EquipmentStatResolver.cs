using System;
using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentStatResolver
    {
        private readonly EquipmentStatDatabase statDatabase;

        public EquipmentStatResolver(
            EquipmentStatDatabase statDatabase)
        {
            if (statDatabase == null)
                throw new ArgumentNullException(nameof(statDatabase));

            this.statDatabase = statDatabase;
        }

        public bool TryResolve(
            ItemBaseData baseData,
            ItemInstance itemInstance,
            StatType statType,
            out ModifierType modifierType,
            out float value)
        {
            modifierType = default;
            value = 0f;

            if (baseData == null)
                return false;

            if (itemInstance == null)
                return false;

            if (TryResolveBaseStat(
                    baseData,
                    statType,
                    out modifierType,
                    out value))
            {
                return true;
            }

            return TryResolveUnlockedStat(
                itemInstance,
                statType,
                out modifierType,
                out value);
        }

        private bool TryResolveBaseStat(
            ItemBaseData baseData,
            StatType statType,
            out ModifierType modifierType,
            out float value)
        {
            modifierType = default;
            value = 0f;

            IReadOnlyList<EquipmentBaseStat> baseStats =
                baseData.BaseStats;

            if (baseStats == null)
                return false;

            for (int i = 0; i < baseStats.Count; i++)
            {
                EquipmentBaseStat stat = baseStats[i];

                if (stat.StatType != statType)
                    continue;

                modifierType = stat.ModifierType;
                value = stat.Value;

                return true;
            }

            return false;
        }

        private bool TryResolveUnlockedStat(
            ItemInstance itemInstance,
            StatType statType,
            out ModifierType modifierType,
            out float value)
        {
            modifierType = default;
            value = 0f;

            IReadOnlyList<EquipmentRolledStat> unlockedStats =
                itemInstance.UnlockedStats;

            if (unlockedStats == null)
                return false;

            for (int i = 0; i < unlockedStats.Count; i++)
            {
                EquipmentRolledStat stat =
                    unlockedStats[i];

                if (stat.StatType != statType)
                    continue;

                if (!statDatabase.TryGetDefinition(
                        stat.StatType,
                        out EquipmentStatDefinition definition))
                {
                    return false;
                }

                modifierType = stat.ModifierType;

                value =
                    stat.RolledValue +
                    (definition.UpgradeGrowth *
                     itemInstance.UpgradeLevel);

                return true;
            }

            return false;
        }
    }
}