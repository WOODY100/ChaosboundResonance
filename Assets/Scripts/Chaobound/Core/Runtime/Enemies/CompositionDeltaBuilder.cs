using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Builds the final enemy composition modification.
    /// </summary>
    public sealed class CompositionDeltaBuilder
    {
        /// <summary>
        /// Builds an enemy composition delta from the selected candidate.
        /// </summary>
        /// <param name="evaluation">
        /// The selected candidate evaluation.
        /// </param>
        /// <returns>
        /// The resulting composition delta.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the evaluation is null.
        /// </exception>
        public EnemyCompositionDelta Build(
            CompositionSynchronizationEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            return new EnemyCompositionDelta(
                CompositionOperation.Add,
                entry.Variant,
                entry.Amount);
        }
    }
}