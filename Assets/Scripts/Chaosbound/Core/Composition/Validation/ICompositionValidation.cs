using System;

namespace Chaosbound.Core.Composition.Validation
{
    /// <summary>
    /// Validates a portion of the composed runtime.
    /// </summary>
    public interface ICompositionValidation
    {
        void Validate(CompositionContext context);
    }
}