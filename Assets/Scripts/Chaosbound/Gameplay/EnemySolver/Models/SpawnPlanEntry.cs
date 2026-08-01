using System;

namespace Chaosbound.Gameplay.EnemySolver.Models
{
    /// <summary>
    /// Represents the budget allocation result for a single enemy variant.
    ///
    /// The desired quantity comes from the target enemy composition,
    /// while the allocated quantity represents how many instances can
    /// currently be sustained by the available threat budget.
    /// </summary>
    public sealed class SpawnPlanEntry
    {
        /// <summary>
        /// Gets the enemy variant represented by this entry.
        /// </summary>
        public EnemyVariantData Variant { get; }

        /// <summary>
        /// Gets the desired quantity defined by the target composition.
        /// This value is always greater than zero.
        /// </summary>
        public int DesiredQuantity { get; }

        /// <summary>
        /// Gets the quantity that can currently be allocated.
        /// This value may be zero if there is not enough threat budget.
        /// </summary>
        public int AllocatedQuantity { get; }

        /// <summary>
        /// Gets how many instances are still pending allocation.
        /// </summary>
        public int PendingQuantity =>
            DesiredQuantity - AllocatedQuantity;

        /// <summary>
        /// Creates a new spawn plan entry.
        /// </summary>
        public SpawnPlanEntry(
            EnemyVariantData variant,
            int desiredQuantity,
            int allocatedQuantity)
        {
            Variant = variant
                ?? throw new ArgumentNullException(nameof(variant));

            ValidateDesiredQuantity(desiredQuantity);
            ValidateAllocatedQuantity(allocatedQuantity);

            if (allocatedQuantity > desiredQuantity)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(allocatedQuantity),
                    allocatedQuantity,
                    "Allocated quantity cannot exceed the desired quantity.");
            }

            DesiredQuantity = desiredQuantity;
            AllocatedQuantity = allocatedQuantity;
        }

        /// <summary>
        /// Validates the desired quantity.
        /// A spawn plan entry must always represent at least one desired enemy.
        /// </summary>
        private static void ValidateDesiredQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Desired quantity must be greater than zero.");
            }
        }

        /// <summary>
        /// Validates the allocated quantity.
        /// The allocated quantity may be zero but can never be negative.
        /// </summary>
        private static void ValidateAllocatedQuantity(int quantity)
        {
            if (quantity < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Allocated quantity cannot be negative.");
            }
        }
    }
}