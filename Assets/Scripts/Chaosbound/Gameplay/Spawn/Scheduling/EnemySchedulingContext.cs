using Chaosbound.Gameplay.Pressure.Models;
using Chaosbound.Content.Expeditions.Runtime.Enemy;
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
        /// Gets the current pressure snapshot.
        /// </summary>
        public PressureSnapshot Pressure { get; }

        public EnemySchedulingContext(
            SpawnJob job,
            RuntimeEnemyConfig enemyConfig,
            PressureSnapshot pressure)
        {
            Job = job
                ?? throw new ArgumentNullException(nameof(job));

            EnemyConfig = enemyConfig
                ?? throw new ArgumentNullException(nameof(enemyConfig));

            Pressure = pressure
                ?? throw new ArgumentNullException(nameof(pressure));
        }
    }
}