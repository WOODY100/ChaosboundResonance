using System;
using UnityEngine;
using Chaosbound.Gameplay.Spawn.Placement.Contracts;
using Chaosbound.Gameplay.Spawn.Placement.Models;

namespace Chaosbound.Gameplay.Spawn.Placement.Strategies
{
    /// <summary>
    /// Resolves a placement around the supplied reference.
    /// </summary>
    public sealed class AroundPlayerPlacementStrategy :
        ISpawnPlacementStrategy
    {
        /// <summary>
        /// Temporary spawn radius.
        /// Will become configurable in a future iteration.
        /// </summary>
        private const float DefaultSpawnRadius = 12f;

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
                context.Reference.position + offset;

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