using System;
using UnityEngine;
using Chaosbound.Gameplay.Spawn.Placement.Contracts;
using Chaosbound.Gameplay.Spawn.Placement.Models;

namespace Chaosbound.Gameplay.Spawn.Placement.Strategies
{
    /// <summary>
    /// Resolves a placement exactly at the supplied
    /// spatial origin.
    /// </summary>
    public sealed class AtOriginPlacementStrategy :
        ISpawnPlacementStrategy
    {
        public PlacementResolution Resolve(
            PlacementContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            SpawnPlacement placement =
                new SpawnPlacement(
                    context.SpatialOrigin.Position,
                    Quaternion.identity);

            return PlacementResolution.Success(
                placement);
        }
    }
}