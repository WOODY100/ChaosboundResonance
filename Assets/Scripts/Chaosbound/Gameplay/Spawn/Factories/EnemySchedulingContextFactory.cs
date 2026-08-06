using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Pressure.Models;
using Chaosbound.Gameplay.Spawn.Domain;
using Chaosbound.Gameplay.Spawn.Scheduling;
using System;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    public sealed class EnemySchedulingContextFactory
    {
        public EnemySchedulingContext Create(
            SpawnJob job,
            RuntimeEnemyConfig enemyConfig,
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references,
            PressureSnapshot pressure,
            ExpeditionRuntimeState expeditionRuntime)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (enemyConfig == null)
                throw new ArgumentNullException(nameof(enemyConfig));

            if (spawnConfig == null)
                throw new ArgumentNullException(nameof(spawnConfig));

            if (references == null)
                throw new ArgumentNullException(nameof(references));

            if (pressure == null)
                throw new ArgumentNullException(nameof(pressure));

            if (expeditionRuntime == null)
                throw new ArgumentNullException(nameof(expeditionRuntime));

            return new EnemySchedulingContext(
                job,
                enemyConfig,
                spawnConfig,
                references,
                pressure,
                expeditionRuntime);
        }
    }
}