using Chaosbound.Content.Expeditions.Enums.Spawn;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Spawn
{
    /// <summary>
    /// Immutable runtime configuration describing the spawn
    /// policies used by the current expedition.
    /// </summary>
    public sealed class RuntimeSpawnConfig
    {
        /// <summary>
        /// Gets the placement policy.
        /// </summary>
        public SpawnPlacementPolicy Placement { get; }
        /// <summary>
        /// Gets every spawn constraint configured for
        /// the current expedition.
        /// </summary>
        public IReadOnlyList<SpawnConstraintPolicy> SpawnConstraints { get; }

        public RuntimeSpawnConfig(
            SpawnPlacementPolicy placement,
            IReadOnlyList<SpawnConstraintPolicy> spawnConstraints)
        {
            Placement =
                placement;

            SpawnConstraints =
                new List<SpawnConstraintPolicy>(
                    spawnConstraints
                    ?? throw new ArgumentNullException(
                        nameof(spawnConstraints)));
        }

        public RuntimeSpawnConfig WithPlacement(
            SpawnPlacementPolicy placement)
        {
            return new RuntimeSpawnConfig(
                placement,
                SpawnConstraints);
        }
    }
}