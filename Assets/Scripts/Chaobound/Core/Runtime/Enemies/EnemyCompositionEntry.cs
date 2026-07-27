using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the participation of an enemy variant within the target ecosystem composition.
    /// </summary>
    public sealed class EnemyCompositionEntry
    {
        /// <summary>
        /// Gets the enemy variant represented by this entry.
        /// </summary>
        public EnemyVariantData Variant { get; }

        /// <summary>
        /// Gets the target quantity for this variant.
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        /// Creates a new composition entry.
        /// </summary>
        public EnemyCompositionEntry(
            EnemyVariantData variant,
            int quantity)
        {
            Variant = variant ?? throw new ArgumentNullException(nameof(variant));

            ValidateQuantity(quantity);

            Quantity = quantity;
        }

        /// <summary>
        /// Updates the target quantity for this entry.
        /// </summary>
        public void UpdateQuantity(int quantity)
        {
            ValidateQuantity(quantity);

            Quantity = quantity;
        }

        /// <summary>
        /// Validates that the specified quantity is valid for a composition entry.
        /// </summary>
        private static void ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Quantity must be greater than zero.");
            }
        }
    }
}