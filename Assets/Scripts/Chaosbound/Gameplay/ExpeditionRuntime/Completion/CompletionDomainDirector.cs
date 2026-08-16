using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Services;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion
{
    /// <summary>
    /// Coordinates Completion Domain runtime behavior
    /// for the current expedition.
    /// </summary>
    public sealed class CompletionDomainDirector
    {
        private readonly CompletionRequirementMatcher
            requirementMatcher;

        public CompletionDomainDirector(
            CompletionRequirementMatcher requirementMatcher)
        {
            this.requirementMatcher =
                requirementMatcher
                ?? throw new ArgumentNullException(
                    nameof(requirementMatcher));
        }

        /// <summary>
        /// Executes Completion Domain behavior for the
        /// current expedition runtime tick.
        /// </summary>
        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            if (context.Config.Completion == null)
            {
                throw new InvalidOperationException(
                    "Completion Domain requires " +
                    "RuntimeCompletionConfig.");
            }

            if (context.State.Completion.State ==
                CompletionDomainState.Inactive)
            {
                context.State.Completion.Start(
                    context.Config.Completion.Requirement);
            }

            if (context.State.Completion.State !=
                CompletionDomainState.Waiting)
            {
                context.State.EventCompletions.Clear();
                return;
            }

            try
            {
                foreach (EventCompleted completedEvent in
                    context.State.EventCompletions.Events)
                {
                    bool matches =
                        requirementMatcher.Matches(
                            context.State.Completion.Requirement,
                            completedEvent);

                    if (!matches)
                        continue;

                    context.State.Completion.Complete(
                        completedEvent);

                    break;
                }
            }
            finally
            {
                context.State.EventCompletions.Clear();
            }
        }
    }
}