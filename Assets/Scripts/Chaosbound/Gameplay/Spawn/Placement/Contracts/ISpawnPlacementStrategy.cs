using Chaosbound.Gameplay.Spawn.Placement.Models;

namespace Chaosbound.Gameplay.Spawn.Placement.Contracts
{
    /// <summary>
    /// Resolves a valid placement for a materializable entity.
    /// </summary>
    public interface ISpawnPlacementStrategy
    {
        /// <summary>
        /// Attempts to resolve a placement for the supplied context.
        /// </summary>
        PlacementResolution Resolve(
            PlacementContext context);
    }
}