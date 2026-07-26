using System;

namespace Chaosbound.Core.Composition.Validation
{
    /// <summary>
    /// Validates the composed world.
    /// </summary>
    public sealed class WorldValidation : ICompositionValidation
    {
        public void Validate(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Validate world runtime objects.
        }
    }
}