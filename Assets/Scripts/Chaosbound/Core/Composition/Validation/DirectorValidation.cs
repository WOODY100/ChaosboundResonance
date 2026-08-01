using System;

namespace Chaosbound.Core.Composition.Validation
{
    /// <summary>
    /// Validates the expedition director.
    /// </summary>
    public sealed class DirectorValidation : ICompositionValidation
    {
        public void Validate(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Validate director runtime objects.
        }
    }
}