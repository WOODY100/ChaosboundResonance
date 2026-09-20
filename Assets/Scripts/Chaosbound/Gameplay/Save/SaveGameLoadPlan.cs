using System;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.MetaProgression.Persistent;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveGameLoadPlan
    {
        public SaveInventoryLoadPlan Inventory { get; }

        public SaveEquipmentLoadPlan Equipment { get; }

        public int MetaExperience { get; }

        public SaveGameLoadPlan(
            SaveInventoryLoadPlan inventory,
            SaveEquipmentLoadPlan equipment,
            int metaExperience)
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
        }
    }
}