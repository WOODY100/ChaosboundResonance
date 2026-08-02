using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Gameplay.Pressure.Models;
using Chaosbound.Gameplay.Spawn.Domain;
using Chaosbound.Gameplay.Spawn.Runtime;
using Chaosbound.Gameplay.Spawn.Scheduling;
using System;

public sealed class EnemySchedulingContextFactory
{
    public EnemySchedulingContext Create(
        SpawnJob job,
        RuntimeEnemyConfig enemyConfig,
        PressureSnapshot pressure,
        SpawnJobRuntimeState runtimeState)
    {
        if (job == null)
            throw new ArgumentNullException(nameof(job));

        if (enemyConfig == null)
            throw new ArgumentNullException(nameof(enemyConfig));

        if (pressure == null)
            throw new ArgumentNullException(nameof(pressure));

        if (runtimeState == null)
            throw new ArgumentNullException(nameof(runtimeState));

        return new EnemySchedulingContext(
            job,
            enemyConfig,
            pressure);
    }
}