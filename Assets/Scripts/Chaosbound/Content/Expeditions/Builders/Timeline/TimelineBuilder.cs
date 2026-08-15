using Chaosbound.Content.Expeditions.Authoring.Timeline;
using Chaosbound.Content.Expeditions.Definitions.Timeline;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Builders.Timeline
{
    /// <summary>
    /// Converts timeline authoring data into its domain representation.
    /// </summary>
    public static class TimelineBuilder
    {
        public static TimelineContent Build(
            TimelineAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<TimelineEventDefinition> eventDefinitions =
                BuildEventDefinitions(
                    authoring.EventDefinitions);

            List<FixedTimeRule> fixedTimeRules =
                BuildFixedTimeRules(
                    authoring.FixedTimeRules);

            List<DistributedRule> distributedRules =
                BuildDistributedRules(
                    authoring.DistributedRules);

            ExpeditionCompletionTarget completionTarget =
                BuildCompletionTarget(
                    authoring.CompletionTarget);

            return new TimelineContent(
                eventDefinitions,
                fixedTimeRules,
                distributedRules,
                completionTarget);
        }

        private static List<TimelineEventDefinition>
            BuildEventDefinitions(
                IReadOnlyList<TimelineEventDefinitionAuthoring> authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<TimelineEventDefinition> result =
                new(authoring.Count);

            foreach (
                TimelineEventDefinitionAuthoring eventAuthoring
                in authoring)
            {
                if (eventAuthoring == null)
                {
                    throw new InvalidOperationException(
                        "TimelineAuthoring contains a null " +
                        "TimelineEventDefinitionAuthoring.");
                }

                TimelineTriggerReference triggerReference = null;

                if (eventAuthoring.TriggerReference != null)
                {
                    triggerReference =
                        BuildTriggerReference(
                            eventAuthoring.TriggerReference);
                }

                result.Add(
                    new TimelineEventDefinition(
                        eventAuthoring.Id,
                        eventAuthoring.IconId,
                        triggerReference));
            }

            return result;
        }

        private static TimelineTriggerReference
            BuildTriggerReference(
        TimelineTriggerReferenceAuthoring authoring)
        {
            if (authoring == null)
                return null;

            bool domainEmpty =
                string.IsNullOrWhiteSpace(
                    authoring.DomainId);

            bool contentEmpty =
                string.IsNullOrWhiteSpace(
                    authoring.ContentId);

            // An empty reference means that this
            // Timeline event does not trigger another domain.
            if (domainEmpty && contentEmpty)
                return null;

            // A partially configured reference is invalid.
            if (domainEmpty)
            {
                throw new InvalidOperationException(
                    "TimelineTriggerReference requires a DomainId " +
                    "when a ContentId is provided.");
            }

            if (contentEmpty)
            {
                throw new InvalidOperationException(
                    "TimelineTriggerReference requires a ContentId " +
                    "when a DomainId is provided.");
            }

            return new TimelineTriggerReference(
                authoring.DomainId,
                authoring.ContentId);
        }

        private static List<FixedTimeRule>
            BuildFixedTimeRules(
                IReadOnlyList<FixedTimeRuleAuthoring> authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<FixedTimeRule> result =
                new(authoring.Count);

            foreach (
                FixedTimeRuleAuthoring ruleAuthoring
                in authoring)
            {
                if (ruleAuthoring == null)
                {
                    throw new InvalidOperationException(
                        "TimelineAuthoring contains a null " +
                        "FixedTimeRuleAuthoring.");
                }

                result.Add(
                    new FixedTimeRule(
                        ruleAuthoring.EventId,
                        ruleAuthoring.TimeSeconds));
            }

            return result;
        }

        private static List<DistributedRule>
            BuildDistributedRules(
                IReadOnlyList<DistributedRuleAuthoring> authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<DistributedRule> result =
                new(authoring.Count);

            foreach (
                DistributedRuleAuthoring ruleAuthoring
                in authoring)
            {
                if (ruleAuthoring == null)
                {
                    throw new InvalidOperationException(
                        "TimelineAuthoring contains a null " +
                        "DistributedRuleAuthoring.");
                }

                TimelineTimeReference endTime =
                    BuildEndTimeReference(
                        ruleAuthoring);

                result.Add(
                    new DistributedRule(
                        ruleAuthoring.EventIds,
                        ruleAuthoring.StartTimeSeconds,
                        endTime));
            }

            return result;
        }

        private static TimelineTimeReference
            BuildEndTimeReference(
                DistributedRuleAuthoring authoring)
        {
            switch (authoring.EndTimeType)
            {
                case TimelineEndTimeType.Fixed:
                    return TimelineTimeReference.Fixed(
                        authoring.EndTimeSeconds);

                case TimelineEndTimeType.CompletionTarget:
                    return TimelineTimeReference.CompletionTarget();

                default:
                    throw new InvalidOperationException(
                        $"Unsupported timeline end time type: " +
                        $"{authoring.EndTimeType}.");
            }
        }

        private static ExpeditionCompletionTarget
            BuildCompletionTarget(
                ExpeditionCompletionTargetAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            return new ExpeditionCompletionTarget(
                authoring.TimeSeconds);
        }
    }
}