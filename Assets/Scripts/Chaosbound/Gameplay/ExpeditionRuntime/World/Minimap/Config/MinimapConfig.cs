using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Config
{
    /// <summary>
    /// Configuration for the runtime minimap presentation.
    ///
    /// This configuration does not contain expedition state
    /// or world data. It only defines how the minimap is rendered.
    /// </summary>
    [Serializable]
    public sealed class MinimapConfig
    {
        //==========================================================
        // World Mapping
        //==========================================================

        [Header("World Mapping")]

        [Min(0.01f)]
        [SerializeField]
        private float tileSize = 12f;

        //==========================================================
        // Orientation
        //==========================================================

        [Header("Orientation")]

        [SerializeField]
        private float orientationDegrees = 45f;

        public float OrientationDegrees =>
            orientationDegrees;

        //==========================================================
        // Texture
        //==========================================================

        [Header("Texture")]

        [Min(1)]
        [SerializeField]
        private int pixelsPerCell = 4;

        //==========================================================
        // Zoom
        //==========================================================

        [Header("Zoom")]

        [Min(0.01f)]
        [SerializeField]
        private float zoom = 2.5f;

        //==========================================================
        // Colors
        //==========================================================

        [Header("Colors")]

        [SerializeField]
        private Color walkableColor = Color.white;

        [SerializeField]
        private Color blockedColor = Color.black;

        //==========================================================
        // Public Properties
        //==========================================================

        public float TileSize =>
            tileSize;

        public int PixelsPerCell =>
            pixelsPerCell;

        public float Zoom =>
            zoom;

        public Color WalkableColor =>
            walkableColor;

        public Color BlockedColor =>
            blockedColor;

#if UNITY_EDITOR

        //==========================================================
        // Validation
        //==========================================================

        private void OnValidate()
        {
            if (tileSize <= 0f)
                tileSize = 0.01f;

            if (pixelsPerCell <= 0)
                pixelsPerCell = 1;

            if (zoom <= 0f)
                zoom = 0.01f;
        }

#endif
    }
}