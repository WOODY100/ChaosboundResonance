using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion
{
    /// <summary>
    /// Runtime pipeline stage responsible for executing
    /// the Completion Domain.
    /// </summary>
    public sealed class CompletionStage :
        IExpeditionRuntimeStage
    {
        private readonly CompletionDomainDirector
            director;

        public CompletionStage(
            CompletionDomainDirector director)
        {
            this.director =
                director
                ?? throw new ArgumentNullException(
                    nameof(director));
        }

        /// <summary>
        /// Determines whether the Completion Domain
        /// should execute during the current tick.
        /// </summary>
        public bool ShouldExecute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            return
                context.State.Completion.State !=
                CompletionDomainState.Completed;
        }

        /// <summary>
        /// Executes the Completion Domain.
        /// </summary>
        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            director.Execute(
                context);
        }
    }
}