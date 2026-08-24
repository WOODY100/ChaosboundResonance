using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Placement.Models
{
    /// <summary>
    /// Represents the physical capsule footprint required
    /// by a materializable entity during placement.
    /// </summary>
    public sealed class PlacementFootprint
    {
        /// <summary>
        /// Gets the local-space center of the capsule.
        /// </summary>
        public Vector3 Center { get; }

        /// <summary>
        /// Gets the capsule radius in local space.
        /// </summary>
        public float Radius { get; }

        /// <summary>
        /// Gets the capsule height in local space.
        /// </summary>
        public float Height { get; }

        public PlacementFootprint(
            Vector3 center,
            float radius,
            float height)
        {
            Center = center;

            Radius =
                Mathf.Max(
                    0.01f,
                    radius);

            Height =
                Mathf.Max(
                    0.01f,
                    height);
        }
    }
}