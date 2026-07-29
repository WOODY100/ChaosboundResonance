using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the current runtime state of a single enemy variant
    /// within the materialized composition.
    /// </summary>
    public sealed class CompositionStateEntry
    {
        /// <summary>
        /// Gets the enemy variant represented by this entry.
        /// </summary>
        public EnemyVariantData Variant { get; }

        /// <summary>
        /// Gets the current number of alive enemies for this variant.
        /// </summary>
        public int AliveCount { get; private set; }

        /// <summary>
        /// Creates a new runtime composition entry.
        /// </summary>
        public CompositionStateEntry(
            EnemyVariantData variant,
            int aliveCount)
        {
            Variant = variant ?? throw new ArgumentNullException(nameof(variant));

            ValidateAliveCount(aliveCount);

            AliveCount = aliveCount;
        }

        /// <summary>
        /// Increments the alive count.
        /// </summary>
        public void Increment()
        {
            AliveCount++;
        }

        /// <summary>
        /// Decrements the alive count.
        /// </summary>
        public void Decrement()
        {
            if (AliveCount == 0)
            {
                throw new InvalidOperationException(
                    "Cannot decrement an empty composition state.");
            }

            AliveCount--;
        }

        private static void ValidateAliveCount(int aliveCount)
        {
            if (aliveCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(aliveCount),
                    aliveCount,
                    "Alive count cannot be negative.");
            }
        }
    }
}