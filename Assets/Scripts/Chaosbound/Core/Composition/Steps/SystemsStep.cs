using System;

namespace Chaosbound.Core.Composition.Steps
{
    /// <summary>
    /// Composes gameplay systems.
    /// </summary>
    public sealed class SystemsStep : ICompositionStep
    {
        public void Execute(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Create gameplay systems.
            // Register created systems.
        }
    }
}