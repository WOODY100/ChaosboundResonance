using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Spawn.Domain;
using System;

namespace Chaosbound.Gameplay.Spawn.Scheduling
{
    /// <summary>
    /// Represents the immutable context required by
    /// Spawn Scheduling to generate scheduled spawn tasks.
    /// </summary>
    public sealed class SpawnSchedulingContext
    {
        /// <summary>
        /// Gets the SpawnJob to schedule.
        /// </summary>
        public SpawnJob Job { get; }

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

        public SpawnSchedulingContext(
            SpawnJob job,
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references,
            ExpeditionRuntimeState expeditionRuntime)
        {
            Job =
                job
                ?? throw new ArgumentNullException(nameof(job));

            SpawnConfig =
                spawnConfig
                ?? throw new ArgumentNullException(nameof(spawnConfig));

            References =
                references
                ?? throw new ArgumentNullException(nameof(references));

            ExpeditionRuntime =
                expeditionRuntime
                ?? throw new ArgumentNullException(
                    nameof(expeditionRuntime));
        }
    }
}