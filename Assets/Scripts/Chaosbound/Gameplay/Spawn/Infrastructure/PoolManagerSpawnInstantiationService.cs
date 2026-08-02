using System;
using Chaosbound.Gameplay.Spawn.Integration;

namespace Chaosbound.Gameplay.Spawn.Infrastructure
{
    /// <summary>
    /// Instantiates gameplay entities using the PoolManager.
    /// </summary>
    public sealed class PoolManagerSpawnInstantiationService :
        ISpawnInstantiationService
    {
        /// <summary>
        /// Spawns the supplied instantiation request.
        /// </summary>
        public void Spawn(
            SpawnInstantiationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // -----------------------------------------------------------------
            // TEMPORARY IMPLEMENTATION
            //
            // The Spawn Runtime already provides a valid instantiation request.
            // The actual interaction with PoolManager will be implemented once
            // the world spawning pipeline (position, rotation, spawn locator,
            // etc.) has been completed.
            // -----------------------------------------------------------------
        }
    }
}