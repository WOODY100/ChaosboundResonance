using System;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.ValueObjects;

namespace Chaosbound.Gameplay.Spawn.Domain
{
    /// <summary>
    /// Represents an immutable unit of work
    /// executed by the Spawn Runtime.
    /// </summary>
    public sealed class SpawnJob
    {
        /// <summary>
        /// Gets the immutable job identity.
        /// </summary>
        public SpawnJobIdentity Identity { get; }

        /// <summary>
        /// Gets the execution plan entry represented by this job.
        /// </summary>
        public SpawnExecutionPlanEntry Entry { get; }

        /// <summary>
        /// Creates a new SpawnJob.
        /// </summary>
        public SpawnJob(
            SpawnJobIdentity identity,
            SpawnExecutionPlanEntry executionEntry)
        {
            Entry = executionEntry
                ?? throw new ArgumentNullException(nameof(executionEntry));

            Identity = identity;
        }
    }
}