using Chaosbound.Content.Expeditions.Enums.Enemy;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Enemy
{
    /// <summary>
    /// Immutable runtime configuration containing every enemy
    /// available for the current expedition.
    /// </summary>
    public sealed class RuntimeEnemyConfig
    {
        /// <summary>
        /// Gets every enemy available for the expedition.
        /// </summary>
        public IReadOnlyList<EnemyVariantData> Enemies { get; }

        /// <summary>
        /// Gets the scheduling strategy used by the Enemy Scheduler.
        /// </summary>
        public EnemySchedulingPolicy SchedulingPolicy { get; }

        public RuntimeEnemyConfig(
            IReadOnlyList<EnemyVariantData> enemies, 
            EnemySchedulingPolicy schedulingPolicy)
        {
            Enemies = new List<EnemyVariantData>(enemies);

            SchedulingPolicy = schedulingPolicy;
        }
    }
}