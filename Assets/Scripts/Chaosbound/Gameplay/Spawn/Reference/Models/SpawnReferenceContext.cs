using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;

namespace Chaosbound.Gameplay.Spawn.Reference.Models
{
    /// <summary>
    /// Represents the immutable context required
    /// to resolve a spawn reference.
    /// </summary>
    public sealed class SpawnReferenceContext
    {
        public RuntimeSpawnConfig SpawnConfig { get; }

        public RuntimeReferencesConfig References { get; }

        public ExpeditionRuntimeState ExpeditionRuntime { get; }

        public SpawnSpatialOrigin? SpatialOrigin { get; }

        public SpawnReferenceContext(
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references,
            ExpeditionRuntimeState expeditionRuntime,
            SpawnSpatialOrigin? spatialOrigin)
        {
            SpawnConfig =
                spawnConfig
                ?? throw new ArgumentNullException(
                    nameof(spawnConfig));

            References =
                references
                ?? throw new ArgumentNullException(
                    nameof(references));

            ExpeditionRuntime =
                expeditionRuntime
                ?? throw new ArgumentNullException(
                    nameof(expeditionRuntime));

            SpatialOrigin =
                spatialOrigin;
        }
    }
}