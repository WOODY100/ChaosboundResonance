using Chaosbound.Content.Expeditions.Definitions.Timeline;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Directors.Timeline
{
    /// <summary>
    /// Materializes declarative timeline content into an immutable agenda.
    /// </summary>
    public static class TimelineDirector
    {
        public static TimelineAgenda Build(
            TimelineContent content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            float completionTargetTime =
                content.CompletionTarget.TimeSeconds;

            Dictionary<string, TimelineEventDefinition>
                eventDefinitions =
                BuildEventDefinitionMap(
                    content.EventDefinitions);

            List<TimelineEntry> entries =
                new();

            HashSet<string> scheduledEventIds =
                new();

            BuildFixedTimeEntries(
                content.FixedTimeRules,
                eventDefinitions,
                completionTargetTime,
                entries,
                scheduledEventIds);

            BuildDistributedEntries(
                content.DistributedRules,
                eventDefinitions,
                completionTargetTime,
                entries,
                scheduledEventIds);

            SortEntries(entries);

            return new TimelineAgenda(
                entries,
                completionTargetTime);
        }

        private static Dictionary<string, TimelineEventDefinition>
            BuildEventDefinitionMap(
                IReadOnlyList<TimelineEventDefinition> definitions)
        {
            if (definitions == null)
                throw new ArgumentNullException(nameof(definitions));

            Dictionary<string, TimelineEventDefinition> result =
                new();

            foreach (
                TimelineEventDefinition definition
                in definitions)
            {
                if (definition == null)
                {
                    throw new InvalidOperationException(
                        "TimelineContent contains a null " +
                        "TimelineEventDefinition.");
                }

                if (result.ContainsKey(definition.Id))
                {
                    throw new InvalidOperationException(
                        $"Timeline contains duplicate event id " +
                        $"'{definition.Id}'.");
                }

                result.Add(
                    definition.Id,
                    definition);
            }

            return result;
        }

        private static void BuildFixedTimeEntries(
            IReadOnlyList<FixedTimeRule> rules,
            IReadOnlyDictionary<string, TimelineEventDefinition>
                eventDefinitions,
            float completionTargetTime,
            List<TimelineEntry> entries,
            HashSet<string> scheduledEventIds)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));

            for (int index = 0; index < rules.Count; index++)
            {
                FixedTimeRule rule =
                    rules[index];

                if (rule == null)
                {
                    throw new InvalidOperationException(
                        "TimelineContent contains a null " +
                        "FixedTimeRule.");
                }

                TimelineEventDefinition definition =
                    ResolveEventDefinition(
                        rule.EventId,
                        eventDefinitions);

                RegisterScheduledEvent(
                    definition.Id,
                    scheduledEventIds,
                    $"FixedTimeRule at index {index}");

                ValidateFixedTime(
                    rule.TimeSeconds,
                    completionTargetTime,
                    $"FixedTimeRule at index {index}");

                entries.Add(
                    CreateEntry(
                        $"fixed:{index}",
                        definition,
                        rule.TimeSeconds));
            }
        }

        private static void BuildDistributedEntries(
            IReadOnlyList<DistributedRule> rules,
            IReadOnlyDictionary<string, TimelineEventDefinition>
                eventDefinitions,
            float completionTargetTime,
            List<TimelineEntry> entries,
            HashSet<string> scheduledEventIds)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));

            for (int index = 0; index < rules.Count; index++)
            {
                DistributedRule rule =
                    rules[index];

                if (rule == null)
                {
                    throw new InvalidOperationException(
                        "TimelineContent contains a null " +
                        "DistributedRule.");
                }

                float endTime =
                    ResolveDistributedEndTime(
                        rule,
                        completionTargetTime);

                ValidateDistributedRange(
                    rule,
                    endTime,
                    completionTargetTime,
                    index);

                int eventCount =
                    rule.EventIds.Count;

                float step =
                    eventCount > 1
                        ? (endTime - rule.StartTimeSeconds) /
                          eventCount
                        : 0f;

                for (int eventIndex = 0;
                     eventIndex < eventCount;
                     eventIndex++)
                {
                    string eventId =
                        rule.EventIds[eventIndex];

                    TimelineEventDefinition definition =
                        ResolveEventDefinition(
                            eventId,
                            eventDefinitions);

                    RegisterScheduledEvent(
                        definition.Id,
                        scheduledEventIds,
                        $"DistributedRule at index {index}");

                    float scheduledTime =
                        rule.StartTimeSeconds +
                        step * eventIndex;

                    ValidateScheduledTime(
                        scheduledTime,
                        completionTargetTime,
                        $"DistributedRule at index {index}");

                    entries.Add(
                        CreateEntry(
                            $"distributed:{index}:{eventIndex}",
                            definition,
                            scheduledTime));
                }
            }
        }

        private static float ResolveDistributedEndTime(
            DistributedRule rule,
            float completionTargetTime)
        {
            switch (rule.EndTime.Type)
            {
                case TimelineTimeReferenceType.Fixed:
                    return rule.EndTime.TimeSeconds;

                case TimelineTimeReferenceType.CompletionTarget:
                    return completionTargetTime;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported timeline time reference type: " +
                        $"{rule.EndTime.Type}.");
            }
        }

        private static void ValidateDistributedRange(
            DistributedRule rule,
            float endTime,
            float completionTargetTime,
            int ruleIndex)
        {
            if (rule.StartTimeSeconds >= completionTargetTime)
            {
                throw new InvalidOperationException(
                    $"DistributedRule at index {ruleIndex} " +
                    $"must start before the expedition completion target.");
            }

            if (endTime > completionTargetTime)
            {
                throw new InvalidOperationException(
                    $"DistributedRule at index {ruleIndex} " +
                    $"ends after the expedition completion target.");
            }

            if (endTime <= rule.StartTimeSeconds)
            {
                throw new InvalidOperationException(
                    $"DistributedRule at index {ruleIndex} " +
                    $"must have an end time greater than its start time.");
            }
        }

        private static void ValidateScheduledTime(
            float scheduledTime,
            float completionTargetTime,
            string source)
        {
            if (scheduledTime < 0f)
            {
                throw new InvalidOperationException(
                    $"{source} produced a negative scheduled time.");
            }

            if (scheduledTime >= completionTargetTime)
            {
                throw new InvalidOperationException(
                    $"{source} schedules an event at or after the " +
                    $"expedition completion target.");
            }
        }

        private static void ValidateFixedTime(
            float scheduledTime,
            float completionTargetTime,
            string source)
        {
            if (scheduledTime < 0f)
            {
                throw new InvalidOperationException(
                    $"{source} produced a negative scheduled time.");
            }

            if (scheduledTime > completionTargetTime)
            {
                throw new InvalidOperationException(
                    $"{source} schedules an event after the " +
                    $"expedition completion target.");
            }
        }

        private static void RegisterScheduledEvent(
            string eventId,
            HashSet<string> scheduledEventIds,
            string source)
        {
            if (!scheduledEventIds.Add(eventId))
            {
                throw new InvalidOperationException(
                    $"{source} attempts to schedule event id " +
                    $"'{eventId}', but that event is already scheduled.");
            }
        }

        private static TimelineEventDefinition
            ResolveEventDefinition(
                string eventId,
                IReadOnlyDictionary<string, TimelineEventDefinition>
                    eventDefinitions)
        {
            if (!eventDefinitions.TryGetValue(
                    eventId,
                    out TimelineEventDefinition definition))
            {
                throw new InvalidOperationException(
                    $"Timeline references an undefined event id " +
                    $"'{eventId}'.");
            }

            return definition;
        }

        private static TimelineEntry CreateEntry(
            string entryId,
            TimelineEventDefinition definition,
            float scheduledTime)
        {
            return new TimelineEntry(
                entryId,
                definition.Id,
                scheduledTime,
                definition.IconId,
                definition.TriggerReference);
        }

        private static void SortEntries(
            List<TimelineEntry> entries)
        {
            entries.Sort(
                CompareEntries);
        }

        private static int CompareEntries(
            TimelineEntry left,
            TimelineEntry right)
        {
            int timeComparison =
                left.ScheduledTime.CompareTo(
                    right.ScheduledTime);

            if (timeComparison != 0)
                return timeComparison;

            return string.CompareOrdinal(
                left.EntryId,
                right.EntryId);
        }
    }
}