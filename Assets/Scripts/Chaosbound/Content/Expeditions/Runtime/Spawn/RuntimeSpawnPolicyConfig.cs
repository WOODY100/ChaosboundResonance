using Chaosbound.Content.Expeditions.Enums.Spawn;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Spawn
{
    /// <summary>
    /// Immutable runtime configuration for one
    /// Spawn policy profile.
    /// </summary>
    public sealed class RuntimeSpawnPolicyConfig
    {
        /// <summary>
        /// Gets the placement policy.
        /// </summary>
        public SpawnPlacementPolicy Placement { get; }

        /// <summary>
        /// Gets the activation policy.
        /// </summary>
        public SpawnActivationPolicy Activation { get; }

        /// <summary>
        /// Gets the spawn constraints.
        /// </summary>
        public IReadOnlyList<SpawnConstraintPolicy>
            SpawnConstraints
        { get; }

        public RuntimeSpawnPolicyConfig(
            SpawnPlacementPolicy placement,
            SpawnActivationPolicy activation,
            IReadOnlyList<SpawnConstraintPolicy> spawnConstraints)
        {
            Placement =
                placement;

            Activation =
                activation;

            SpawnConstraints =
                new List<SpawnConstraintPolicy>(
                    spawnConstraints
                    ?? throw new ArgumentNullException(
                        nameof(spawnConstraints)));
        }
    }
}