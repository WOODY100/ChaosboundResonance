using UnityEngine;

namespace Chaosbound.Content.Expeditions.Runtime.Minimap
{
    /// <summary>
    /// Immutable runtime configuration for the expedition minimap.
    /// </summary>
    public sealed class RuntimeMinimapConfig
    {
        /// <summary>
        /// Texture used to represent walkable areas
        /// on the static minimap.
        /// </summary>
        public Texture2D WalkableTexture { get; }

        public RuntimeMinimapConfig(
            Texture2D walkableTexture)
        {
            WalkableTexture =
                walkableTexture;
        }
    }
}