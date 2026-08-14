using Chaosbound.Content.Expeditions.Definitions.Timeline;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Timeline
{
    /// <summary>
    /// Represents the transient result of evaluating
    /// the expedition timeline during one runtime tick.
    /// </summary>
    public sealed class TimelineEvaluation
    {
        private readonly IReadOnlyList<TimelineEntry>
            reachedEntries;

        /// <summary>
        /// Gets the timeline entries reached during
        /// this evaluation.
        /// </summary>
        public IReadOnlyList<TimelineEntry> ReachedEntries =>
            reachedEntries;

        /// <summary>
        /// Gets whether the expedition completion target
        /// was reached during this evaluation.
        /// </summary>
        public bool CompletionTargetReached { get; }

        public TimelineEvaluation(
            IReadOnlyList<TimelineEntry> entries,
            bool completionTargetReached)
        {
            if (entries == null)
                throw new ArgumentNullException(
                    nameof(entries));

            List<TimelineEntry> copy =
                new(entries.Count);

            foreach (TimelineEntry entry in entries)
            {
                if (entry == null)
                {
                    throw new InvalidOperationException(
                        "TimelineEvaluation cannot contain " +
                        "a null TimelineEntry.");
                }

                copy.Add(entry);
            }

            reachedEntries =
                new List<TimelineEntry>(copy).AsReadOnly();

            CompletionTargetReached =
                completionTargetReached;
        }
    }
}