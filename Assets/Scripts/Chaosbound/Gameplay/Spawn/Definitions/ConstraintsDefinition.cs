using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Shared.Contracts;
using System;

namespace Chaosbound.Gameplay.Spawn.Definitions
{
    /// <summary>
    /// Describes the declarative constraints of a spawn job.
    /// </summary>
    public sealed class ConstraintsDefinition : IDefinition
    {
        /// <summary>
        /// Gets the declarative constraint reference for the spawn job.
        /// </summary>
        public IConstraintReference Constraints { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintsDefinition"/> class.
        /// </summary>
        /// <param name="constraints">
        /// Declarative constraint reference.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="constraints"/> is null.
        /// </exception>
        public ConstraintsDefinition(IConstraintReference constraints)
        {
            Constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));
        }
    }
}