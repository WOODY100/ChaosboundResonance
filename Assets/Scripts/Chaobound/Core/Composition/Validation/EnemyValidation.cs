using System;

namespace Chaosbound.Core.Composition.Validation
{
    /// <summary>
    /// Validates the composed enemy.
    /// </summary>
    public sealed class EnemyValidation : ICompositionValidation
    {
        public void Validate(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Validate enemy runtime objects.
        }
    }
}