using Chaosbound.Content.World.Themes.TileSets;
using Chaosbound.Gameplay.ExpeditionRuntime.World;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Data;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Rendering
{
    /// <summary>
    /// Generates the static cartographic representation
    /// of the expedition world.
    ///
    /// This renderer does not render the physical world.
    /// It converts MinimapMapData into a procedural
    /// cartographic texture.
    /// </summary>
    public sealed class MinimapStaticMapRenderer
    {
        private const int CellResolution =
            MinimapTileMask.Resolution;

        private readonly float tileSize;

        private readonly int pixelsPerCell;

        private readonly Color walkableColor;
        private readonly Color blockedColor;

        public MinimapStaticMapRenderer(
            float tileSize,
            int pixelsPerCell,
            Color walkableColor,
            Color blockedColor)
        {
            if (tileSize <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tileSize));
            }

            if (pixelsPerCell <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pixelsPerCell));
            }

            this.tileSize =
                tileSize;

            this.pixelsPerCell =
                pixelsPerCell;

            this.walkableColor =
                walkableColor;

            this.blockedColor =
                blockedColor;
        }

        /// <summary>
        /// Generates a cartographic texture from
        /// the provided minimap map data.
        /// </summary>
        public Texture2D Render(
            MinimapMapData mapData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException(
                    nameof(mapData));
            }

            int textureWidth =
                CalculateTextureDimension(
                    mapData.WorldBounds.size.x);

            int textureHeight =
                CalculateTextureDimension(
                    mapData.WorldBounds.size.z);

            Texture2D texture =
                new Texture2D(
                    textureWidth,
                    textureHeight,
                    TextureFormat.RGBA32,
                    false);

            texture.name =
                "Minimap_StaticMap";

            texture.filterMode =
                FilterMode.Point;

            texture.wrapMode =
                TextureWrapMode.Clamp;

            ClearTexture(
                texture);

            foreach (
                MinimapTileData tile
                in mapData.Tiles)
            {
                DrawTile(
                    texture,
                    mapData,
                    tile);
            }

            texture.Apply();

            return texture;
        }

        private int CalculateTextureDimension(
            float worldSize)
        {
            int logicalCells =
                Mathf.RoundToInt(
                    worldSize /
                    tileSize *
                    CellResolution);

            return Mathf.Max(
                1,
                logicalCells *
                pixelsPerCell);
        }

        private void ClearTexture(
            Texture2D texture)
        {
            Color[] pixels =
                new Color[
                    texture.width *
                    texture.height];

            for (int i = 0;
                 i < pixels.Length;
                 i++)
            {
                pixels[i] =
                    Color.clear;
            }

            texture.SetPixels(
                pixels);
        }

        private void DrawTile(
            Texture2D texture,
            MinimapMapData mapData,
            MinimapTileData tile)
        {
            Vector3 tileCenter =
                GetTileCenter(
                    mapData,
                    tile);

            float totalWidth =
                tile.Footprint.x *
                tileSize;

            float totalDepth =
                tile.Footprint.y *
                tileSize;

            float cellWidth =
                totalWidth /
                CellResolution;

            float cellDepth =
                totalDepth /
                CellResolution;

            for (int z = 0;
                 z < CellResolution;
                 z++)
            {
                for (int x = 0;
                     x < CellResolution;
                     x++)
                {
                    bool blocked =
                        tile.IsBlocked(
                            x,
                            z);

                    Vector3 localPosition =
                        GetCellLocalPosition(
                            tile,
                            x,
                            z,
                            cellWidth,
                            cellDepth);

                    Vector3 worldPosition =
                        tileCenter +
                        localPosition;

                    Color color =
                        blocked
                        ? blockedColor
                        : walkableColor;

                    DrawWorldCell(
                        texture,
                        mapData.WorldBounds,
                        worldPosition,
                        cellWidth,
                        cellDepth,
                        color);
                }
            }
        }

        private Vector3 GetTileCenter(
            MinimapMapData mapData,
            MinimapTileData tile)
        {
            Bounds bounds =
                mapData.WorldBounds;

            float firstTileCenterX =
                bounds.min.x +
                (tileSize * 0.5f);

            float firstTileCenterZ =
                bounds.min.z +
                (tileSize * 0.5f);

            float centerX =
                firstTileCenterX +
                (tile.GridPosition.x *
                 tileSize);

            float centerZ =
                firstTileCenterZ +
                (tile.GridPosition.y *
                 tileSize);

            float footprintOffsetX =
                (tile.Footprint.x - 1) *
                tileSize *
                0.5f;

            float footprintOffsetZ =
                (tile.Footprint.y - 1) *
                tileSize *
                0.5f;

            return new Vector3(
                centerX + footprintOffsetX,
                bounds.min.y,
                centerZ + footprintOffsetZ);
        }

        private Vector3 GetCellLocalPosition(
            MinimapTileData tile,
            int x,
            int z,
            float cellWidth,
            float cellDepth)
        {
            float localX =
                (-tile.Footprint.x *
                 tileSize *
                 0.5f) +
                (cellWidth * 0.5f) +
                (x * cellWidth);

            float localZ =
                (-tile.Footprint.y *
                 tileSize *
                 0.5f) +
                (cellDepth * 0.5f) +
                (z * cellDepth);

            Vector3 local =
                new Vector3(
                    localX,
                    0f,
                    localZ);

            return RotateLocalPosition(
                local,
                tile.Rotation);
        }

        private Vector3 RotateLocalPosition(
            Vector3 position,
            TileRotation rotation)
        {
            switch (rotation)
            {
                case TileRotation.Rotation0:
                    return position;

                case TileRotation.Rotation90:
                    return new Vector3(
                        position.z,
                        0f,
                        -position.x);

                case TileRotation.Rotation180:
                    return new Vector3(
                        -position.x,
                        0f,
                        -position.z);

                case TileRotation.Rotation270:
                    return new Vector3(
                        -position.z,
                        0f,
                        position.x);

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(rotation),
                        rotation,
                        "Unsupported tile rotation.");
            }
        }

        private void DrawWorldCell(
            Texture2D texture,
            Bounds worldBounds,
            Vector3 worldPosition,
            float worldCellWidth,
            float worldCellDepth,
            Color color)
        {
            float normalizedX =
                (worldPosition.x -
                 worldBounds.min.x) /
                worldBounds.size.x;

            float normalizedZ =
                (worldPosition.z -
                 worldBounds.min.z) /
                worldBounds.size.z;

            float normalizedWidth =
                worldCellWidth /
                worldBounds.size.x;

            float normalizedDepth =
                worldCellDepth /
                worldBounds.size.z;

            int centerX =
                Mathf.RoundToInt(
                    normalizedX *
                    texture.width);

            int centerZ =
                Mathf.RoundToInt(
                    normalizedZ *
                    texture.height);

            int pixelWidth =
                Mathf.Max(
                    1,
                    Mathf.RoundToInt(
                        normalizedWidth *
                        texture.width));

            int pixelHeight =
                Mathf.Max(
                    1,
                    Mathf.RoundToInt(
                        normalizedDepth *
                        texture.height));

            int startX =
                centerX -
                (pixelWidth / 2);

            int startZ =
                centerZ -
                (pixelHeight / 2);

            for (int z = 0;
                 z < pixelHeight;
                 z++)
            {
                int textureZ =
                    startZ + z;

                if (textureZ < 0 ||
                    textureZ >= texture.height)
                {
                    continue;
                }

                for (int x = 0;
                     x < pixelWidth;
                     x++)
                {
                    int textureX =
                        startX + x;

                    if (textureX < 0 ||
                        textureX >= texture.width)
                    {
                        continue;
                    }

                    texture.SetPixel(
                        textureX,
                        textureZ,
                        color);
                }
            }
        }
    }
}