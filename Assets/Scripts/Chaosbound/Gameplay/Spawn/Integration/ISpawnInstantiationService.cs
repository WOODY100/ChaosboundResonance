using Chaosbound.Gameplay.Spawn.Infrastructure;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Integration
{
    /// <summary>
    /// Instantiates entities into the game world.
    /// </summary>
    public interface ISpawnInstantiationService
    {
        GameObject Spawn(
            SpawnInstantiationRequest request);
    }
}