using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates
{
    /// <summary>
    /// Converts positions from world space into normalized
    /// minimap coordinates.
    ///
    /// X represents the horizontal axis.
    /// Y represents the vertical minimap axis and is derived
    /// from world Z.
    ///
    /// The mapper does not know about UI objects, textures
    /// or markers.
    /// </summary>
    public sealed class MinimapCoordinateMapper
    {
        private readonly Bounds worldBounds;

        public MinimapCoordinateMapper(
            Bounds worldBounds)
        {
            if (worldBounds.size.x <= 0f)
            {
                throw new ArgumentException(
                    "World bounds width must be greater than zero.",
                    nameof(worldBounds));
            }

            if (worldBounds.size.z <= 0f)
            {
                throw new ArgumentException(
                    "World bounds depth must be greater than zero.",
                    nameof(worldBounds));
            }

            this.worldBounds =
                worldBounds;
        }

        /// <summary>
        /// Converts a world position into normalized minimap
        /// coordinates.
        ///
        /// X = 0 represents the west/left edge.
        /// X = 1 represents the east/right edge.
        ///
        /// Y = 0 represents the south/bottom edge.
        /// Y = 1 represents the north/top edge.
        /// </summary>
        public Vector2 WorldToNormalized(
            Vector3 worldPosition)
        {
            float normalizedX =
                Mathf.InverseLerp(
                    worldBounds.min.x,
                    worldBounds.max.x,
                    worldPosition.x);

            float normalizedY =
                Mathf.InverseLerp(
                    worldBounds.min.z,
                    worldBounds.max.z,
                    worldPosition.z);

            return new Vector2(
                normalizedX,
                normalizedY);
        }

        /// <summary>
        /// Converts normalized minimap coordinates into
        /// a world position on the generated world plane.
        /// </summary>
        public Vector3 NormalizedToWorld(
            Vector2 normalizedPosition,
            float y)
        {
            float worldX =
                Mathf.Lerp(
                    worldBounds.min.x,
                    worldBounds.max.x,
                    normalizedPosition.x);

            float worldZ =
                Mathf.Lerp(
                    worldBounds.min.z,
                    worldBounds.max.z,
                    normalizedPosition.y);

            return new Vector3(
                worldX,
                y,
                worldZ);
        }

        /// <summary>
        /// Converts a world position directly into texture
        /// pixel coordinates.
        /// </summary>
        public Vector2Int WorldToPixel(
            Vector3 worldPosition,
            int textureWidth,
            int textureHeight)
        {
            if (textureWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(textureWidth));
            }

            if (textureHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(textureHeight));
            }

            Vector2 normalized =
                WorldToNormalized(
                    worldPosition);

            int pixelX =
                Mathf.Clamp(
                    Mathf.FloorToInt(
                        normalized.x * textureWidth),
                    0,
                    textureWidth - 1);

            int pixelY =
                Mathf.Clamp(
                    Mathf.FloorToInt(
                        normalized.y * textureHeight),
                    0,
                    textureHeight - 1);

            return new Vector2Int(
                pixelX,
                pixelY);
        }
    }
}