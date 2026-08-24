using System;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Spawn.Contracts;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates SpawnRequestContext instances.
    /// </summary>
    public sealed class SpawnRequestContextFactory
    {
        /// <summary>
        /// Creates a SpawnRequestContext from the runtime spawn configuration.
        /// </summary>
        public SpawnRequestContext Create(
            RuntimeSpawnConfig runtimeSpawnConfig)
        {
            if (runtimeSpawnConfig == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeSpawnConfig));
            }

            return new SpawnRequestContext(
                runtimeSpawnConfig);
        }
    }
}