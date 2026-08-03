using System;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.Placement.Models;

namespace Chaosbound.Gameplay.Spawn.Placement.Factories
{
    /// <summary>
    /// Creates immutable PlacementIntent instances.
    /// </summary>
    public sealed class PlacementIntentFactory
    {
        /// <summary>
        /// Creates a PlacementIntent.
        /// </summary>
        public PlacementIntent Create(
            ScheduledSpawnTask scheduledTask,
            RuntimeSpawnConfig spawnConfig)
        {
            if (scheduledTask == null)
                throw new ArgumentNullException(nameof(scheduledTask));

            if (spawnConfig == null)
                throw new ArgumentNullException(nameof(spawnConfig));

            return new PlacementIntent(
                scheduledTask
                    .Task
                    .Entry
                    .Materializable,
                spawnConfig.Placement);
        }
    }
}