using Chaosbound.Shared.Authoring;
using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Bosses
{
    [Serializable]
    public sealed class BossesAuthoring
    {
        [SerializeField]
        private ContentReferenceAuthoring[] m_Content;

        public ContentReferenceAuthoring[] Content => m_Content;
    }
}