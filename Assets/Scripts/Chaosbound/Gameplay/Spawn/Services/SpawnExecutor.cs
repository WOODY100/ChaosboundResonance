using System;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Models;

namespace Chaosbound.Gameplay.Spawn.Services
{
    /// <summary>
    /// Translates SpawnRequests into executable runtime plans.
    /// </summary>
    public sealed class SpawnExecutor
    {
        private readonly SpawnExecutionPlanFactory executionPlanFactory;

        /// <summary>
        /// Creates a SpawnExecutor using the default execution plan factory.
        /// </summary>
        public SpawnExecutor()
            : this(new SpawnExecutionPlanFactory())
        {
        }

        /// <summary>
        /// Creates a SpawnExecutor with the specified execution plan factory.
        /// </summary>
        /// <param name="executionPlanFactory">
        /// Factory responsible for creating execution plans.
        /// </param>
        public SpawnExecutor(
            SpawnExecutionPlanFactory executionPlanFactory)
        {
            this.executionPlanFactory =
                executionPlanFactory
                ?? throw new ArgumentNullException(nameof(executionPlanFactory));
        }

        /// <summary>
        /// Produces an execution plan from the supplied SpawnRequest.
        /// </summary>
        /// <param name="request">
        /// The SpawnRequest to execute.
        /// </param>
        /// <returns>
        /// A runtime execution plan.
        /// </returns>
        public SpawnExecutionPlan Execute(
            SpawnRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return executionPlanFactory.Create(request);
        }
    }
}