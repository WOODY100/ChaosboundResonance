using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Services
{
    /// <summary>
    /// Determines whether a completed event
    /// satisfies a completion requirement.
    /// </summary>
    public sealed class CompletionRequirementMatcher
    {
        /// <summary>
        /// Determines whether the completed event
        /// matches the configured requirement.
        /// </summary>
        public bool Matches(
            CompletionRequirement requirement,
            EventCompleted completedEvent)
        {
            return
                requirement.DomainId ==
                completedEvent.DomainId
                &&
                requirement.EventId ==
                completedEvent.EventId;
        }
    }
}