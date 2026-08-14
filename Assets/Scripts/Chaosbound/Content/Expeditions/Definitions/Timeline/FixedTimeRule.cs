using System;

namespace Chaosbound.Content.Expeditions.Definitions.Timeline
{
    /// <summary>
    /// Schedules one timeline event at a fixed point in time.
    /// </summary>
    public sealed class FixedTimeRule
    {
        public string EventId { get; }

        public float TimeSeconds { get; }

        public FixedTimeRule(
            string eventId,
            float timeSeconds)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                throw new ArgumentException(
                    "Event id cannot be null or empty.",
                    nameof(eventId));

            if (timeSeconds < 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(timeSeconds),
                    "Fixed timeline time cannot be negative.");

            EventId = eventId;
            TimeSeconds = timeSeconds;
        }
    }
}