using Chaosbound.Content.Enemy.MiniBosses;
using Chaosbound.Gameplay.MiniBosses.Models;
using System;

namespace Chaosbound.Gameplay.MiniBosses.Services
{
    /// <summary>
    /// Builds a concrete MiniBossSpawnPlan from the
    /// MiniBoss selected by the MiniBoss Domain.
    ///
    /// This builder does not select the MiniBoss.
    /// It only converts the selected MiniBoss into
    /// a materialization plan.
    /// </summary>
    public sealed class MiniBossSpawnPlanBuilder
    {
        public MiniBossSpawnPlan Build(
            MiniBossData miniBoss)
        {
            if (miniBoss == null)
            {
                throw new ArgumentNullException(
                    nameof(miniBoss));
            }

            MiniBossSpawnPlanEntry entry =
                new MiniBossSpawnPlanEntry(
                    miniBoss,
                    1);

            return new MiniBossSpawnPlan(
                new[]
                {
                    entry
                });
        }
    }
}