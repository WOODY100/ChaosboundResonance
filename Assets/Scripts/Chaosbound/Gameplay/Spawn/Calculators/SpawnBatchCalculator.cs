using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.Spawn.Models;

namespace Chaosbound.Gameplay.Spawn.Calculators
{
    /// <summary>
    /// Calculates how a SpawnExecutionPlanEntry
    /// should be divided into execution batches.
    /// </summary>
    public sealed class SpawnBatchCalculator
    {
        /// <summary>
        /// Temporary batch size.
        /// Will become configurable through RuntimeEnemyConfig.
        /// </summary>
        private const int DefaultBatchSize = 5;

        /// <summary>
        /// Calculates the execution batches.
        /// </summary>
        public IReadOnlyList<int> Calculate(
            SpawnExecutionPlanEntry executionEntry)
        {
            if (executionEntry == null)
                throw new ArgumentNullException(nameof(executionEntry));

            List<int> batches = new();

            int remaining = executionEntry.Quantity;

            while (remaining > 0)
            {
                int quantity =
                    Math.Min(DefaultBatchSize, remaining);

                batches.Add(quantity);

                remaining -= quantity;
            }

            return batches;
        }
    }
}