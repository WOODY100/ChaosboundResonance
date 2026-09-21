using System;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentModifierSourceBuilder
    {
        public const string SourceId = "Equipment";

        private readonly ItemContentResolver itemContentResolver;
        private readonly EquipmentStatResolver statResolver;

        public EquipmentModifierSourceBuilder(
            ItemContentResolver itemContentResolver,
            EquipmentStatResolver statResolver)
        {
            if (itemContentResolver == null)
                throw new ArgumentNullException(
                    nameof(itemContentResolver));

            if (statResolver == null)
                throw new ArgumentNullException(
                    nameof(statResolver));

            this.itemContentResolver =
                itemContentResolver;

            this.statResolver =
                statResolver;
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

                if (!itemContentResolver.TryResolve(
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
            for (int i = 0; i < baseData.BaseStats.Count; i++)
            {
                EquipmentBaseStat baseStat =
                    baseData.BaseStats[i];

                UnityEngine.Debug.Log(
                    $"[Equipment Modifier Debug] " +
                    $"Item={baseData.ContentId} | " +
                    $"Stat={baseStat.StatType} | " +
                    $"Modifier={baseStat.ModifierType} | " +
                    $"Value={baseStat.Value}");

                source.Modifiers.Add(new StatModifier
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