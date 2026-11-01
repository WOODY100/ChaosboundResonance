using System;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveGameLoadPlan
    {
        public SaveInventoryLoadPlan Inventory { get; }

        public SaveEquipmentLoadPlan Equipment { get; }

        public int MetaExperience { get; }

        public int MetaLevel { get; }

        public int MetaSkillPoints { get; }

        public SaveSkillTreeLoadPlan SkillTree { get; }

        public ExpeditionProgressData ExpeditionProgress { get; }

        public SaveGameLoadPlan(
            SaveInventoryLoadPlan inventory,
            SaveEquipmentLoadPlan equipment,
            int metaExperience,
            int metaLevel,
            int metaSkillPoints,
            SaveSkillTreeLoadPlan skillTree,
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
                throw new ArgumentOutOfRangeException(
                    nameof(metaExperience));

            if (metaLevel < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(metaLevel));

            if (metaSkillPoints < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(metaSkillPoints));

            MetaExperience = metaExperience;
            MetaLevel = metaLevel;
            MetaSkillPoints = metaSkillPoints;

            SkillTree =
                skillTree ??
                throw new ArgumentNullException(nameof(skillTree));

            ExpeditionProgress =
                expeditionProgress ??
                throw new ArgumentNullException(
                    nameof(expeditionProgress));
        }
    }
}
