using System;

namespace Chaosbound.Core.Composition.Validation
{
    /// <summary>
    /// Validates the composed player.
    /// </summary>
    public sealed class PlayerValidation : ICompositionValidation
    {
        public void Validate(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Validate player runtime objects.
        }
    }
}