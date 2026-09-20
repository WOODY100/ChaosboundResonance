using System;
using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Equipment;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public sealed class EquipmentTooltipStatBuilder
    {
        private readonly EquipmentStatResolver statResolver;

        public EquipmentTooltipStatBuilder(
            EquipmentStatResolver statResolver)
        {
            if (statResolver == null)
                throw new ArgumentNullException(
                    nameof(statResolver));

            this.statResolver = statResolver;
        }

        public List<EquipmentTooltipStatData> Build(
            ItemBaseData baseData,
            ItemInstance itemInstance)
        {
            List<EquipmentTooltipStatData> result =
                new List<EquipmentTooltipStatData>();

            if (baseData == null)
                return result;

            if (itemInstance == null)
                return result;

            if (baseData.Category != ItemCategory.Equipment)
                return result;

            Array statTypes =
                Enum.GetValues(typeof(StatType));

            for (int i = 0; i < statTypes.Length; i++)
            {
                StatType statType =
                    (StatType)statTypes.GetValue(i);

                if (!statResolver.TryResolve(
                        baseData,
                        itemInstance,
                        statType,
                        out ModifierType modifierType,
                        out float value))
                {
                    continue;
                }

                result.Add(
                    new EquipmentTooltipStatData(
                        statType,
                        modifierType,
                        value));
            }

            return result;
        }
    }
}