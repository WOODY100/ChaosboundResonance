using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Models;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates SpawnExecutionPlan aggregate roots.
    /// </summary>
    public sealed class SpawnExecutionPlanFactory
    {
        private readonly SpawnExecutionPlanEntryFactory entryFactory;

        /// <summary>
        /// Creates a SpawnExecutionPlanFactory using
        /// the default specialized factory.
        /// </summary>
        public SpawnExecutionPlanFactory()
            : this(
                new SpawnExecutionPlanEntryFactory())
        {
        }

        /// <summary>
        /// Creates a SpawnExecutionPlanFactory with
        /// the specified specialized factory.
        /// </summary>
        /// <param name="entryFactory">
        /// Factory responsible for creating execution entries.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when entryFactory is null.
        /// </exception>
        public SpawnExecutionPlanFactory(
            SpawnExecutionPlanEntryFactory entryFactory)
        {
            this.entryFactory = entryFactory
                ?? throw new ArgumentNullException(nameof(entryFactory));
        }

        /// <summary>
        /// Creates a SpawnExecutionPlan from a SpawnRequest.
        /// </summary>
        /// <param name="request">
        /// The SpawnRequest to translate.
        /// </param>
        /// <returns>
        /// A runtime execution plan.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when request is null.
        /// </exception>
        public SpawnExecutionPlan Create(
            SpawnRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            List<SpawnExecutionPlanEntry> entries =
                new List<SpawnExecutionPlanEntry>();

            foreach (SpawnRequestEntry requestEntry in request.Entries)
            {
                entries.Add(
                    entryFactory.Create(requestEntry));
            }

            return new SpawnExecutionPlan(entries);
        }
    }
}