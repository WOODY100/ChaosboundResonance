using UnityEngine;

namespace Chaosbound.Content.Expeditions.Definitions.Minimap
{
    /// <summary>
    /// Immutable definition of the minimap content
    /// associated with an expedition.
    /// </summary>
    public sealed class MinimapDefinition
    {
        public Texture2D WalkableTexture { get; }

        public MinimapDefinition(
            Texture2D walkableTexture)
        {
            WalkableTexture =
                walkableTexture;
        }
    }
}