using System;
using UnityEngine;

namespace Chaosbound.Shared.Authoring
{
    [Serializable]
    public sealed class ContentReferenceAuthoring
    {
        [SerializeField]
        private string m_Id;

        public string Id => m_Id;
    }
}