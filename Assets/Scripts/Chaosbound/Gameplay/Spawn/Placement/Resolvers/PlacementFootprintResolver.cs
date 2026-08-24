using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Placement.Contracts;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using Chaosbound.Shared.Contracts;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Placement.Resolvers
{
    /// <summary>
    /// Resolves the physical footprint of a materializable
    /// entity from its gameplay prefab.
    /// </summary>
    public sealed class PlacementFootprintResolver :
        IPlacementFootprintResolver
    {
        public PlacementFootprint Resolve(
            IMaterializableReference reference)
        {
            if (reference == null)
                throw new ArgumentNullException(
                    nameof(reference));

            if (reference is not ISpawnPrefabReference
                prefabReference)
            {
                throw new InvalidOperationException(
                    $"Materializable reference " +
                    $"'{reference.GetType().Name}' does not " +
                    $"implement ISpawnPrefabReference.");
            }

            GameObject prefab =
                prefabReference.SpawnPrefab;

            if (prefab == null)
            {
                throw new InvalidOperationException(
                    $"Materializable reference " +
                    $"'{reference.GetType().Name}' " +
                    $"does not define a SpawnPrefab.");
            }

            CapsuleCollider capsule =
                prefab.GetComponent<CapsuleCollider>();

            if (capsule == null)
            {
                throw new InvalidOperationException(
                    $"Spawn prefab '{prefab.name}' must contain " +
                    $"a CapsuleCollider.");
            }

            if (capsule.direction != 1)
            {
                throw new InvalidOperationException(
                    $"Spawn prefab '{prefab.name}' must use " +
                    $"a Y-axis CapsuleCollider.");
            }

            Vector3 scale =
                prefab.transform.localScale;

            float horizontalScale =
                Mathf.Max(
                    Mathf.Abs(scale.x),
                    Mathf.Abs(scale.z));

            float verticalScale =
                Mathf.Abs(scale.y);

            float radius =
                capsule.radius *
                horizontalScale;

            float height =
                capsule.height *
                verticalScale;

            Vector3 center =
                Vector3.Scale(
                    capsule.center,
                    scale);

            return new PlacementFootprint(
                center,
                radius,
                height);
        }
    }
}