using Chaosbound.Content.Expeditions.Runtime.Enemy;
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

        public SpawnRuntimeValidationContext(
            EnemySchedulingContext schedulingContext,
            RuntimeEnemyConfig enemyConfig,
            PressureSnapshot pressureSnapshot)
        {
            SchedulingContext = schedulingContext;
            EnemyConfig = enemyConfig;
            PressureSnapshot = pressureSnapshot;
        }
    }
}