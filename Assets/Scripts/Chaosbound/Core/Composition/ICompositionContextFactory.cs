using Chaosbound.Content.Expeditions.Runtime.Configs;

namespace Chaosbound.Core.Composition
{
    /// <summary>
    /// Creates composition contexts.
    /// </summary>
    public interface ICompositionContextFactory
    {
        CompositionContext Create(
            RunSession runSession,
            RuntimeExpeditionConfig runtimeConfig);
    }
}