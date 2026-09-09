using Chaosbound.Gameplay.Spawn.Domain;
using Chaosbound.Gameplay.Spawn.Scheduling;
using System;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates immutable contexts required by
    /// Spawn Scheduling.
    /// </summary>
    public sealed class SpawnSchedulingContextFactory
    {
        public SpawnSchedulingContext Create(SpawnJob job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            return new SpawnSchedulingContext(job);
        }
    }
}
