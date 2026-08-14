using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Timeline
{
    [Serializable]
    public sealed class TimelineTriggerReferenceAuthoring
    {
        [SerializeField]
        private string m_DomainId;

        public string DomainId => m_DomainId;

        [SerializeField]
        private string m_ContentId;

        public string ContentId => m_ContentId;
    }
}