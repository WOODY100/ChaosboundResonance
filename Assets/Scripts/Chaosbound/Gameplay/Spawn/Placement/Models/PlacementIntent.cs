using Chaosbound.Content.Expeditions.Enums.Spawn;
using Chaosbound.Gameplay.Spawn.Definitions;
using System;

namespace Chaosbound.Gameplay.Spawn.Placement.Models
{
    /// <summary>
    /// Represents the declarative intent to place
    /// a materializable entity into the world.
    /// </summary>
    public sealed class PlacementIntent
    {
        public MaterializableDefinition Materializable { get; }

        public SpawnPlacementPolicy PlacementPolicy { get; }

        public PlacementIntent(
            MaterializableDefinition materializable,
            SpawnPlacementPolicy placementPolicy)
        {
            Materializable =
                materializable
                ?? throw new ArgumentNullException(nameof(materializable));

            PlacementPolicy = placementPolicy;
        }
    }
}