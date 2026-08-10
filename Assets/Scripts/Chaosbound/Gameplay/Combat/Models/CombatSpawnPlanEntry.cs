using System;

namespace Chaosbound.Gameplay.Combat.Models
{
    /// <summary>
    /// Represents a concrete enemy variant and the quantity
    /// requested for materialization by the Combat domain.
    /// </summary>
    public sealed class CombatSpawnPlanEntry
    {
        /// <summary>
        /// Gets the concrete enemy variant to materialize.
        /// </summary>
        public EnemyVariantData Variant { get; }

        /// <summary>
        /// Gets the requested quantity of this variant.
        /// </summary>
        public int Quantity { get; }

        public CombatSpawnPlanEntry(
            EnemyVariantData variant,
            int quantity)
        {
            Variant =
                variant
                ?? throw new ArgumentNullException(
                    nameof(variant));

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Combat spawn quantity must be greater than zero.");
            }

            Quantity = quantity;
        }
    }
}