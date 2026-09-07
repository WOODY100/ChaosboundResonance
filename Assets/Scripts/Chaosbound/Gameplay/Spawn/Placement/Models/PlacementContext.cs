using System;
using Chaosbound.Gameplay.Spawn.Reference.Models;

namespace Chaosbound.Gameplay.Spawn.Placement.Models
{
    /// <summary>
    /// Represents the immutable context required
    /// to resolve a placement.
    /// </summary>
    public sealed class PlacementContext
    {
        /// <summary>
        /// Gets the placement intent.
        /// </summary>
        public PlacementIntent Intent { get; }

        /// <summary>
        /// Gets the world-space spatial origin
        /// used during placement resolution.
        /// </summary>
        public SpawnSpatialOrigin SpatialOrigin { get; }

        /// <summary>
        /// Creates a new placement context.
        /// </summary>
        public PlacementContext(
            PlacementIntent intent,
            SpawnSpatialOrigin spatialOrigin)
        {
            Intent =
                intent
                ?? throw new ArgumentNullException(nameof(intent));

            SpatialOrigin =
                spatialOrigin;
        }
    }
}