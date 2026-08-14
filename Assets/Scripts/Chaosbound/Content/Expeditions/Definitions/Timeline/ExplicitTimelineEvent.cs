using System;

namespace Chaosbound.Content.Expeditions.Definitions.Timeline
{
    /// <summary>
    /// Explicitly scheduled timeline event placed by the designer.
    /// </summary>
    public sealed class ExplicitTimelineEvent
    {
        public string EventId { get; }

        public float TimeSeconds { get; }

        public ExplicitTimelineEvent(
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
                    "Explicit timeline time cannot be negative.");

            EventId = eventId;
            TimeSeconds = timeSeconds;
        }
    }
}