using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Result
{
    public sealed class ExpeditionResultData
    {
        public ExpeditionResultStatus Status { get; }

        public float ElapsedTime { get; }

        public int PlayerLevel { get; }

        public int EnemiesDefeated { get; }

        public RuntimeSkill[] Skills { get; }

        public int MetaExperienceReward { get; }

        public string[] RewardItemContentIds { get; }

        public ExpeditionResultData(
            ExpeditionResultStatus status,
            float elapsedTime,
            int playerLevel,
            int enemiesDefeated,
            RuntimeSkill[] skills,
            int metaExperienceReward,
            string[] rewardItemContentIds)
        {
            Status = status;

            ElapsedTime = Mathf.Max(0f, elapsedTime);

            PlayerLevel = Mathf.Max(0, playerLevel);

            EnemiesDefeated = Mathf.Max(0, enemiesDefeated);

            Skills = skills ?? Array.Empty<RuntimeSkill>();

            MetaExperienceReward = Mathf.Max(0, metaExperienceReward);

            RewardItemContentIds =
                rewardItemContentIds ?? Array.Empty<string>();
        }
    }
}