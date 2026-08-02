using System;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Models;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates SpawnExecutionPlanEntry instances from
    /// SpawnRequestEntry models.
    /// </summary>
    public sealed class SpawnExecutionPlanEntryFactory
    {
        /// <summary>
        /// Creates a SpawnExecutionPlanEntry.
        /// </summary>
        /// <param name="requestEntry">
        /// The SpawnRequest entry to translate.
        /// </param>
        /// <returns>
        /// A runtime execution plan entry.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when requestEntry is null.
        /// </exception>
        public SpawnExecutionPlanEntry Create(
            SpawnRequestEntry requestEntry)
        {
            if (requestEntry == null)
            {
                throw new ArgumentNullException(nameof(requestEntry));
            }

            return new SpawnExecutionPlanEntry(
                requestEntry.Materializable,
                requestEntry.Quantity);
        }
    }
}