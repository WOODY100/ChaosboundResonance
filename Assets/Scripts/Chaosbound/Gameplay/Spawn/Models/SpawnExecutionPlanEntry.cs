using System;
using Chaosbound.Gameplay.Spawn.Definitions;

namespace Chaosbound.Gameplay.Spawn.Models
{
    /// <summary>
    /// Represents a single executable entry within
    /// a SpawnExecutionPlan.
    /// </summary>
    public sealed class SpawnExecutionPlanEntry
    {
        /// <summary>
        /// Gets the materializable content.
        /// </summary>
        public MaterializableDefinition Materializable { get; }

        /// <summary>
        /// Gets the amount that should be executed.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Creates a new execution plan entry.
        /// </summary>
        public SpawnExecutionPlanEntry(
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