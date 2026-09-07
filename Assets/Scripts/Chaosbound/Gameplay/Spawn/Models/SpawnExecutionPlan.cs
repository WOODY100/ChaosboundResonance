using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Chaosbound.Gameplay.Spawn.Contracts;

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
        /// Gets the request context preserved for execution.
        /// </summary>
        public SpawnRequestContext Context { get; }

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
            IEnumerable<SpawnExecutionPlanEntry> entries,
            SpawnRequestContext context)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            List<SpawnExecutionPlanEntry> list =
                entries.ToList();

            this.entries =
                new ReadOnlyCollection<SpawnExecutionPlanEntry>(list);

            Context = context;
        }
    }
}