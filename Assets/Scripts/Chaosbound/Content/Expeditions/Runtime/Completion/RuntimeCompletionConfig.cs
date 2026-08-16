using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;

namespace Chaosbound.Content.Expeditions.Runtime.Completion
{
    /// <summary>
    /// Immutable runtime configuration for expedition completion.
    /// </summary>
    public sealed class RuntimeCompletionConfig
    {
        /// <summary>
        /// Gets the requirement that completes the expedition.
        /// </summary>
        public CompletionRequirement Requirement
        {
            get;
        }

        public RuntimeCompletionConfig(
            CompletionRequirement requirement)
        {
            Requirement =
                requirement;
        }
    }
}