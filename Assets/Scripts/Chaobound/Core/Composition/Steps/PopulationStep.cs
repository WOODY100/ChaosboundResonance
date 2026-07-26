using System;

namespace Chaosbound.Core.Composition.Steps
{
    /// <summary>
    /// Composes the expedition population.
    /// </summary>
    public sealed class PopulationStep : ICompositionStep
    {
        public void Execute(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Obtain RuntimePopulationConfig.
            // Population generation will be delegated to PopulationGenerator.
            // Register generated population.
        }
    }
}