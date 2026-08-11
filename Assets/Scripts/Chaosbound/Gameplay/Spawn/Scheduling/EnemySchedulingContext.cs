using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Spawn.Domain;
using System;

namespace Chaosbound.Gameplay.Spawn.Scheduling
{
    /// <summary>
    /// Represents the immutable context required by an enemy
    /// scheduling policy to generate scheduled spawn tasks.
    /// </summary>
    public sealed class EnemySchedulingContext
    {
        /// <summary>
        /// Gets the SpawnJob to schedule.
        /// </summary>
        public SpawnJob Job { get; }

        /// <summary>
        /// Gets the runtime enemy configuration.
        /// </summary>
        public RuntimeEnemyConfig EnemyConfig { get; }

        /// <summary>
        /// Gets the runtime spawn configuration.
        /// </summary>
        public RuntimeSpawnConfig SpawnConfig { get; }

        /// <summary>
        /// Gets the runtime world references.
        /// </summary>
        public RuntimeReferencesConfig References { get; }

        /// <summary>
        /// Gets the current expedition runtime state.
        /// </summary>
        public ExpeditionRuntimeState ExpeditionRuntime { get; }

        public EnemySchedulingContext(
            SpawnJob job,
            RuntimeEnemyConfig enemyConfig,
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references,
            ExpeditionRuntimeState expeditionRuntime)
        {
            Job = job
                ?? throw new ArgumentNullException(nameof(job));

            EnemyConfig = enemyConfig
                ?? throw new ArgumentNullException(nameof(enemyConfig));

            SpawnConfig = spawnConfig
                ?? throw new ArgumentNullException(nameof(spawnConfig));
            
            References = references
                ?? throw new ArgumentNullException(nameof(references));

            ExpeditionRuntime = expeditionRuntime
                ?? throw new ArgumentNullException(nameof(expeditionRuntime));
        }
    }
}