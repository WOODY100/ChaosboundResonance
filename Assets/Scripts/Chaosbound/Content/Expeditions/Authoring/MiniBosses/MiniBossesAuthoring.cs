using Chaosbound.Shared.Authoring;
using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.MiniBosses
{
    [Serializable]
    public sealed class MiniBossesAuthoring
    {
        [SerializeField]
        private ContentReferenceAuthoring[] m_Content;

        public ContentReferenceAuthoring[] Content => m_Content;
    }
}