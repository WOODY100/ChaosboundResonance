using Chaosbound.Gameplay.Spawn.Models;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Spawn.Scheduling
{
    /// <summary>
    /// Defines the contract implemented by every
    /// Spawn Scheduling policy.
    /// </summary>
    public interface ISpawnSchedulingPolicy
    {
        /// <summary>
        /// Produces the scheduled spawn tasks required
        /// to execute the supplied scheduling context.
        /// </summary>
        IReadOnlyList<ScheduledSpawnTask> Schedule(
            SpawnSchedulingContext context);
    }
}