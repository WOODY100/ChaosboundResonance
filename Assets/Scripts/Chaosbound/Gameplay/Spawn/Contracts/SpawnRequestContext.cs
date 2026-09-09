using Chaosbound.Gameplay.Spawn.Reference.Models;
using System;

namespace Chaosbound.Gameplay.Spawn.Contracts
{
    /// <summary>
    /// Represents the immutable context associated
    /// with a SpawnRequest.
    /// </summary>
    public sealed class SpawnRequestContext
    {
        /// <summary>
        /// Gets the optional spatial origin supplied
        /// by the producer of the request.
        /// </summary>
        public SpawnSpatialOrigin? SpatialOrigin { get; }

        public SpawnRequestContext(
            SpawnSpatialOrigin? spatialOrigin)
        {
            SpatialOrigin =
                spatialOrigin;
        }
    }
}