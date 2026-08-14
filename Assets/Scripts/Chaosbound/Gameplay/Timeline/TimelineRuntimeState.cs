using Chaosbound.Content.Expeditions.Definitions.Timeline;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Timeline
{
    /// <summary>
    /// Represents the mutable runtime state of the
    /// expedition timeline.
    /// </summary>
    public sealed class TimelineRuntimeState
    {
        /// <summary>
        /// Gets the index of the next timeline entry
        /// that has not yet been reached.
        /// </summary>
        public int NextEntryIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets whether the expedition completion target
        /// has been reached.
        /// </summary>
        public bool CompletionTargetReached
        {
            get;
            private set;
        }

        /// <summary>
        /// Advances the next pending timeline entry index.
        /// </summary>
        public void AdvanceEntry()
        {
            NextEntryIndex++;
        }

        /// <summary>
        /// Marks the expedition completion target as reached.
        /// </summary>
        public void MarkCompletionTargetReached()
        {
            CompletionTargetReached = true;
        }

        /// <summary>
        /// Gets the timeline entries reached during
        /// the latest runtime tick.
        /// </summary>
        public IReadOnlyList<TimelineEntry>
            ReachedEntriesThisTick =>
            reachedEntriesThisTick;

        /// <summary>
        /// Gets whether the completion target was reached
        /// during the latest runtime tick.
        /// </summary>
        public bool CompletionTargetReachedThisTick
        {
            get;
            private set;
        }

        private IReadOnlyList<TimelineEntry>
            reachedEntriesThisTick =
                Array.Empty<TimelineEntry>();

        /// <summary>
        /// Updates the transient results produced during
        /// the latest timeline evaluation.
        /// </summary>
        public void SetEvaluation(
            IReadOnlyList<TimelineEntry> reachedEntries,
            bool completionTargetReached)
        {
            if (reachedEntries == null)
                throw new ArgumentNullException(
                    nameof(reachedEntries));

            reachedEntriesThisTick =
                reachedEntries;

            CompletionTargetReachedThisTick =
                completionTargetReached;
        }

        /// <summary>
        /// Clears the transient results from the previous
        /// runtime tick.
        /// </summary>
        public void ClearEvaluation()
        {
            reachedEntriesThisTick =
                Array.Empty<TimelineEntry>();

            CompletionTargetReachedThisTick =
                false;
        }
    }
}