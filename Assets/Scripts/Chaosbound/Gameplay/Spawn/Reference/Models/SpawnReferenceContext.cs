using System;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;

namespace Chaosbound.Gameplay.Spawn.Reference.Models
{
    /// <summary>
    /// Represents the immutable context required
    /// to resolve a spawn reference.
    /// </summary>
    public sealed class SpawnReferenceContext
    {
        /// <summary>
        /// Gets the runtime spawn configuration.
        /// </summary>
        public RuntimeSpawnConfig SpawnConfig { get; }

        /// <summary>
        /// Gets the runtime world references.
        /// </summary>
        public RuntimeReferencesConfig References { get; }

        public SpawnReferenceContext(
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references)
        {
            SpawnConfig =
                spawnConfig
                ?? throw new ArgumentNullException(nameof(spawnConfig));

            References =
                references
                ?? throw new ArgumentNullException(nameof(references));
        }
    }
}