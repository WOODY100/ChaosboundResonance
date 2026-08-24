using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Spawn.Reference.Models;
using System;

namespace Chaosbound.Gameplay.Spawn.Reference.Factories
{
    /// <summary>
    /// Creates immutable SpawnReferenceContext instances.
    /// </summary>
    public sealed class SpawnReferenceContextFactory
    {
        public SpawnReferenceContext Create(
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references,
            ExpeditionRuntimeState expeditionRuntime)
        {
            if (spawnConfig == null)
                throw new ArgumentNullException(
                    nameof(spawnConfig));

            if (references == null)
                throw new ArgumentNullException(
                    nameof(references));

            if (expeditionRuntime == null)
                throw new ArgumentNullException(
                    nameof(expeditionRuntime));

            return new SpawnReferenceContext(
                spawnConfig,
                references,
                expeditionRuntime);
        }
    }
}