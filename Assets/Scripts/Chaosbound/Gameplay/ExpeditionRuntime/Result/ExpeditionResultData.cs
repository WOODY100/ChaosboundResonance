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


        public ExpeditionResultData(
            ExpeditionResultStatus status,
            float elapsedTime,
            int playerLevel,
            int enemiesDefeated,
            RuntimeSkill[] skills)
        {
            Status = status;

            ElapsedTime =
                Mathf.Max(
                    0f,
                    elapsedTime);

            PlayerLevel =
                Mathf.Max(
                    0,
                    playerLevel);

            EnemiesDefeated =
                Mathf.Max(
                    0,
                    enemiesDefeated);

            Skills =
                skills;
        }
    }
}