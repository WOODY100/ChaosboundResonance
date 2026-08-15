using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.Timeline
{
    /// <summary>
    /// Schedules multiple different timeline events
    /// inside a temporal window.
    /// </summary>
    public sealed class DistributedRule
    {
        public IReadOnlyList<string> EventIds { get; }

        public float StartTimeSeconds { get; }

        public TimelineTimeReference EndTime { get; }

        public DistributedRule(
            IReadOnlyList<string> eventIds,
            float startTimeSeconds,
            TimelineTimeReference endTime)
        {
            if (eventIds == null)
                throw new ArgumentNullException(
                    nameof(eventIds));

            if (eventIds.Count == 0)
            {
                throw new ArgumentException(
                    "Distributed rule must contain at least one event id.",
                    nameof(eventIds));
            }

            HashSet<string> uniqueEventIds =
                new();

            foreach (string eventId in eventIds)
            {
                if (string.IsNullOrWhiteSpace(eventId))
                {
                    throw new ArgumentException(
                        "Distributed rule cannot contain an empty event id.",
                        nameof(eventIds));
                }

                if (!uniqueEventIds.Add(eventId))
                {
                    throw new ArgumentException(
                        $"Distributed rule contains duplicate event id " +
                        $"'{eventId}'.",
                        nameof(eventIds));
                }
            }

            if (startTimeSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(startTimeSeconds),
                    "Distributed start time cannot be negative.");
            }

            if (endTime == null)
                throw new ArgumentNullException(nameof(endTime));

            if (endTime.Type == TimelineTimeReferenceType.Fixed &&
                startTimeSeconds >= endTime.TimeSeconds)
            {
                throw new ArgumentException(
                    "Distributed start time must be less than the fixed end time.");
            }

            EventIds =
                new List<string>(
                    eventIds).AsReadOnly();

            StartTimeSeconds =
                startTimeSeconds;

            EndTime =
                endTime;
        }
    }
}