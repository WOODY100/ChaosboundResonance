using Chaosbound.Content.Expeditions.Enums.Spawn;
using Chaosbound.Gameplay.Spawn.Placement.Contracts;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using Chaosbound.Gameplay.Spawn.Placement.ValueObjects;
using Chaosbound.Shared.Contracts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Chaosbound.Gameplay.Spawn.Placement.Validation
{
    /// <summary>
    /// Validates whether a resolved placement satisfies
    /// the physical placement constraints requested
    /// by the spawn operation.
    /// </summary>
    public sealed class PlacementValidator
    {
        private const float DefaultNavMeshSampleDistance = 0.25f;

        private readonly IPlacementFootprintResolver
            footprintResolver;

        private readonly LayerMask obstacleLayer;

        private readonly float navMeshSampleDistance;

        private readonly int navMeshAreaMask;

        public PlacementValidator(
            IPlacementFootprintResolver footprintResolver,
            LayerMask obstacleLayer,
            float navMeshSampleDistance,
            int navMeshAreaMask)
        {
            this.footprintResolver =
                footprintResolver
                ?? throw new ArgumentNullException(
                    nameof(footprintResolver));

            this.obstacleLayer =
                obstacleLayer;

            if (navMeshSampleDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(navMeshSampleDistance));
            }

            this.navMeshSampleDistance =
                navMeshSampleDistance;

            this.navMeshAreaMask =
                navMeshAreaMask;
        }

        public PlacementResolution Validate(
            IMaterializableReference reference,
            SpawnPlacement placement,
            IReadOnlyList<SpawnConstraintPolicy> spawnConstraints)
        {
            if (reference == null)
                throw new ArgumentNullException(
                    nameof(reference));

            if (placement == null)
                throw new ArgumentNullException(
                    nameof(placement));

            if (spawnConstraints == null)
                throw new ArgumentNullException(
                    nameof(spawnConstraints));

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

            if (HasConstraint(
                spawnConstraints,
                SpawnConstraintPolicy.RequireNavMeshWalkable))
            {
                if (!IsNavMeshWalkable(center))
                {
                    return PlacementResolution.Failure(
                        FailureReason.NavMeshUnavailable);
                }
            }

            return PlacementResolution.Success(
                placement);
        }

        private bool IsNavMeshWalkable(
            Vector3 position)
        {
            return NavMesh.SamplePosition(
                position,
                out NavMeshHit hit,
                navMeshSampleDistance,
                navMeshAreaMask);
        }

        private static bool HasConstraint(
            IReadOnlyList<SpawnConstraintPolicy> constraints,
            SpawnConstraintPolicy constraint)
        {
            for (int i = 0; i < constraints.Count; i++)
            {
                if (constraints[i] == constraint)
                {
                    return true;
                }
            }

            return false;
        }
    }
}