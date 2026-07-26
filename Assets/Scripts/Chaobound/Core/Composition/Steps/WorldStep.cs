using System;

namespace Chaosbound.Core.Composition.Steps
{
    /// <summary>
    /// Composes the expedition world.
    /// </summary>
    public sealed class WorldStep : ICompositionStep
    {
        public void Execute(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Obtain RuntimeWorldConfig.
            // World generation will be delegated to WorldGenerator.
            // Generated objects will be registered in the CompositionContext.
        }
    }
}