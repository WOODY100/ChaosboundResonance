using System;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveGameLoadPlan
    {
        public SaveInventoryLoadPlan Inventory { get; }

        public SaveEquipmentLoadPlan Equipment { get; }

        public int MetaExperience { get; }

        public ExpeditionProgressData ExpeditionProgress { get; }

        public SaveGameLoadPlan(
            SaveInventoryLoadPlan inventory,
            SaveEquipmentLoadPlan equipment,
            int metaExperience,
            ExpeditionProgressData expeditionProgress)
        {
            Inventory =
                inventory ??
                throw new ArgumentNullException(
                    nameof(inventory));

            Equipment =
                equipment ??
                throw new ArgumentNullException(
                    nameof(equipment));

            if (metaExperience < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(metaExperience));
            }

            MetaExperience =
                metaExperience;

            ExpeditionProgress =
                expeditionProgress ??
                throw new ArgumentNullException(
                    nameof(expeditionProgress));
        }
    }
}
