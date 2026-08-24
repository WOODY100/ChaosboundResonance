using Chaosbound.Content.Portal.Exit;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Models;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Services
{
    /// <summary>
    /// Builds a declarative Exit Portal spawn plan.
    ///
    /// This planner does not interact with Spawn Runtime.
    /// It does not resolve placement or materialization.
    /// </summary>
    public sealed class ExitPortalSpawnPlanner
    {
        /// <summary>
        /// Builds a spawn plan for the supplied Exit Portal.
        /// </summary>
        public ExitPortalSpawnPlan Build(
            ExitPortalData exitPortal)
        {
            if (exitPortal == null)
            {
                throw new ArgumentNullException(
                    nameof(exitPortal));
            }

            ExitPortalSpawnPlanEntry entry =
                new ExitPortalSpawnPlanEntry(
                    exitPortal,
                    1);

            return new ExitPortalSpawnPlan(
                new List<ExitPortalSpawnPlanEntry>
                {
                    entry
                });
        }
    }
}