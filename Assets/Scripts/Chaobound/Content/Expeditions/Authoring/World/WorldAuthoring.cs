using System;
using UnityEngine;
using Chaosbound.Content.World.Themes;

namespace Chaosbound.Content.Expeditions.Authoring.World
{
    [Serializable]
    public sealed class WorldAuthoring
    {
        [Header("Geometry")]
        [SerializeField]
        private int m_Width;

        [SerializeField]
        private int m_Height;

        [Header("Theme")]
        [SerializeField]
        private WorldThemeAsset m_Theme;

        public int Width => m_Width;

        public int Height => m_Height;

        public WorldThemeAsset Theme => m_Theme;
    }
}