using System;

namespace Chaosbound.Core.Composition.Validation
{
    /// <summary>
    /// Validates composed gameplay systems.
    /// </summary>
    public sealed class SystemsValidation : ICompositionValidation
    {
        public void Validate(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Validate gameplay systems.
        }
    }
}