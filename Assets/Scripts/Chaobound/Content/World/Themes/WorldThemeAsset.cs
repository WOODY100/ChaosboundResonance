using UnityEngine;
using Chaosbound.Content.World.Themes.TileSets;
using Chaosbound.Content.World.Themes.Decorations;
using Chaosbound.Content.World.Themes.Water;

namespace Chaosbound.Content.World.Themes
{
    [CreateAssetMenu(
        fileName = "WorldTheme",
        menuName = "Chaosbound/World/World Theme")]
    public sealed class WorldThemeAsset : ScriptableObject
    {
        [Header("Theme Composition")]
        [SerializeField]
        private TileSetProfile tileSet;

        [SerializeField]
        private DecorationProfile decoration;

        [SerializeField]
        private WaterProfile water;

        public TileSetProfile TileSet => tileSet;

        public DecorationProfile Decoration => decoration;

        public WaterProfile Water => water;

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Intencionalmente vacío.
            // WorldThemeAsset únicamente compone recursos.
        }
#endif
    }
}