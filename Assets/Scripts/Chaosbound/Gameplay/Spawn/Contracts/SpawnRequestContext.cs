using Chaosbound.Content.Expeditions.Runtime.Spawn;
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
        /// Gets the runtime spawn configuration associated
        /// with the request.
        /// </summary>
        public RuntimeSpawnConfig SpawnConfig { get; }

        /// <summary>
        /// Gets the optional spatial origin supplied
        /// by the producer of the request.
        /// </summary>
        public SpawnSpatialOrigin? SpatialOrigin { get; }

        public SpawnRequestContext(
            RuntimeSpawnConfig spawnConfig,
            SpawnSpatialOrigin? spatialOrigin)
        {
            SpawnConfig =
                spawnConfig
                ?? throw new ArgumentNullException(
                    nameof(spawnConfig));

            SpatialOrigin =
                spatialOrigin;
        }
    }
}