using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Timeline
{
    [Serializable]
    public sealed class TimelineEventDefinitionAuthoring
    {
        [SerializeField]
        private string m_Id;

        public string Id => m_Id;

        [SerializeField]
        private string m_IconId;

        public string IconId => m_IconId;

        [SerializeField]
        private TimelineTriggerReferenceAuthoring m_TriggerReference;

        public TimelineTriggerReferenceAuthoring TriggerReference =>
            m_TriggerReference;
    }
}