using Chaosbound.Content.Expeditions.Enums.Enemy;
using Chaosbound.Gameplay.Spawn.Calculators;
using Chaosbound.Gameplay.Spawn.Factories;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Spawn.Scheduling
{
    /// <summary>
    /// Resolves the scheduling policy implementation
    /// used by the Enemy Scheduler.
    /// </summary>
    public sealed class EnemySchedulingPolicyResolver
    {
        private readonly IReadOnlyDictionary<
            EnemySchedulingPolicy,
            IEnemySchedulingPolicy> policies;

        public EnemySchedulingPolicyResolver(
            SpawnBatchCalculator batchCalculator,
            SpawnTaskEntryFactory taskEntryFactory,
            SpawnTaskFactory taskFactory)
        {
            policies =
                new Dictionary<
                    EnemySchedulingPolicy,
                    IEnemySchedulingPolicy>
                {
                    {
                        EnemySchedulingPolicy.Continuous,
                        new ContinuousSchedulingPolicy(
                            batchCalculator,
                            taskEntryFactory,
                            taskFactory)
                    }
                };
        }

        /// <summary>
        /// Resolves the scheduling policy implementation.
        /// </summary>
        public IEnemySchedulingPolicy Resolve(
            EnemySchedulingPolicy policy)
        {
            if (!policies.TryGetValue(
                policy,
                out IEnemySchedulingPolicy implementation))
            {
                throw new InvalidOperationException(
                    $"Scheduling policy '{policy}' is not registered.");
            }

            return implementation;
        }
    }
}