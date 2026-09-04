using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Statistics
{
    public sealed class ExpeditionRuntimeStatistics
    {
        public int EnemiesDefeated
        {
            get;
            private set;
        }

        public void RegisterEnemyDefeated()
        {
            EnemiesDefeated++;
        }
    }
}