using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Placement.Models
{
    /// <summary>
    /// Represents the immutable world transform
    /// where an entity will be materialized.
    /// </summary>
    public sealed class SpawnPlacement
    {
        /// <summary>
        /// Gets the world position.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Gets the world rotation.
        /// </summary>
        public Quaternion Rotation { get; }

        /// <summary>
        /// Creates a new immutable SpawnPlacement.
        /// </summary>
        /// <param name="position">
        /// World position where the entity will be materialized.
        /// </param>
        /// <param name="rotation">
        /// World rotation of the entity.
        /// </param>
        public SpawnPlacement(
            Vector3 position,
            Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}