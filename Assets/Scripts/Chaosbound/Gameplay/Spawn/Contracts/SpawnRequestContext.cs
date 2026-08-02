using System;
using Chaosbound.Content.Expeditions.Runtime.Spawn;

namespace Chaosbound.Gameplay.Spawn.Contracts
{
    /// <summary>
    /// Represents the execution context associated with
    /// a SpawnRequest.
    /// </summary>
    public sealed class SpawnRequestContext
    {
        /// <summary>
        /// Gets the runtime spawn configuration.
        /// </summary>
        public RuntimeSpawnConfig RuntimeSpawnConfig { get; }

        /// <summary>
        /// Creates a new spawn request context.
        /// </summary>
        public SpawnRequestContext(
            RuntimeSpawnConfig runtimeSpawnConfig)
        {
            RuntimeSpawnConfig = runtimeSpawnConfig
                ?? throw new ArgumentNullException(nameof(runtimeSpawnConfig));
        }
    }
}