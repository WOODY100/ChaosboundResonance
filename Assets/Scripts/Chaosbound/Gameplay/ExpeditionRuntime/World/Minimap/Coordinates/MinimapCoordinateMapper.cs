using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates
{
    /// <summary>
    /// Converts positions from world space into normalized
    /// minimap coordinates.
    ///
    /// The minimap coordinate system is defined by a
    /// MinimapOrientationBasis.
    ///
    /// The mapper does not know about cameras, UI objects,
    /// textures or markers.
    /// </summary>
    public sealed class MinimapCoordinateMapper
    {
        private readonly Bounds worldBounds;

        private readonly MinimapOrientationBasis
            orientationBasis;

        public MinimapCoordinateMapper(
            Bounds worldBounds,
            MinimapOrientationBasis orientationBasis)
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

            this.orientationBasis =
                orientationBasis;
        }

        //==========================================================
        // World → Minimap
        //==========================================================

        /// <summary>
        /// Converts a world position into normalized minimap
        /// coordinates using the configured minimap orientation.
        ///
        /// X = 0 represents the left edge.
        /// X = 1 represents the right edge.
        ///
        /// Y = 0 represents the bottom edge.
        /// Y = 1 represents the top edge.
        /// </summary>
        public Vector2 WorldToNormalized(
            Vector3 worldPosition)
        {
            Vector2 relativePosition =
                new Vector2(
                    worldPosition.x -
                    worldBounds.center.x,

                    worldPosition.z -
                    worldBounds.center.z);

            float minimapX =
                Vector2.Dot(
                    relativePosition,
                    orientationBasis.Right);

            float minimapY =
                Vector2.Dot(
                    relativePosition,
                    orientationBasis.Up);

            float normalizedX =
                (minimapX /
                 worldBounds.size.x) +
                0.5f;

            float normalizedY =
                (minimapY /
                 worldBounds.size.z) +
                0.5f;

            return new Vector2(
                normalizedX,
                normalizedY);
        }

        //==========================================================
        // Minimap → World
        //==========================================================

        /// <summary>
        /// Converts normalized minimap coordinates into
        /// a world position on the generated world plane.
        /// </summary>
        public Vector3 NormalizedToWorld(
            Vector2 normalizedPosition,
            float y)
        {
            float minimapX =
                (normalizedPosition.x - 0.5f) *
                worldBounds.size.x;

            float minimapY =
                (normalizedPosition.y - 0.5f) *
                worldBounds.size.z;

            Vector2 worldRelativePosition =
                (orientationBasis.Right * minimapX) +
                (orientationBasis.Up * minimapY);

            return new Vector3(
                worldRelativePosition.x +
                    worldBounds.center.x,

                y,

                worldRelativePosition.y +
                    worldBounds.center.z);
        }

        //==========================================================
        // World → Pixel
        //==========================================================

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
                        normalized.x *
                        textureWidth),
                    0,
                    textureWidth - 1);

            int pixelY =
                Mathf.Clamp(
                    Mathf.FloorToInt(
                        normalized.y *
                        textureHeight),
                    0,
                    textureHeight - 1);

            return new Vector2Int(
                pixelX,
                pixelY);
        }
    }
}