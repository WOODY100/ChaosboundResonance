using System.Collections.Generic;
using Chaosbound.Gameplay.Spawn.Models;

namespace Chaosbound.Gameplay.Spawn.Scheduling
{
    /// <summary>
    /// Defines the contract implemented by every
    /// enemy scheduling policy.
    /// </summary>
    public interface IEnemySchedulingPolicy
    {
        /// <summary>
        /// Produces the scheduled spawn tasks required
        /// to execute the supplied scheduling context.
        /// </summary>
        IReadOnlyList<ScheduledSpawnTask> Schedule(
            EnemySchedulingContext context);
    }
}