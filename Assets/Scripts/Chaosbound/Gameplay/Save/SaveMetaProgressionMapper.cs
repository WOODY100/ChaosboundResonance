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
                Experience = state.Experience
            };
        }

        public static void ApplyLoadPlan(
            int experience,
            PersistentMetaState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            if (experience < 0)
                throw new ArgumentOutOfRangeException(nameof(experience));

            state.Clear();
            state.AddExperience(experience);
        }
    }
}