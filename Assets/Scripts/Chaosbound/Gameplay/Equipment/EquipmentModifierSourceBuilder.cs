using System;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentModifierSourceBuilder
    {
        public const string SourceId = "Equipment";

        private readonly ItemDatabase itemDatabase;
        private readonly EquipmentStatResolver statResolver;

        public EquipmentModifierSourceBuilder(
            ItemDatabase itemDatabase,
            EquipmentStatResolver statResolver)
        {
            if (itemDatabase == null)
                throw new ArgumentNullException(nameof(itemDatabase));

            if (statResolver == null)
                throw new ArgumentNullException(nameof(statResolver));

            this.itemDatabase = itemDatabase;
            this.statResolver = statResolver;
        }

        public bool TryBuild(
            EquipmentLoadoutRuntime loadout,
            out ModifierSource source)
        {
            source = new ModifierSource(SourceId);

            if (loadout == null)
                return false;

            foreach (var pair in loadout.EquippedItems)
            {
                ItemInstance itemInstance =
                    pair.Value;

                if (itemInstance == null)
                    continue;

                if (!itemDatabase.TryGet(
                        itemInstance.BaseDataId,
                        out ItemBaseData baseData))
                {
                    source = null;
                    return false;
                }

                AddItemModifiers(
                    source,
                    baseData,
                    itemInstance);
            }

            return true;
        }

        private void AddItemModifiers(
            ModifierSource source,
            ItemBaseData baseData,
            ItemInstance itemInstance)
        {
            for (int i = 0;
                 i < baseData.BaseStats.Count;
                 i++)
            {
                EquipmentBaseStat baseStat =
                    baseData.BaseStats[i];

                source.Modifiers.Add(
                    new StatModifier
                    {
                        StatType = baseStat.StatType,
                        ModifierType = baseStat.ModifierType,
                        Value = baseStat.Value
                    });
            }

            for (int i = 0;
                 i < itemInstance.UnlockedStats.Count;
                 i++)
            {
                EquipmentRolledStat rolledStat =
                    itemInstance.UnlockedStats[i];

                if (!statResolver.TryResolve(
                        baseData,
                        itemInstance,
                        rolledStat.StatType,
                        out ModifierType modifierType,
                        out float value))
                {
                    continue;
                }

                source.Modifiers.Add(
                    new StatModifier
                    {
                        StatType = rolledStat.StatType,
                        ModifierType = modifierType,
                        Value = value
                    });
            }
        }
    }
}