using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Timeline
{
    [Serializable]
    public sealed class DistributedRuleAuthoring
    {
        [SerializeField]
        private string m_EventId;

        public string EventId => m_EventId;

        [SerializeField]
        private int m_Count = 1;

        public int Count => m_Count;

        [SerializeField]
        private float m_StartTimeSeconds;

        public float StartTimeSeconds => m_StartTimeSeconds;

        [SerializeField]
        private TimelineEndTimeType m_EndTimeType =
            TimelineEndTimeType.Fixed;

        public TimelineEndTimeType EndTimeType =>
            m_EndTimeType;

        [SerializeField]
        private float m_EndTimeSeconds;

        public float EndTimeSeconds => m_EndTimeSeconds;
    }
}