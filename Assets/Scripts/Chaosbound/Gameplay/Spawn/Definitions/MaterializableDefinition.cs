using System;
using Chaosbound.Shared.Contracts;

namespace Chaosbound.Gameplay.Spawn.Definitions
{
    /// <summary>
    /// Describes the content that should be materialized by a spawn job.
    /// </summary>
    public sealed class MaterializableDefinition : IDefinition
    {
        /// <summary>
        /// Gets the materializable reference.
        /// </summary>
        public IMaterializableReference Reference { get; }

        /// <summary>
        /// Initializes a new materializable definition.
        /// </summary>
        /// <param name="reference">
        /// Reference to the content that will be materialized.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the reference is null.
        /// </exception>
        public MaterializableDefinition(IMaterializableReference reference)
        {
            Reference = reference ?? throw new ArgumentNullException(nameof(reference));
        }
    }
}