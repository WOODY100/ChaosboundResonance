using System;
using Chaosbound.Gameplay.Spawn.Definitions;

namespace Chaosbound.Gameplay.Spawn.Models
{
    /// <summary>
    /// Represents the immediate execution entry
    /// produced by the Spawn Runtime.
    /// </summary>
    public sealed class SpawnTaskEntry
    {
        /// <summary>
        /// Gets the materializable content.
        /// </summary>
        public MaterializableDefinition Materializable { get; }

        /// <summary>
        /// Gets the quantity to materialize immediately.
        /// </summary>
        public int Quantity { get; }

        public SpawnTaskEntry(
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