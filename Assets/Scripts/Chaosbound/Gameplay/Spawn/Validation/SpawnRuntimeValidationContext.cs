using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Spawn.Scheduling;
using System;

namespace Chaosbound.Gameplay.Spawn.Validation
{
    /// <summary>
    /// Represents a fully constructed Spawn Runtime
    /// validation environment.
    /// </summary>
    public sealed class SpawnRuntimeValidationContext
    {
        public SpawnSchedulingContext SchedulingContext { get; }

        public RuntimeSpawnConfig SpawnConfig { get; }

        public RuntimeReferencesConfig References { get; }

        public ExpeditionRuntimeState ExpeditionRuntime { get; }

        public SpawnRuntimeValidationContext(
            SpawnSchedulingContext schedulingContext,
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references,
            ExpeditionRuntimeState expeditionRuntime)
        {
            SchedulingContext =
                schedulingContext
                ?? throw new ArgumentNullException(nameof(schedulingContext));

            SpawnConfig =
                spawnConfig
                ?? throw new ArgumentNullException(nameof(spawnConfig));

            References =
                references
                ?? throw new ArgumentNullException(nameof(references));

            ExpeditionRuntime =
                expeditionRuntime
                ?? throw new ArgumentNullException(nameof(expeditionRuntime));
        }
    }
}