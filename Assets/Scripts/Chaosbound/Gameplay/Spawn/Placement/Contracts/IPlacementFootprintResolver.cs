using Chaosbound.Shared.Contracts;
using Chaosbound.Gameplay.Spawn.Placement.Models;

namespace Chaosbound.Gameplay.Spawn.Placement.Contracts
{
    /// <summary>
    /// Resolves the physical footprint required
    /// by a materializable reference.
    /// </summary>
    public interface IPlacementFootprintResolver
    {
        PlacementFootprint Resolve(
            IMaterializableReference reference);
    }
}