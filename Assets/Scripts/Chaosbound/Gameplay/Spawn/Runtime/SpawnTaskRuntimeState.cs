using System;
using Chaosbound.Gameplay.Spawn.Domain;

namespace Chaosbound.Gameplay.Spawn.Runtime
{
    /// <summary>
    /// Represents the runtime execution state
    /// associated with a SpawnTask.
    /// </summary>
    public sealed class SpawnTaskRuntimeState
    {
        /// <summary>
        /// Gets the immutable task.
        /// </summary>
        public SpawnTask Task { get; }

        /// <summary>
        /// Gets the runtime lifecycle.
        /// </summary>
        public SpawnTaskLifecycleState Lifecycle
        {
            get;
            internal set;
        }

        public SpawnTaskRuntimeState(
            SpawnTask task)
        {
            Task = task
                ?? throw new ArgumentNullException(nameof(task));

            Lifecycle =
                SpawnTaskLifecycleState.Pending;
        }
    }
}