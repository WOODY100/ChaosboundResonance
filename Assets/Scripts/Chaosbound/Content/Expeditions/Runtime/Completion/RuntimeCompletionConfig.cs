using Chaosbound.Content.Portal.Exit;
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

        /// <summary>
        /// Gets the Exit Portal content that must be
        /// materialized when the expedition completes.
        /// </summary>
        public ExitPortalData ExitPortal
        {
            get;
        }

        public RuntimeCompletionConfig(
            CompletionRequirement requirement,
            ExitPortalData exitPortal)
        {
            Requirement =
                requirement;

            ExitPortal =
                exitPortal;
        }
    }
}