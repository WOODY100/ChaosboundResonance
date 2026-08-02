using System;
using Chaosbound.Gameplay.Spawn.ValueObjects;

namespace Chaosbound.Gameplay.Spawn.Contracts
{
    /// <summary>
    /// Represents metadata associated with a SpawnRequest.
    /// </summary>
    public sealed class SpawnRequestMetadata
    {
        /// <summary>
        /// Gets the unique request identifier.
        /// </summary>
        public SpawnRequestId RequestId { get; }

        /// <summary>
        /// Gets the request origin.
        /// </summary>
        public SpawnRequestOrigin Origin { get; }

        /// <summary>
        /// Creates new request metadata.
        /// </summary>
        public SpawnRequestMetadata(
            SpawnRequestId requestId,
            SpawnRequestOrigin origin)
        {
            RequestId = requestId;
            Origin = origin;
        }
    }
}