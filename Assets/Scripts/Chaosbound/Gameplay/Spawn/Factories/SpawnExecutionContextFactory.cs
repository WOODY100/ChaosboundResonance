using System;
using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.Runtime;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates SpawnExecutionContext instances.
    /// </summary>
    public sealed class SpawnExecutionContextFactory
    {
        /// <summary>
        /// Creates an execution context for the supplied task.
        /// </summary>
        public SpawnExecutionContext Create(
            ScheduledSpawnTask scheduledTask,
            SpawnJobRuntimeState runtimeState)
        {
            if (scheduledTask == null)
                throw new ArgumentNullException(nameof(scheduledTask));

            if (runtimeState == null)
                throw new ArgumentNullException(nameof(runtimeState));

            return new SpawnExecutionContext(
                scheduledTask,
                runtimeState);
        }
    }
}