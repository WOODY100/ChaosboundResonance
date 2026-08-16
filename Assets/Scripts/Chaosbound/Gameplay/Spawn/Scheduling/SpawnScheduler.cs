using Chaosbound.Gameplay.Spawn.Models;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Spawn.Scheduling
{
    /// <summary>
    /// Coordinates the scheduling of SpawnJobs.
    ///
    /// Spawn Scheduling is independent from the gameplay
    /// system that produced the SpawnRequest.
    /// </summary>
    public sealed class SpawnScheduler
    {
        private readonly SpawnSchedulingPolicyResolver resolver;

        public SpawnScheduler(
            SpawnSchedulingPolicyResolver resolver)
        {
            this.resolver =
                resolver
                ?? throw new ArgumentNullException(
                    nameof(resolver));
        }

        /// <summary>
        /// Schedules the supplied Spawn context.
        /// </summary>
        public IReadOnlyList<ScheduledSpawnTask> Schedule(
            SpawnSchedulingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            ISpawnSchedulingPolicy policy =
                resolver.Resolve();

            return policy.Schedule(context);
        }
    }
}