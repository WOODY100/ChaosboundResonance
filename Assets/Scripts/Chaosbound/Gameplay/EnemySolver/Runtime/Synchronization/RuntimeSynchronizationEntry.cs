using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents a single synchronization action required to align
    /// the runtime composition with the desired composition.
    /// </summary>
    public sealed class RuntimeSynchronizationEntry
    {
        /// <summary>
        /// Gets the enemy variant.
        /// </summary>
        public EnemyVariantData Variant { get; }

        /// <summary>
        /// Gets the synchronization operation.
        /// </summary>
        public SynchronizationOperationType Operation { get; }

        /// <summary>
        /// Gets the amount involved in the synchronization.
        /// </summary>
        public int Amount { get; }

        public RuntimeSynchronizationEntry(
            EnemyVariantData variant,
            SynchronizationOperationType operation,
            int amount)
        {
            Variant = variant ?? throw new ArgumentNullException(nameof(variant));

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Synchronization amount must be greater than zero.");
            }

            Operation = operation;
            Amount = amount;
        }
    }
}