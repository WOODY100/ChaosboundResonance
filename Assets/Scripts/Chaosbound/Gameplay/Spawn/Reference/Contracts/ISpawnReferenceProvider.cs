using Chaosbound.Gameplay.Spawn.Reference.Models;

namespace Chaosbound.Gameplay.Spawn.Reference.Contracts
{
    /// <summary>
    /// Resolves a runtime reference used
    /// by the Spawn Runtime.
    /// </summary>
    public interface ISpawnReferenceProvider
    {
        /// <summary>
        /// Resolves a runtime reference.
        /// </summary>
        SpawnReferenceResult Resolve(
            SpawnReferenceContext context);
    }
}