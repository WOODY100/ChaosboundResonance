using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Spawn.Domain;
using System;

namespace Chaosbound.Gameplay.Spawn.Runtime
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

        public ExpeditionRuntimeState ExpeditionRuntime { get; }

        public SpawnJobRuntimeState(
            SpawnJob job,
            ExpeditionRuntimeState expeditionRuntime)
        {
            Job = job
                ?? throw new ArgumentNullException(nameof(job));

            ExpeditionRuntime =
                expeditionRuntime
                ?? throw new ArgumentNullException(nameof(expeditionRuntime));

            Lifecycle = SpawnJobLifecycleState.Pending;
        }
    }
}