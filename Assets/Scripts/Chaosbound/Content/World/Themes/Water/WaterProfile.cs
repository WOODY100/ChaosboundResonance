using UnityEngine;

namespace Chaosbound.Content.World.Themes.Water
{
    [CreateAssetMenu(
        fileName = "WaterProfile",
        menuName = "Chaosbound/World/Water Profile")]
    public sealed class WaterProfile : ScriptableObject
    {
        [Header("Material")]
        [SerializeField]
        private Material material;

        [Header("Identity")]
        [SerializeField]
        private Color waterDarkColor = Color.black;

        [SerializeField]
        private Color waterBrightColor = Color.white;

        [SerializeField]
        private Color shoreFoamColor = Color.white;

        [SerializeField]
        private Color waterFoamColor = Color.white;

        [Min(0f)]
        [SerializeField]
        private float waveIntensity = 0.2f;

        [Min(0f)]
        [SerializeField]
        private float emission = 0.3f;

        [Header("World Surface")]
        [SerializeField]
        private float surfaceHeight = 0f;

        [Min(0f)]
        [SerializeField]
        private float boundsPadding = 12f;

        public Material Material => material;

        public Color WaterDarkColor => waterDarkColor;
        public Color WaterBrightColor => waterBrightColor;
        public Color ShoreFoamColor => shoreFoamColor;
        public Color WaterFoamColor => waterFoamColor;

        public float WaveIntensity => waveIntensity;
        public float Emission => emission;

        public float SurfaceHeight => surfaceHeight;
        public float BoundsPadding => boundsPadding;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (waveIntensity < 0f)
                waveIntensity = 0f;

            if (emission < 0f)
                emission = 0f;

            if (boundsPadding < 0f)
                boundsPadding = 0f;
        }
#endif
    }
}