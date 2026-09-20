using System;
using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentProgressionRuntime
    {
        private readonly EquipmentProgressionConfig progressionConfig;
        private readonly EquipmentTierOptionGenerator optionGenerator;

        public event Action<ItemInstance> ProgressionChanged;

        public EquipmentProgressionRuntime(
            EquipmentProgressionConfig progressionConfig,
            EquipmentTierOptionGenerator optionGenerator)
        {
            if (progressionConfig == null)
                throw new ArgumentNullException(nameof(progressionConfig));

            if (optionGenerator == null)
                throw new ArgumentNullException(nameof(optionGenerator));

            this.progressionConfig = progressionConfig;
            this.optionGenerator = optionGenerator;
        }

        public bool CanUpgrade(ItemInstance itemInstance)
        {
            if (itemInstance == null)
                return false;

            return itemInstance.UpgradeLevel <
                   progressionConfig.UpgradesPerTier;
        }

        public bool IsReadyForTierUp(ItemInstance itemInstance)
        {
            if (itemInstance == null)
                return false;

            if (itemInstance.CurrentTier == ItemTier.Legendary)
                return false;

            return itemInstance.UpgradeLevel >=
                   progressionConfig.UpgradesPerTier;
        }

        public bool TryUpgrade(ItemInstance itemInstance)
        {
            if (!CanUpgrade(itemInstance))
                return false;

            if (!itemInstance.TryIncreaseUpgradeLevel())
                return false;

            ProgressionChanged?.Invoke(itemInstance);

            return true;
        }

        public bool TryGenerateTierOptions(
            ItemBaseData baseData,
            ItemInstance itemInstance,
            out List<EquipmentStatOption> options)
        {
            options = new List<EquipmentStatOption>();

            if (!IsReadyForTierUp(itemInstance))
                return false;

            return optionGenerator.TryGenerate(
                baseData,
                itemInstance,
                out options);
        }

        public bool TryApplyTierUp(
            ItemInstance itemInstance,
            EquipmentStatOption selectedOption)
        {
            if (!IsReadyForTierUp(itemInstance))
                return false;

            if (itemInstance.HasUnlockedStat(
                    selectedOption.StatType))
            {
                return false;
            }

            EquipmentRolledStat rolledStat =
                selectedOption.ToRolledStat();

            if (!itemInstance.TryAddUnlockedStat(rolledStat))
                return false;

            if (!itemInstance.TryAdvanceTier())
                return false;

            ProgressionChanged?.Invoke(itemInstance);

            return true;
        }
    }
}