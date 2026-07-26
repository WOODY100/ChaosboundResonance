using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Identity
{
    [Serializable]
    public sealed class IdentityAuthoring
    {
        [SerializeField]
        private string m_id = "expedition.default";

        public string Id => m_id;
    }
}