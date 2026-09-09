using Chaosbound.Gameplay.Spawn.Domain;
using System;

namespace Chaosbound.Gameplay.Spawn.Scheduling
{
    /// <summary>
    /// Represents the immutable context required by
    /// Spawn Scheduling to generate scheduled spawn tasks.
    /// </summary>
    public sealed class SpawnSchedulingContext
    {
        /// <summary>
        /// Gets the SpawnJob to schedule.
        /// </summary>
        public SpawnJob Job { get; }

        public SpawnSchedulingContext(SpawnJob job)
        {
            Job =
                job
                ?? throw new ArgumentNullException(nameof(job));
        }
    }
}
