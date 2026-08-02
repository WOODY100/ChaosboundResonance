using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chaosbound.Gameplay.Spawn.Models
{
    /// <summary>
    /// Represents the executable runtime plan produced
    /// from a SpawnRequest.
    /// </summary>
    public sealed class SpawnExecutionPlan
    {
        private readonly IReadOnlyList<SpawnExecutionPlanEntry> entries;

        /// <summary>
        /// Gets the execution plan entries.
        /// </summary>
        public IReadOnlyList<SpawnExecutionPlanEntry> Entries => entries;

        /// <summary>
        /// Gets whether the execution plan contains no work.
        /// </summary>
        public bool IsEmpty => entries.Count == 0;

        /// <summary>
        /// Gets the total number of execution operations.
        /// </summary>
        public int TotalExecutionCount =>
            entries.Sum(entry => entry.Quantity);

        /// <summary>
        /// Creates a new execution plan.
        /// </summary>
        public SpawnExecutionPlan(
            IEnumerable<SpawnExecutionPlanEntry> entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            List<SpawnExecutionPlanEntry> list =
                entries.ToList();

            this.entries =
                new ReadOnlyCollection<SpawnExecutionPlanEntry>(list);
        }
    }
}