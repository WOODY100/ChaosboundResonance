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
        /// Gets the resolved task being executed.
        /// </summary>
        public ResolvedSpawnTask ResolvedTask { get; }

        /// <summary>
        /// Gets the runtime state associated with the parent SpawnJob.
        /// </summary>
        public SpawnJobRuntimeState RuntimeState { get; }

        public SpawnExecutionContext(
            ResolvedSpawnTask resolvedTask,
            SpawnJobRuntimeState runtimeState)
        {
            ResolvedTask =
                resolvedTask
                ?? throw new ArgumentNullException(nameof(resolvedTask));

            RuntimeState =
                runtimeState
                ?? throw new ArgumentNullException(nameof(runtimeState));
        }
    }
}