using Chaosbound.Content.Enemy.MiniBosses;
using Chaosbound.Gameplay.MiniBosses.Models;
using System;

namespace Chaosbound.Gameplay.MiniBosses.Services
{
    /// <summary>
    /// Resolves a selected MiniBoss into a concrete
    /// MiniBossSpawnPlan.
    ///
    /// This planner does not select the MiniBoss.
    /// It does not create SpawnRequests.
    /// It does not interact with Spawn Runtime.
    /// </summary>
    public sealed class MiniBossSpawnPlanner
    {
        private readonly MiniBossSpawnPlanBuilder
            spawnPlanBuilder;

        public MiniBossSpawnPlanner(
            MiniBossSpawnPlanBuilder spawnPlanBuilder)
        {
            this.spawnPlanBuilder =
                spawnPlanBuilder
                ?? throw new ArgumentNullException(
                    nameof(spawnPlanBuilder));
        }

        /// <summary>
        /// Builds a spawn plan for the selected MiniBoss.
        /// </summary>
        public MiniBossSpawnPlan Build(
            MiniBossData miniBoss)
        {
            if (miniBoss == null)
            {
                throw new ArgumentNullException(
                    nameof(miniBoss));
            }

            return spawnPlanBuilder.Build(
                miniBoss);
        }
    }
}