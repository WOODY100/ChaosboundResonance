using System;
using UnityEngine;
using Chaosbound.Gameplay.Spawn.Placement.Contracts;
using Chaosbound.Gameplay.Spawn.Placement.Models;

namespace Chaosbound.Gameplay.Spawn.Placement.Strategies
{
    /// <summary>
    /// Resolves a placement near the supplied reference.
    /// </summary>
    public sealed class NearReferencePlacementStrategy :
        ISpawnPlacementStrategy
    {
        /// <summary>
        /// Default distance from the reference.
        /// </summary>
        private const float DefaultSpawnRadius = 3f;

        /// <summary>
        /// Additional search distance applied per failed placement attempt.
        /// </summary>
        private const float SpawnRadiusStep = 1.5f;

        /// <inheritdoc/>
        public PlacementResolution Resolve(
            PlacementContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Vector2 direction =
                UnityEngine.Random.insideUnitCircle.normalized;

            float spawnRadius =
                DefaultSpawnRadius +
                context.AttemptIndex * SpawnRadiusStep;

            Vector3 offset =
                new Vector3(
                    direction.x,
                    0f,
                    direction.y)
                * spawnRadius;

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