using System;
using Chaosbound.Gameplay.Spawn.Definitions;

namespace Chaosbound.Gameplay.Spawn.Contracts
{
    /// <summary>
    /// Represents a single materialization request contained within a
    /// SpawnRequest.
    ///
    /// Each entry declares WHAT should be materialized and HOW MANY
    /// instances are requested.
    /// </summary>
    public sealed class SpawnRequestEntry
    {
        /// <summary>
        /// Gets the requested materializable definition.
        /// </summary>
        public MaterializableDefinition Materializable { get; }

        /// <summary>
        /// Gets the requested quantity.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Creates a new spawn request entry.
        /// </summary>
        /// <param name="materializable">
        /// Materializable definition.
        /// </param>
        /// <param name="quantity">
        /// Requested quantity.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the materializable is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the quantity is not greater than zero.
        /// </exception>
        public SpawnRequestEntry(
            MaterializableDefinition materializable,
            int quantity)
        {
            Materializable = materializable
                ?? throw new ArgumentNullException(nameof(materializable));

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Quantity must be greater than zero.");
            }

            Quantity = quantity;
        }
    }
}