using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Timeline
{
    [Serializable]
    public sealed class ExplicitTimelineEventAuthoring
    {
        [SerializeField]
        private string m_EventId;

        public string EventId => m_EventId;

        [SerializeField]
        private float m_TimeSeconds;

        public float TimeSeconds => m_TimeSeconds;
    }
}