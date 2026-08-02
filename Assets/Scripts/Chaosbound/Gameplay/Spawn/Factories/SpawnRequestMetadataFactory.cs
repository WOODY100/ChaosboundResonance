using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.ValueObjects;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates SpawnRequestMetadata instances.
    /// </summary>
    public sealed class SpawnRequestMetadataFactory
    {
        /// <summary>
        /// Creates metadata for a SpawnRequest.
        /// </summary>
        public SpawnRequestMetadata Create(
            SpawnRequestOrigin origin)
        {
            return new SpawnRequestMetadata(
                SpawnRequestId.New(),
                origin);
        }
    }
}