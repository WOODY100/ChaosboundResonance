using System;
using UnityEngine;
using Chaosbound.Gameplay.Spawn.Placement.Contracts;
using Chaosbound.Gameplay.Spawn.Placement.Models;

namespace Chaosbound.Gameplay.Spawn.Placement.Strategies
{
    /// <summary>
    /// Resolves a placement around the spatial origin
    /// supplied by the Spawn Runtime context.
    /// </summary>
    public sealed class AroundOriginPlacementStrategy :
        ISpawnPlacementStrategy
    {
        /// <summary>
        /// Default distance from the supplied spatial origin.
        /// </summary>
        private const float DefaultSpawnRadius = 3f;

        /// <inheritdoc/>
        public PlacementResolution Resolve(
            PlacementContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Vector2 direction =
                UnityEngine.Random.insideUnitCircle.normalized;

            Vector3 offset =
                new Vector3(
                    direction.x,
                    0f,
                    direction.y)
                * DefaultSpawnRadius;

            Vector3 position =
                context.SpatialOrigin.Position + offset;

            Quaternion rotation =
                Quaternion.LookRotation(
                    -offset.normalized,
                    Vector3.up);

            SpawnPlacement placement =
                new SpawnPlacement(
                    position,
                    rotation);

            return PlacementResolution.Success(
                placement);
        }
    }
}