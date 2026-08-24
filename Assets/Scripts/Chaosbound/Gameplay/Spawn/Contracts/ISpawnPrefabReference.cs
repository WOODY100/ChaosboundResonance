using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Contracts
{
    /// <summary>
    /// Represents a materializable reference that exposes
    /// the gameplay prefab used during spawn.
    /// </summary>
    public interface ISpawnPrefabReference
    {
        GameObject SpawnPrefab { get; }
    }
}