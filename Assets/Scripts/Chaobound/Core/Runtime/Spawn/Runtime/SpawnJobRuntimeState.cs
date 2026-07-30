using Chaosbound.Core.Runtime.Spawn.Domain;
using System;

namespace Chaosbound.Core.Runtime.Spawn.Runtime
{
    /// <summary>
    /// Represents the runtime execution state associated with a SpawnJob.
    /// </summary>
    public sealed class SpawnJobRuntimeState
    {
        /// <summary>
        /// Gets the immutable declarative SpawnJob associated with this runtime state.
        /// </summary>
        public SpawnJob Job { get; }

        /// <summary>
        /// Gets the current lifecycle state of the SpawnJob during the run.
        /// Only the Spawn Domain may modify this value.
        /// </summary>
        public SpawnJobLifecycleState Lifecycle { get; internal set; }

        public SpawnJobRuntimeState(SpawnJob job)
        {
            Job = job ?? throw new ArgumentNullException(nameof(job));

            Lifecycle = SpawnJobLifecycleState.Pending;
        }
    }
}