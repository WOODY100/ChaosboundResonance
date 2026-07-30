using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Applies composition deltas to the current enemy composition.
    /// </summary>
    public sealed class CompositionApplier
    {
        /// <summary>
        /// Applies the specified delta to the current composition.
        /// </summary>
        /// <param name="composition">
        /// The composition to modify.
        /// </param>
        /// <param name="delta">
        /// The composition delta to apply.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any argument is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the operation is not supported.
        /// </exception>
        public void Apply(
            EnemyComposition composition,
            EnemyCompositionDelta delta)
        {
            if (composition == null)
                throw new ArgumentNullException(nameof(composition));

            if (delta == null)
                throw new ArgumentNullException(nameof(delta));

            switch (delta.Operation)
            {
                case CompositionOperation.Add:
                    ApplyAdd(composition, delta);
                    break;

                case CompositionOperation.Remove:
                    ApplyRemove(composition, delta);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported composition operation '{delta.Operation}'.");
            }
        }

        private static void ApplyRemove(
            EnemyComposition composition,
            EnemyCompositionDelta delta)
        {
            throw new NotImplementedException();
        }

        private static void ApplyAdd(
            EnemyComposition composition,
            EnemyCompositionDelta delta)
        {
            if (composition.TryGetEntry(delta.Candidate, out EnemyCompositionEntry entry))
            {
                entry.UpdateQuantity(entry.Quantity + delta.Quantity);
                return;
            }

            composition.Add(
                new EnemyCompositionEntry(
                    delta.Candidate,
                    delta.Quantity));
        }
    }
}