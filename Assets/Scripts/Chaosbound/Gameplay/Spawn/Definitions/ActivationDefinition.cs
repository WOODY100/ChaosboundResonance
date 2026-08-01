using System;
using Chaosbound.Shared.Contracts;
using Chaosbound.Gameplay.Spawn.Contracts;

namespace Chaosbound.Gameplay.Spawn.Definitions
{
    /// <summary>
    /// Describes when a spawn job may become active.
    /// </summary>
    public sealed class ActivationDefinition : IDefinition
    {
        /// <summary>
        /// Gets the declarative activation strategy for the spawn job.
        /// </summary>
        public IActivationReference Activation { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationDefinition"/> class.
        /// </summary>
        /// <param name="activation">
        /// Declarative activation strategy.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="activation"/> is null.
        /// </exception>
        public ActivationDefinition(IActivationReference activation)
        {
            Activation = activation ?? throw new ArgumentNullException(nameof(activation));
        }
    }
}