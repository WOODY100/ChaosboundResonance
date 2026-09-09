using Chaosbound.Content.Expeditions.Enums.Spawn;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.Spawn
{
    /// <summary>
    /// Defines the spawn policies used by an expedition.
    /// </summary>
    public sealed class SpawnDefinition
    {
        /// <summary>
        /// Gets the placement policy.
        /// </summary>
        public SpawnPlacementPolicy Placement { get; }
        /// <summary>
        /// Gets every spawn constraint configured
        /// for this expedition.
        /// </summary>
        public IReadOnlyList<SpawnConstraintPolicy> SpawnConstraints { get; }

        public SpawnDefinition(
            SpawnPlacementPolicy placement,
            IReadOnlyList<SpawnConstraintPolicy> spawnConstraints)
        {
            Placement = placement;

            SpawnConstraints =
                new List<SpawnConstraintPolicy>(spawnConstraints);
        }
    }
}