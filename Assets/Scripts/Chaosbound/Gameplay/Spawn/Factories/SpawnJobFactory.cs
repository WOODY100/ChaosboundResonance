using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Chaosbound.Gameplay.Spawn.Domain;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.ValueObjects;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates immutable SpawnJob instances from
    /// a SpawnExecutionPlan.
    /// </summary>
    public sealed class SpawnJobFactory
    {
        /// <summary>
        /// Creates the runtime jobs required to execute
        /// the supplied execution plan.
        /// </summary>
        /// <param name="executionPlan">
        /// Execution plan produced by the Spawn Runtime.
        /// </param>
        /// <returns>
        /// Immutable collection of SpawnJobs.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the execution plan is null.
        /// </exception>
        public IReadOnlyList<SpawnJob> Create(
            SpawnExecutionPlan executionPlan)
        {
            if (executionPlan == null)
            {
                throw new ArgumentNullException(nameof(executionPlan));
            }

            List<SpawnJob> jobs = new();

            foreach (SpawnExecutionPlanEntry entry in executionPlan.Entries)
            {
                jobs.Add(
                    new SpawnJob(
                        SpawnJobIdentity.New(),
                        entry));
            }

            return new ReadOnlyCollection<SpawnJob>(jobs);
        }
    }
}