using Chaosbound.Gameplay.Spawn.Placement.Contracts;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using Chaosbound.Gameplay.Spawn.Placement.ValueObjects;
using Chaosbound.Shared.Contracts;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Placement.Validation
{
    /// <summary>
    /// Validates whether a resolved placement has enough
    /// physical space for the materializable entity.
    /// </summary>
    public sealed class PlacementValidator
    {
        private readonly IPlacementFootprintResolver
            footprintResolver;

        private readonly LayerMask obstacleLayer;

        public PlacementValidator(
            IPlacementFootprintResolver footprintResolver,
            LayerMask obstacleLayer)
        {
            this.footprintResolver =
                footprintResolver
                ?? throw new ArgumentNullException(
                    nameof(footprintResolver));

            this.obstacleLayer =
                obstacleLayer;
        }

        public PlacementResolution Validate(
            IMaterializableReference reference,
            SpawnPlacement placement)
        {
            if (reference == null)
                throw new ArgumentNullException(
                    nameof(reference));

            if (placement == null)
                throw new ArgumentNullException(
                    nameof(placement));

            PlacementFootprint footprint =
                footprintResolver.Resolve(
                    reference);

            Vector3 center =
                placement.Position +
                footprint.Center;

            float radius =
                footprint.Radius;

            float halfHeight =
                Mathf.Max(
                    footprint.Height * 0.5f,
                    radius);

            float cylinderHalfHeight =
                halfHeight - radius;

            Vector3 point1 =
                center +
                Vector3.up * cylinderHalfHeight;

            Vector3 point2 =
                center -
                Vector3.up * cylinderHalfHeight;

            bool blocked =
                Physics.CheckCapsule(
                    point1,
                    point2,
                    radius,
                    obstacleLayer,
                    QueryTriggerInteraction.Ignore);

            if (blocked)
            {
                return PlacementResolution.Failure(
                    FailureReason.ObstacleOccupied);
            }

            return PlacementResolution.Success(
                placement);
        }
    }
}