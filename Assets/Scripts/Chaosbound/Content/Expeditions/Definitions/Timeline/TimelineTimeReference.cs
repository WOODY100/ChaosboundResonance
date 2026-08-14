using System;

namespace Chaosbound.Content.Expeditions.Definitions.Timeline
{
    public enum TimelineTimeReferenceType
    {
        Fixed = 0,
        CompletionTarget = 1
    }

    /// <summary>
    /// Declarative reference to a temporal position in the expedition.
    /// </summary>
    public sealed class TimelineTimeReference
    {
        public TimelineTimeReferenceType Type { get; }

        public float TimeSeconds { get; }

        private TimelineTimeReference(
            TimelineTimeReferenceType type,
            float timeSeconds)
        {
            if (type == TimelineTimeReferenceType.Fixed &&
                timeSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(timeSeconds),
                    "Timeline time cannot be negative.");
            }

            Type = type;
            TimeSeconds = timeSeconds;
        }

        public static TimelineTimeReference Fixed(float timeSeconds)
        {
            return new TimelineTimeReference(
                TimelineTimeReferenceType.Fixed,
                timeSeconds);
        }

        public static TimelineTimeReference CompletionTarget()
        {
            return new TimelineTimeReference(
                TimelineTimeReferenceType.CompletionTarget,
                0f);
        }
    }
}