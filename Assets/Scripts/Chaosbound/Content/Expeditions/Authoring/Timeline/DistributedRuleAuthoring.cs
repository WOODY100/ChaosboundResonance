using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Timeline
{
    [Serializable]
    public sealed class DistributedRuleAuthoring
    {
        [SerializeField]
        private List<string> m_EventIds = new();

        public IReadOnlyList<string> EventIds =>
            m_EventIds;

        [SerializeField]
        private float m_StartTimeSeconds;

        public float StartTimeSeconds =>
            m_StartTimeSeconds;

        [SerializeField]
        private TimelineEndTimeType m_EndTimeType =
            TimelineEndTimeType.Fixed;

        public TimelineEndTimeType EndTimeType =>
            m_EndTimeType;

        [SerializeField]
        private float m_EndTimeSeconds;

        public float EndTimeSeconds =>
            m_EndTimeSeconds;
    }
}