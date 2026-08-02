using System;
using Chaosbound.Gameplay.Spawn.Models;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates SpawnTaskEntry instances from execution plan entries.
    /// </summary>
    public sealed class SpawnTaskEntryFactory
    {
        /// <summary>
        /// Creates a SpawnTaskEntry.
        /// </summary>
        public SpawnTaskEntry Create(
            SpawnExecutionPlanEntry executionEntry,
            int quantity)
        {
            if (executionEntry == null)
                throw new ArgumentNullException(nameof(executionEntry));

            return new SpawnTaskEntry(
                executionEntry.Materializable,
                quantity);
        }
    }
}