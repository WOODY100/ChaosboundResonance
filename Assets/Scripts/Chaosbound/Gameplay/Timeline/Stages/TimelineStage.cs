using Chaosbound.Content.Expeditions.Definitions.Timeline;
using Chaosbound.Content.Expeditions.Runtime.Timeline;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Timeline.Stages
{
    /// <summary>
    /// Executes the Timeline Domain for the current
    /// Expedition Runtime tick.
    /// </summary>
    public sealed class TimelineStage :
        IExpeditionRuntimeStage
    {
        private readonly TimelineRuntime
            timelineRuntime;

        public TimelineStage(
            TimelineRuntime timelineRuntime)
        {
            this.timelineRuntime =
                timelineRuntime
                ?? throw new ArgumentNullException(
                    nameof(timelineRuntime));
        }

        /// <inheritdoc/>
        public bool ShouldExecute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(
                    nameof(context));
            }

            RuntimeTimelineConfig timelineConfig =
                context.Config.Timeline;

            if (timelineConfig == null)
                return false;

            if (timelineConfig.Agenda == null)
                return false;

            return true;
        }

        /// <inheritdoc/>
        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(
                    nameof(context));
            }

            RuntimeTimelineConfig timelineConfig =
                context.Config.Timeline;

            if (timelineConfig == null)
            {
                throw new InvalidOperationException(
                    "TimelineStage requires " +
                    "RuntimeTimelineConfig.");
            }

            if (timelineConfig.Agenda == null)
            {
                throw new InvalidOperationException(
                    "TimelineStage requires a TimelineAgenda.");
            }

            TimelineRuntimeState timelineState =
                context.State.Timeline;

            timelineState.ClearEvaluation();

            TimelineEvaluation evaluation =
                timelineRuntime.Evaluate(
                    timelineConfig.Agenda,
                    context.State.ElapsedTime,
                    timelineState);

            timelineState.SetEvaluation(
                evaluation.ReachedEntries,
                evaluation.CompletionTargetReached);
        }
    }
}