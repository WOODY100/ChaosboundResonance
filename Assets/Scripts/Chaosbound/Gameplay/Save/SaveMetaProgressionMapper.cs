using System;
using Chaosbound.Gameplay.MetaProgression.Persistent;

namespace Chaosbound.Gameplay.Save
{
    public static class SaveMetaProgressionMapper
    {
        public static SaveMetaProgressionData ToSaveData(
            PersistentMetaState state)
        {
            if (state == null)
                throw new ArgumentNullException(
                    nameof(state));

            return new SaveMetaProgressionData
            {
                Experience = state.Experience,
                MetaLevel = state.MetaLevel,
                SkillPoints = state.SkillPoints
            };
        }

        public static void ApplyLoadPlan(
            int experience,
            int metaLevel,
            int skillPoints,
            PersistentMetaRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));

            runtime.RestoreProgression(
                experience,
                metaLevel,
                skillPoints);
        }
    }
}
