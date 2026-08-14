using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Timeline
{
    [Serializable]
    public sealed class TimelineAuthoring
    {
        [Header("Events")]

        [SerializeField]
        private List<TimelineEventDefinitionAuthoring>
            m_EventDefinitions = new();

        public IReadOnlyList<TimelineEventDefinitionAuthoring> EventDefinitions =>
            m_EventDefinitions;

        [Header("Scheduling Rules")]

        [SerializeField]
        private List<FixedTimeRuleAuthoring>
            m_FixedTimeRules = new();

        public IReadOnlyList<FixedTimeRuleAuthoring> FixedTimeRules =>
            m_FixedTimeRules;

        [SerializeField]
        private List<DistributedRuleAuthoring>
            m_DistributedRules = new();

        public IReadOnlyList<DistributedRuleAuthoring> DistributedRules =>
            m_DistributedRules;

        [Header("Explicit Events")]

        [SerializeField]
        private List<ExplicitTimelineEventAuthoring>
            m_ExplicitEvents = new();

        public IReadOnlyList<ExplicitTimelineEventAuthoring> ExplicitEvents =>
            m_ExplicitEvents;

        [Header("Completion Target")]

        [SerializeField]
        private ExpeditionCompletionTargetAuthoring
            m_CompletionTarget = new();

        public ExpeditionCompletionTargetAuthoring CompletionTarget =>
            m_CompletionTarget;
    }
}