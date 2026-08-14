using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Content.Expeditions.Definitions.Timeline;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Timeline
{
    /// <summary>
    /// Evaluates the expedition timeline against
    /// the current expedition elapsed time.
    /// </summary>
    public sealed class TimelineRuntime
    {
        /// <summary>
        /// Evaluates the timeline for the current runtime time.
        /// </summary>
        public TimelineEvaluation Evaluate(
            TimelineAgenda agenda,
            TimeSpan elapsedTime,
            TimelineRuntimeState runtimeState)
        {
            if (agenda == null)
                throw new ArgumentNullException(
                    nameof(agenda));

            if (runtimeState == null)
                throw new ArgumentNullException(
                    nameof(runtimeState));

            if (elapsedTime < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(elapsedTime),
                    "Elapsed time cannot be negative.");
            }

            float elapsedSeconds =
                (float)elapsedTime.TotalSeconds;

            List<TimelineEntry> reachedEntries =
                new();

            IReadOnlyList<TimelineEntry> entries =
                agenda.Entries;

            while (
                runtimeState.NextEntryIndex <
                entries.Count)
            {
                TimelineEntry entry =
                    entries[
                        runtimeState.NextEntryIndex];

                if (entry.ScheduledTime >
                    elapsedSeconds)
                {
                    break;
                }

                reachedEntries.Add(entry);

                runtimeState.AdvanceEntry();
            }

            bool completionTargetReached =
                false;

            if (!runtimeState.CompletionTargetReached &&
                elapsedSeconds >=
                agenda.CompletionTargetTime)
            {
                runtimeState.MarkCompletionTargetReached();

                completionTargetReached = true;
            }

            return new TimelineEvaluation(
                reachedEntries,
                completionTargetReached);
        }
    }
}