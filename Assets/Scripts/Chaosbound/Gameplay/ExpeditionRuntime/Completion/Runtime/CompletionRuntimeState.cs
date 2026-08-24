using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Runtime
{
    /// <summary>
    /// Mutable runtime state owned by the Completion Domain.
    /// </summary>
    public sealed class CompletionRuntimeState
    {
        /// <summary>
        /// Gets the current lifecycle state of the Completion Domain.
        /// </summary>
        public CompletionDomainState State
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the completion requirement for the expedition.
        /// </summary>
        public CompletionRequirement Requirement
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the event that satisfied the completion requirement.
        /// </summary>
        public EventCompleted? CompletedEvent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the completed expedition result.
        /// </summary>
        public ExpeditionCompleted? CompletedExpedition
        {
            get;
            private set;
        }

        public CompletionRuntimeState()
        {
            State =
                CompletionDomainState.Inactive;

            CompletedEvent =
                null;

            CompletedExpedition =
                null;
        }

        /// <summary>
        /// Activates the Completion Domain with
        /// the configured expedition requirement.
        /// </summary>
        public void Start(
            CompletionRequirement requirement)
        {
            Requirement =
                requirement;

            CompletedEvent =
                null;

            CompletedExpedition =
                null;

            State =
                CompletionDomainState.Waiting;
        }

        /// <summary>
        /// Marks the completion requirement as satisfied.
        /// </summary>
        public void Complete(
            EventCompleted completedEvent)
        {
            CompletedEvent =
                completedEvent;

            CompletedExpedition =
                new ExpeditionCompleted(
                    completedEvent.Origin);

            State =
                CompletionDomainState.Completed;
        }
    }
}