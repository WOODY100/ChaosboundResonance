using Chaosbound.Shared.Authoring;
using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Rewards
{
    [Serializable]
    public sealed class RewardsAuthoring
    {
        [SerializeField]
        private ContentReferenceAuthoring[] m_Content;

        public ContentReferenceAuthoring[] Content => m_Content;
    }
}