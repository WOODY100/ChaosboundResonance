using System;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Spawn.Reference.Models;

namespace Chaosbound.Gameplay.Spawn.Reference.Factories
{
    /// <summary>
    /// Creates immutable SpawnReferenceContext instances.
    /// </summary>
    public sealed class SpawnReferenceContextFactory
    {
        /// <summary>
        /// Creates a SpawnReferenceContext.
        /// </summary>
        public SpawnReferenceContext Create(
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references)
        {
            if (spawnConfig == null)
                throw new ArgumentNullException(nameof(spawnConfig));

            if (references == null)
                throw new ArgumentNullException(nameof(references));

            return new SpawnReferenceContext(
                spawnConfig,
                references);
        }
    }
}