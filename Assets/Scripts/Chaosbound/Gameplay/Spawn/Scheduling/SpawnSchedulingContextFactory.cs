using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Spawn.Domain;
using Chaosbound.Gameplay.Spawn.Scheduling;
using System;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates immutable contexts required by
    /// Spawn Scheduling.
    /// </summary>
    public sealed class SpawnSchedulingContextFactory
    {
        public SpawnSchedulingContext Create(
            SpawnJob job,
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references,
            ExpeditionRuntimeState expeditionRuntime)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (spawnConfig == null)
                throw new ArgumentNullException(nameof(spawnConfig));

            if (references == null)
                throw new ArgumentNullException(nameof(references));

            if (expeditionRuntime == null)
                throw new ArgumentNullException(
                    nameof(expeditionRuntime));

            return new SpawnSchedulingContext(
                job,
                spawnConfig,
                references,
                expeditionRuntime);
        }
    }
}