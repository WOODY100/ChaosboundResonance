using System;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.Placement.Models;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates immutable ResolvedSpawnTask instances.
    /// </summary>
    public sealed class ResolvedSpawnTaskFactory
    {
        /// <summary>
        /// Creates a resolved spawn task.
        /// </summary>
        public ResolvedSpawnTask Create(
            ScheduledSpawnTask scheduledTask,
            PlacementResolution placement)
        {
            if (scheduledTask == null)
                throw new ArgumentNullException(nameof(scheduledTask));

            if (placement == null)
                throw new ArgumentNullException(nameof(placement));

            return new ResolvedSpawnTask(
                scheduledTask,
                placement);
        }
    }
}