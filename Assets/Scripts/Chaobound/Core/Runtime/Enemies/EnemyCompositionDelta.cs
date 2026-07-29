using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents a modification to an enemy composition.
    /// </summary>
    public sealed class EnemyCompositionDelta
    {
        /// <summary>
        /// Gets the operation to perform.
        /// </summary>
        public CompositionOperation Operation
        {
            get;
        }

        /// <summary>
        /// Gets the enemy candidate associated with the operation.
        /// </summary>
        public EnemyVariantData Candidate
        {
            get;
        }

        /// <summary>
        /// Initializes a new composition delta.
        /// </summary>
        /// <param name="operation">
        /// The composition operation.
        /// </param>
        /// <param name="candidate">
        /// The selected enemy candidate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the candidate is null.
        /// </exception>
        public EnemyCompositionDelta(
            CompositionOperation operation,
            EnemyVariantData candidate)
        {
            Candidate = candidate
                ?? throw new ArgumentNullException(nameof(candidate));

            Operation = operation;
        }
    }
}