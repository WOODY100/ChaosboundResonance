using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Result
{
    public sealed class ExpeditionResultBuilder
    {
        public ExpeditionResultData Build(
            ExpeditionResultStatus status,
            ExpeditionRuntimeState runtimeState,
            PlayerExperienceSystem experience,
            PlayerSkillLoadout loadout,
            RuntimeExpeditionConfig expeditionConfig)
        {
            if (runtimeState == null)
                throw new ArgumentNullException(nameof(runtimeState));

            if (experience == null)
                throw new ArgumentNullException(nameof(experience));

            if (loadout == null)
                throw new ArgumentNullException(nameof(loadout));

            if (expeditionConfig == null)
                throw new ArgumentNullException(nameof(expeditionConfig));

            int metaExperienceReward = 0;
            string[] rewardItemContentIds = Array.Empty<string>();

            if (status == ExpeditionResultStatus.Completed &&
                expeditionConfig.Rewards != null)
            {
                metaExperienceReward =
                    expeditionConfig.Rewards.MetaExperience;

                var configuredIds =
                    expeditionConfig.Rewards.ItemContentIds;

                if (configuredIds != null)
                {
                    var validIds = new System.Collections.Generic.List<string>();

                    for (int i = 0; i < configuredIds.Count; i++)
                    {
                        string contentId = configuredIds[i];

                        if (string.IsNullOrWhiteSpace(contentId))
                            continue;

                        validIds.Add(contentId.Trim());
                    }

                    rewardItemContentIds = validIds.ToArray();
                }
            }

            return new ExpeditionResultData(
                status,
                (float)runtimeState.ElapsedTime.TotalSeconds,
                experience.CurrentLevel,
                runtimeState.Statistics.EnemiesDefeated,
                loadout.GetAllSkills(),
                metaExperienceReward,
                rewardItemContentIds);
        }
    }
}