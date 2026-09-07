using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Reference.Models
{
    /// <summary>
    /// Represents an immutable spatial origin used
    /// during Spawn Runtime execution.
    /// </summary>
    public readonly struct SpawnSpatialOrigin
    {
        /// <summary>
        /// Gets the world-space position of the origin.
        /// </summary>
        public Vector3 Position { get; }

        public SpawnSpatialOrigin(
            Vector3 position)
        {
            Position =
                position;
        }
    }
}