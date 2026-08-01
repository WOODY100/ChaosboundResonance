using Chaosbound.Shared.Enums;
using System;
using UnityEngine;

namespace Chaosbound.Shared.Authoring
{
    [Serializable]
    public sealed class ContentReferenceAuthoring
    {
        [SerializeField]
        private string m_Id;

        [SerializeField]
        private ContentCategory m_Category;

        public string Id => m_Id;

        public ContentCategory Category => m_Category;
    }
}