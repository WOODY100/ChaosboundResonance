using Chaosbound.Content.Expeditions.Runtime.Configs;
using System;

namespace Chaosbound.Core.Composition
{
    /// <summary>
    /// Creates valid composition contexts.
    /// </summary>
    public sealed class CompositionContextFactory
        : ICompositionContextFactory
    {
        public CompositionContext Create(
            RunSession runSession,
            RuntimeExpeditionConfig runtimeConfig)
        {
            if (runSession == null)
                throw new ArgumentNullException(nameof(runSession));

            if (runtimeConfig == null)
                throw new ArgumentNullException(nameof(runtimeConfig));

            return new CompositionContext(
                runSession,
                runtimeConfig);
        }
    }
}