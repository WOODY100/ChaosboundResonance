using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Gameplay.Bosses.Models;
using System;

namespace Chaosbound.Gameplay.Bosses.Services
{
    /// <summary>
    /// Builds a concrete BossSpawnPlan from the Boss
    /// selected by the Boss Domain.
    ///
    /// This builder does not select the Boss.
    /// It only converts the selected Boss into
    /// a materialization plan.
    /// </summary>
    public sealed class BossSpawnPlanBuilder
    {
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
                new[]
                {
                    entry
                });
        }
    }
}