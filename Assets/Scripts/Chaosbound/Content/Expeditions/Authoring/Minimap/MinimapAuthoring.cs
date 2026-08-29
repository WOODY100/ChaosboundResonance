using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Minimap
{
    /// <summary>
    /// Authoring configuration for the expedition minimap.
    ///
    /// Defines the visual texture used to represent
    /// walkable world areas on the static minimap.
    /// </summary>
    [Serializable]
    public sealed class MinimapAuthoring
    {
        [Header("Walkable")]

        [SerializeField]
        private Texture2D m_walkableTexture;

        public Texture2D WalkableTexture =>
            m_walkableTexture;
    }
}