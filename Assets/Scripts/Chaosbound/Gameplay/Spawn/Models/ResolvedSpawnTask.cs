using System;
using Chaosbound.Gameplay.Spawn.Placement.Models;

namespace Chaosbound.Gameplay.Spawn.Models
{
    /// <summary>
    /// Represents a scheduled spawn task whose
    /// placement has already been resolved.
    /// </summary>
    public sealed class ResolvedSpawnTask
    {
        /// <summary>
        /// Gets the scheduled spawn task.
        /// </summary>
        public ScheduledSpawnTask ScheduledTask { get; }

        /// <summary>
        /// Gets the resolved placement.
        /// </summary>
        public PlacementResolution Placement { get; }

        /// <summary>
        /// Creates a resolved spawn task.
        /// </summary>
        public ResolvedSpawnTask(
            ScheduledSpawnTask scheduledTask,
            PlacementResolution placement)
        {
            ScheduledTask =
                scheduledTask
                ?? throw new ArgumentNullException(
                    nameof(scheduledTask));

            Placement =
                placement
                ?? throw new ArgumentNullException(
                    nameof(placement));
        }
    }
}