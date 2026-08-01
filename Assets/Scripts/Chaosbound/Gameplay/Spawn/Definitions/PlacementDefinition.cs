using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Shared.Contracts;
using System;

namespace Chaosbound.Gameplay.Spawn.Definitions
{
    /// <summary>
    /// Describes where a spawn job should be materialized.
    /// </summary>
    public sealed class PlacementDefinition : IDefinition
    {
        public IPlacementReference Placement { get; }

        public PlacementDefinition(IPlacementReference placement)
        {
            Placement = placement ?? throw new ArgumentNullException(nameof(placement));
        }
    }
}