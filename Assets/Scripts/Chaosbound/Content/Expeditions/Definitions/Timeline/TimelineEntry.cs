using System;

namespace Chaosbound.Content.Expeditions.Definitions.Timeline
{
    /// <summary>
    /// Represents one materialized event on the expedition timeline.
    /// </summary>
    public sealed class TimelineEntry
    {
        public string EntryId { get; }

        public string EventId { get; }

        public float ScheduledTime { get; }

        public string IconId { get; }

        public TimelineTriggerReference TriggerReference { get; }

        public TimelineEntry(
            string entryId,
            string eventId,
            float scheduledTime,
            string iconId,
            TimelineTriggerReference triggerReference)
        {
            if (string.IsNullOrWhiteSpace(entryId))
                throw new ArgumentException(
                    "Entry id cannot be null or empty.",
                    nameof(entryId));

            if (string.IsNullOrWhiteSpace(eventId))
                throw new ArgumentException(
                    "Event id cannot be null or empty.",
                    nameof(eventId));

            if (scheduledTime < 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(scheduledTime),
                    "Scheduled time cannot be negative.");

            if (string.IsNullOrWhiteSpace(iconId))
                throw new ArgumentException(
                    "Icon id cannot be null or empty.",
                    nameof(iconId));

            EntryId = entryId;
            EventId = eventId;
            ScheduledTime = scheduledTime;
            IconId = iconId;
            TriggerReference = triggerReference;
        }
    }
}