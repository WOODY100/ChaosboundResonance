using System;

namespace Chaosbound.Content.Expeditions.Definitions.Timeline
{
    /// <summary>
    /// Schedules multiple occurrences of one timeline event
    /// inside a temporal window.
    /// </summary>
    public sealed class DistributedRule
    {
        public string EventId { get; }

        public int Count { get; }

        public float StartTimeSeconds { get; }

        public TimelineTimeReference EndTime { get; }

        public DistributedRule(
            string eventId,
            int count,
            float startTimeSeconds,
            TimelineTimeReference endTime)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                throw new ArgumentException(
                    "Event id cannot be null or empty.",
                    nameof(eventId));

            if (count < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "Distributed rule count must be at least one.");

            if (startTimeSeconds < 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(startTimeSeconds),
                    "Distributed start time cannot be negative.");

            if (endTime == null)
                throw new ArgumentNullException(nameof(endTime));

            if (endTime.Type == TimelineTimeReferenceType.Fixed &&
                startTimeSeconds >= endTime.TimeSeconds)
            {
                throw new ArgumentException(
                    "Distributed start time must be less than the fixed end time.");
            }

            EventId = eventId;
            Count = count;
            StartTimeSeconds = startTimeSeconds;
            EndTime = endTime;
        }
    }
}