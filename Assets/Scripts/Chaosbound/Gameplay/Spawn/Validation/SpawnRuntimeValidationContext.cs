using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Spawn.Scheduling;

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

        public SpawnRuntimeValidationContext(
            SpawnSchedulingContext schedulingContext,
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references)
        {
            SchedulingContext = schedulingContext;
            SpawnConfig = spawnConfig;
            References = references;
        }
    }
}