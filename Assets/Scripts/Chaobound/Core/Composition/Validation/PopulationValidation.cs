using System;

namespace Chaosbound.Core.Composition.Validation
{
    /// <summary>
    /// Validates the composed population.
    /// </summary>
    public sealed class PopulationValidation : ICompositionValidation
    {
        public void Validate(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Validate population runtime objects.
        }
    }
}