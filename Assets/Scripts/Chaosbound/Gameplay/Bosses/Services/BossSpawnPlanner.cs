using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Gameplay.Bosses.Models;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Bosses.Services
{
    /// <summary>
    /// Builds a declarative BossSpawnPlan from
    /// the Boss selected by the Boss Domain.
    ///
    /// This planner does not select the Boss.
    /// It does not interact with Spawn Runtime.
    /// It does not resolve placement or materialization.
    /// </summary>
    public sealed class BossSpawnPlanner
    {
        /// <summary>
        /// Builds a spawn plan for the selected Boss.
        /// </summary>
        public BossSpawnPlan Build(
            BossData boss)
        {
            if (boss == null)
            {
                throw new ArgumentNullException(
                    nameof(boss));
            }

            BossSpawnPlanEntry entry =
                new BossSpawnPlanEntry(
                    boss,
                    1);

            return new BossSpawnPlan(
                new List<BossSpawnPlanEntry>
                {
                    entry
                });
        }
    }
}