using System;

namespace Chaosbound.Content.Expeditions.Definitions.Timeline
{
    /// <summary>
    /// Declarative definition of a moment represented on the expedition timeline.
    /// </summary>
    public sealed class TimelineEventDefinition
    {
        public string Id { get; }

        public string IconId { get; }

        public TimelineTriggerReference TriggerReference { get; }

        public TimelineEventDefinition(
            string id,
            string iconId,
            TimelineTriggerReference triggerReference)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(
                    "Timeline event id cannot be null or empty.",
                    nameof(id));

            if (string.IsNullOrWhiteSpace(iconId))
                throw new ArgumentException(
                    "Timeline event icon id cannot be null or empty.",
                    nameof(iconId));

            Id = id;
            IconId = iconId;
            TriggerReference = triggerReference;
        }
    }
}