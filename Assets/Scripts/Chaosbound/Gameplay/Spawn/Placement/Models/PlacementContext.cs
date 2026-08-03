using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Placement.Models
{
    /// <summary>
    /// Represents the immutable context required
    /// to resolve a placement.
    /// </summary>
    public sealed class PlacementContext
    {
        /// <summary>
        /// Gets the placement intent.
        /// </summary>
        public PlacementIntent Intent { get; }

        /// <summary>
        /// Gets the world-space reference transform
        /// used during placement resolution.
        /// </summary>
        public Transform Reference { get; }

        /// <summary>
        /// Creates a new placement context.
        /// </summary>
        public PlacementContext(
            PlacementIntent intent,
            Transform reference)
        {
            Intent =
                intent
                ?? throw new ArgumentNullException(nameof(intent));

            Reference =
                reference
                ?? throw new ArgumentNullException(nameof(reference));
        }
    }
}