using Chaosbound.Gameplay.Spawn.Execution;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Materialization
{
    /// <summary>
    /// Materializes a scheduled spawn task into the game world.
    /// </summary>
    public interface ISpawnMaterializer
    {
        /// <summary>
        /// Materializes the supplied execution context.
        /// </summary>
        GameObject Materialize(
            SpawnExecutionContext context);
    }
}