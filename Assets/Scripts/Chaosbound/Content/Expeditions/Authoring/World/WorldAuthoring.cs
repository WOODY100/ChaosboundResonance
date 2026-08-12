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
        [Min(3)]
        private int m_Width = 11;

        [SerializeField]
        [Min(3)]
        private int m_Height = 11;

        [Header("Theme")]
        [SerializeField]
        private WorldThemeAsset m_Theme;

        public int Width => m_Width;

        public int Height => m_Height;

        public WorldThemeAsset Theme => m_Theme;

#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateDimension(m_Width, nameof(m_Width));
            ValidateDimension(m_Height, nameof(m_Height));
        }

        private void ValidateDimension(
            int value,
            string fieldName)
        {
            if (value < 3)
            {
                Debug.LogError(
                    $"WorldAuthoring '{fieldName}' must be at least 3.",
                    null);

                return;
            }

            if (value % 2 == 0)
            {
                Debug.LogError(
                    $"WorldAuthoring '{fieldName}' must be an odd number. " +
                    $"Current value: {value}.",
                    null);
            }
        }
#endif
    }
}