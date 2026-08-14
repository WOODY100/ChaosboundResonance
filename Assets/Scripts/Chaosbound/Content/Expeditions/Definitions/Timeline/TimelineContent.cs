using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.Timeline
{
    /// <summary>
    /// Complete declarative timeline content for an expedition.
    /// </summary>
    public sealed class TimelineContent
    {
        public IReadOnlyList<TimelineEventDefinition> EventDefinitions { get; }

        public IReadOnlyList<FixedTimeRule> FixedTimeRules { get; }

        public IReadOnlyList<DistributedRule> DistributedRules { get; }

        public IReadOnlyList<ExplicitTimelineEvent> ExplicitEvents { get; }

        public ExpeditionCompletionTarget CompletionTarget { get; }

        public TimelineContent(
            IReadOnlyList<TimelineEventDefinition> eventDefinitions,
            IReadOnlyList<FixedTimeRule> fixedTimeRules,
            IReadOnlyList<DistributedRule> distributedRules,
            IReadOnlyList<ExplicitTimelineEvent> explicitEvents,
            ExpeditionCompletionTarget completionTarget)
        {
            if (eventDefinitions == null)
                throw new ArgumentNullException(nameof(eventDefinitions));

            if (fixedTimeRules == null)
                throw new ArgumentNullException(nameof(fixedTimeRules));

            if (distributedRules == null)
                throw new ArgumentNullException(nameof(distributedRules));

            if (explicitEvents == null)
                throw new ArgumentNullException(nameof(explicitEvents));

            if (completionTarget == null)
                throw new ArgumentNullException(nameof(completionTarget));

            EventDefinitions = eventDefinitions;
            FixedTimeRules = fixedTimeRules;
            DistributedRules = distributedRules;
            ExplicitEvents = explicitEvents;
            CompletionTarget = completionTarget;
        }
    }
}