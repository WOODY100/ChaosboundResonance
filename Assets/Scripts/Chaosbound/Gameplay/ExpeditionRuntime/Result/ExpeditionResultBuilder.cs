using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Result
{
    public sealed class ExpeditionResultBuilder
    {
        public ExpeditionResultData Build(
            ExpeditionResultStatus status,
            ExpeditionRuntimeState runtimeState,
            PlayerExperienceSystem experience,
            PlayerSkillLoadout loadout)
        {
            if (runtimeState == null)
                throw new System.ArgumentNullException(
                    nameof(runtimeState));

            if (experience == null)
                throw new System.ArgumentNullException(
                    nameof(experience));

            if (loadout == null)
                throw new System.ArgumentNullException(
                    nameof(loadout));

            return new ExpeditionResultData(
                status,
                (float)runtimeState.ElapsedTime.TotalSeconds,
                experience.CurrentLevel,
                runtimeState.Statistics.EnemiesDefeated,
                loadout.GetAllSkills());
        }
    }
}