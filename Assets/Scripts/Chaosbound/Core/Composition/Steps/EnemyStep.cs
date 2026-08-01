using System;

namespace Chaosbound.Core.Composition.Steps
{
    /// <summary>
    /// Composes the expedition enemys.
    /// </summary>
    public sealed class EnemyStep : ICompositionStep
    {
        public void Execute(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Obtain RuntimeEnemyConfig.
            // Enemy generation will be delegated to EnemyGenerator.
            // Register generated enemy.
        }
    }
}