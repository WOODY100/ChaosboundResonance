using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.Timeline
{
    /// <summary>
    /// Immutable materialized agenda generated from timeline content.
    /// </summary>
    public sealed class TimelineAgenda
    {
        private readonly IReadOnlyList<TimelineEntry> m_Entries;

        public IReadOnlyList<TimelineEntry> Entries =>
            m_Entries;

        public float CompletionTargetTime { get; }

        public TimelineAgenda(
            IReadOnlyList<TimelineEntry> entries,
            float completionTargetTime)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            if (completionTargetTime <= 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(completionTargetTime),
                    "Completion target time must be greater than zero.");

            List<TimelineEntry> copy =
                new(entries.Count);

            foreach (TimelineEntry entry in entries)
            {
                if (entry == null)
                {
                    throw new InvalidOperationException(
                        "TimelineAgenda cannot contain a null TimelineEntry.");
                }

                copy.Add(entry);
            }

            m_Entries =
                new List<TimelineEntry>(copy).AsReadOnly();

            CompletionTargetTime = completionTargetTime;
        }
    }
}