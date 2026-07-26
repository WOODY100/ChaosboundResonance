using System;

namespace Chaosbound.Core.Composition.Steps
{
    /// <summary>
    /// Composes the expedition player.
    /// </summary>
    public sealed class PlayerStep : ICompositionStep
    {
        public void Execute(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Obtain runtime player configuration.
            // Player creation will be delegated to PlayerGenerator.
            // Register player in CompositionContext.
        }
    }
}