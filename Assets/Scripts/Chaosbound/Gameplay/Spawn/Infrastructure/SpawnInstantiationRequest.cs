using System;
using Chaosbound.Shared.Contracts;

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

        public SpawnInstantiationRequest(
            IMaterializableReference reference)
        {
            Reference =
                reference
                ?? throw new ArgumentNullException(
                    nameof(reference));
        }
    }
}