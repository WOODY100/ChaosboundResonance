using System;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using Chaosbound.Gameplay.Spawn.Reference.Models;

namespace Chaosbound.Gameplay.Spawn.Placement.Factories
{
    /// <summary>
    /// Creates immutable PlacementContext instances.
    /// </summary>
    public sealed class PlacementContextFactory
    {
        /// <summary>
        /// Creates a PlacementContext.
        /// </summary>
        public PlacementContext Create(
            PlacementIntent intent,
            SpawnSpatialOrigin spatialOrigin)
        {
            if (intent == null)
                throw new ArgumentNullException(nameof(intent));

            return new PlacementContext(
                intent,
                spatialOrigin);
        }
    }
}