using Chaosbound.Gameplay.Spawn.Execution;

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
        void Materialize(
            SpawnExecutionContext context);
    }
}