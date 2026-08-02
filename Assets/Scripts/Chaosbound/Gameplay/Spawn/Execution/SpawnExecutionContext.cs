using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;

namespace Chaosbound.Gameplay.Spawn.Execution
{
    /// <summary>
    /// Represents the execution context used during
    /// SpawnTask execution.
    /// </summary>
    public sealed class SpawnExecutionContext
    {
        /// <summary>
        /// Gets the scheduled task being executed.
        /// </summary>
        public ScheduledSpawnTask ScheduledTask { get; }

        /// <summary>
        /// Gets the runtime state associated with the parent SpawnJob.
        /// </summary>
        public SpawnJobRuntimeState RuntimeState { get; }

        public SpawnExecutionContext(
            ScheduledSpawnTask scheduledTask,
            SpawnJobRuntimeState runtimeState)
        {
            ScheduledTask =
                scheduledTask
                ?? throw new ArgumentNullException(nameof(scheduledTask));

            RuntimeState =
                runtimeState
                ?? throw new ArgumentNullException(nameof(runtimeState));
        }
    }
}