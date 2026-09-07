using System;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Reference.Models;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates SpawnRequestContext instances.
    /// </summary>
    public sealed class SpawnRequestContextFactory
    {
        /// <summary>
        /// Creates a SpawnRequestContext from the runtime spawn configuration
        /// and optional spatial origin.
        /// </summary>
        public SpawnRequestContext Create(
            RuntimeSpawnConfig runtimeSpawnConfig,
            SpawnSpatialOrigin? spatialOrigin)
        {
            if (runtimeSpawnConfig == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeSpawnConfig));
            }

            return new SpawnRequestContext(
                runtimeSpawnConfig,
                spatialOrigin);
        }
    }
}