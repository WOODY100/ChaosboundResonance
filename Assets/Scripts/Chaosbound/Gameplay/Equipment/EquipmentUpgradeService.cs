using System;
using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentUpgradeService
    {
        private readonly ItemDatabase itemDatabase;
        private readonly EquipmentProgressionRuntime progressionRuntime;

        public EquipmentUpgradeService(
            ItemDatabase itemDatabase,
            EquipmentProgressionRuntime progressionRuntime)
        {
            if (itemDatabase == null)
                throw new ArgumentNullException(
                    nameof(itemDatabase));

            if (progressionRuntime == null)
                throw new ArgumentNullException(
                    nameof(progressionRuntime));

            this.itemDatabase =
                itemDatabase;

            this.progressionRuntime =
                progressionRuntime;
        }

        public bool TryGenerateTierOptions(
            ItemInstance itemInstance,
            out List<EquipmentStatOption> options)
        {
            options =
                new List<EquipmentStatOption>();

            if (itemInstance == null)
                return false;

            if (!itemDatabase.TryGet(
                    itemInstance.BaseDataId,
                    out ItemBaseData baseData))
            {
                return false;
            }

            if (baseData.Category !=
                ItemCategory.Equipment)
            {
                return false;
            }

            return progressionRuntime.TryGenerateTierOptions(
                baseData,
                itemInstance,
                out options);
        }

        public bool TryApplyTierUp(
            ItemInstance itemInstance,
            EquipmentStatOption selectedOption)
        {
            if (itemInstance == null)
                return false;

            if (!itemDatabase.TryGet(
                    itemInstance.BaseDataId,
                    out ItemBaseData baseData))
            {
                return false;
            }

            if (baseData.Category !=
                ItemCategory.Equipment)
            {
                return false;
            }

            return progressionRuntime.TryApplyTierUp(
                itemInstance,
                selectedOption);
        }

        public bool TryRerollTierOptions(
            ItemInstance itemInstance,
            out List<EquipmentStatOption> options)
        {
            options =
                new List<EquipmentStatOption>();

            if (itemInstance == null)
                return false;

            if (!itemDatabase.TryGet(
                    itemInstance.BaseDataId,
                    out ItemBaseData baseData))
            {
                return false;
            }

            if (baseData.Category !=
                ItemCategory.Equipment)
            {
                return false;
            }

            return progressionRuntime.TryGenerateTierOptions(
                baseData,
                itemInstance,
                out options);
        }
    }
}