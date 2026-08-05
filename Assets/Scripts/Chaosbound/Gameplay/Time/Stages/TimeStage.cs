using System;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Contracts;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Time.Stages
{
    /// <summary>
    /// Advances the expedition runtime clock.
    /// </summary>
    public sealed class TimeStage :
        IExpeditionRuntimeStage
    {
        private readonly ITimeProvider
            timeProvider;

        /// <summary>
        /// Creates a new Time Stage.
        /// </summary>
        public TimeStage(
            ITimeProvider timeProvider)
        {
            this.timeProvider =
                timeProvider
                ?? throw new ArgumentNullException(
                    nameof(timeProvider));
        }

        /// <inheritdoc/>
        public bool ShouldExecute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            return true;
        }

        /// <inheritdoc/>
        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            context.State.AdvanceTime(
                timeProvider.DeltaTime);
        }
    }
}