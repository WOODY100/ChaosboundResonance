using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Pressure.Models;
using Chaosbound.Gameplay.Spawn.Scheduling;

namespace Chaosbound.Gameplay.Spawn.Validation
{
    /// <summary>
    /// Represents a fully constructed Spawn Runtime
    /// validation environment.
    /// </summary>
    public sealed class SpawnRuntimeValidationContext
    {
        public EnemySchedulingContext SchedulingContext { get; }

        public RuntimeEnemyConfig EnemyConfig { get; }

        public PressureSnapshot PressureSnapshot { get; }

        public RuntimeSpawnConfig SpawnConfig { get; }

        public RuntimeReferencesConfig References { get; }

        public SpawnRuntimeValidationContext(
            EnemySchedulingContext schedulingContext,
            RuntimeEnemyConfig enemyConfig,
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references,
            PressureSnapshot pressureSnapshot)
        {
            SchedulingContext = schedulingContext;
            EnemyConfig = enemyConfig;
            SpawnConfig = spawnConfig;
            References = references;
            PressureSnapshot = pressureSnapshot;
        }
    }
}