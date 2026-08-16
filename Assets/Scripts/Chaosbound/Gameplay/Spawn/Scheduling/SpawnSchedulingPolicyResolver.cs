using Chaosbound.Gameplay.Spawn.Calculators;
using Chaosbound.Gameplay.Spawn.Factories;
using System;

namespace Chaosbound.Gameplay.Spawn.Scheduling
{
    /// <summary>
    /// Resolves the scheduling policy implementation
    /// used by the Spawn Runtime.
    ///
    /// V1 provides a single scheduling strategy:
    /// ContinuousSchedulingPolicy.
    /// </summary>
    public sealed class SpawnSchedulingPolicyResolver
    {
        private readonly ISpawnSchedulingPolicy
            continuousPolicy;

        public SpawnSchedulingPolicyResolver(
            SpawnBatchCalculator batchCalculator,
            SpawnTaskEntryFactory taskEntryFactory,
            SpawnTaskFactory taskFactory)
        {
            if (batchCalculator == null)
                throw new ArgumentNullException(
                    nameof(batchCalculator));

            if (taskEntryFactory == null)
                throw new ArgumentNullException(
                    nameof(taskEntryFactory));

            if (taskFactory == null)
                throw new ArgumentNullException(
                    nameof(taskFactory));

            continuousPolicy =
                new ContinuousSchedulingPolicy(
                    batchCalculator,
                    taskEntryFactory,
                    taskFactory);
        }

        /// <summary>
        /// Resolves the scheduling policy implementation
        /// available in Spawn Runtime V1.
        /// </summary>
        public ISpawnSchedulingPolicy Resolve()
        {
            return continuousPolicy;
        }
    }
}