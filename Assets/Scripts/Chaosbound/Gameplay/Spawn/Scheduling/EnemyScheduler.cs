using Chaosbound.Debugging;
using Chaosbound.Gameplay.Spawn.Models;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Spawn.Scheduling
{
    /// <summary>
    /// Coordinates the scheduling of enemy SpawnJobs.
    /// </summary>
    public sealed class EnemyScheduler
    {
        private readonly EnemySchedulingPolicyResolver resolver;

        public EnemyScheduler(
            EnemySchedulingPolicyResolver resolver)
        {
            this.resolver =
                resolver
                ?? throw new ArgumentNullException(nameof(resolver));
        }

        /// <summary>
        /// Schedules the supplied enemy context.
        /// </summary>
        public IReadOnlyList<ScheduledSpawnTask> Schedule(
            EnemySchedulingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            IEnemySchedulingPolicy policy =
                resolver.Resolve(
                    context.EnemyConfig.SchedulingPolicy);

            IReadOnlyList<ScheduledSpawnTask> tasks =
                policy.Schedule(context);

            SpawnRuntimeDebugger.LogScheduling(
                context,
                tasks.Count);

            return tasks;
        }
    }
}