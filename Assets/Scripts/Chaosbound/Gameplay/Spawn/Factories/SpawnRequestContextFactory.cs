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
        /// Creates a SpawnRequestContext from an optional spatial origin.
        /// </summary>
        public SpawnRequestContext Create(
            SpawnSpatialOrigin? spatialOrigin)
        {
            return new SpawnRequestContext(
                spatialOrigin);
        }
    }
}