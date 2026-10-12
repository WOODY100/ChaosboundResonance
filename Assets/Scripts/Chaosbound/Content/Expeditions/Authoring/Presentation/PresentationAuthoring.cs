using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Presentation
{
    [Serializable]
    public sealed class PresentationAuthoring
    {
        [SerializeField]
        private string m_displayName = "New Expedition";

        [SerializeField]
        [TextArea]
        private string m_description;

        [SerializeField]
        private Sprite m_image;

        public string DisplayName => m_displayName;

        public string Description => m_description;

        public Sprite Image => m_image;
    }
}