using System;

namespace Chaosbound.Core.Composition.Steps
{
    /// <summary>
    /// Composes the expedition director.
    /// </summary>
    public sealed class DirectorStep : ICompositionStep
    {
        public void Execute(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Create ExpeditionDirector.
            // Register director in CompositionContext.
        }
    }
}