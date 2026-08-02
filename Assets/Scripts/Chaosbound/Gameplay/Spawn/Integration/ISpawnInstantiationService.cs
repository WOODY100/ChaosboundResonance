using Chaosbound.Gameplay.Spawn.Infrastructure;

namespace Chaosbound.Gameplay.Spawn.Integration
{
    /// <summary>
    /// Instantiates entities into the game world.
    /// </summary>
    public interface ISpawnInstantiationService
    {
        /// <summary>
        /// Spawns the supplied instantiation request.
        /// </summary>
        void Spawn(
            SpawnInstantiationRequest request);
    }
}