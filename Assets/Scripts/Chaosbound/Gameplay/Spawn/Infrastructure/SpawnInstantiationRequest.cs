using System;
using Chaosbound.Shared.Contracts;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Infrastructure
{
    /// <summary>
    /// Represents a request to instantiate
    /// gameplay content into the world.
    /// </summary>
    public sealed class SpawnInstantiationRequest
    {
        /// <summary>
        /// Gets the materializable content.
        /// </summary>
        public IMaterializableReference Reference { get; }

        /// <summary>
        /// Gets the world position where the
        /// entity should be instantiated.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Gets the world rotation that the
        /// entity should use when instantiated.
        /// </summary>
        public Quaternion Rotation { get; }

        /// <summary>
        /// Creates a new spawn instantiation request.
        /// </summary>
        public SpawnInstantiationRequest(
            IMaterializableReference reference,
            Vector3 position,
            Quaternion rotation)
        {
            Reference =
                reference
                ?? throw new ArgumentNullException(
                    nameof(reference));

            Position = position;
            Rotation = rotation;
        }
    }
}