using System;

namespace Chaosbound.Gameplay.Combat.Runtime.Composition
{
    /// <summary>
    /// Represents the current materialized runtime state
    /// of a single enemy variant within combat.
    /// </summary>
    public sealed class CombatRuntimeCompositionEntry
    {
        /// <summary>
        /// Gets the enemy variant represented by this entry.
        /// </summary>
        public EnemyVariantData Variant { get; }

        /// <summary>
        /// Gets the current number of alive enemies
        /// for this variant.
        /// </summary>
        public int AliveCount { get; private set; }

        public CombatRuntimeCompositionEntry(
            EnemyVariantData variant,
            int aliveCount)
        {
            Variant =
                variant
                ?? throw new ArgumentNullException(
                    nameof(variant));

            ValidateAliveCount(aliveCount);

            AliveCount = aliveCount;
        }

        public void Increment()
        {
            AliveCount++;
        }

        public void Decrement()
        {
            if (AliveCount == 0)
            {
                throw new InvalidOperationException(
                    "Cannot decrement an empty runtime composition entry.");
            }

            AliveCount--;
        }

        private static void ValidateAliveCount(
            int aliveCount)
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