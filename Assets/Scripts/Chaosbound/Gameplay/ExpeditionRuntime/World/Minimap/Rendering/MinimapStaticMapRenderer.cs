using Chaosbound.Content.World.Themes.TileSets;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Data;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Rendering
{
    /// <summary>
    /// Generates the static cartographic representation
    /// of the expedition world.
    ///
    /// Walkable cells are rendered using the configured
    /// seamless walkable texture.
    /// Blocked cells are rendered using the configured
    /// blocked color.
    ///
    /// The walkable texture is mapped in world space and
    /// repeats once per world tile.
    /// </summary>
    public sealed class MinimapStaticMapRenderer
    {
        private const int CellResolution =
            MinimapTileMask.Resolution;

        private const int MaxWalkableTextureResolution =
            256;

        private readonly float tileSize;

        private readonly int pixelsPerCell;

        private readonly Color blockedColor;

        private readonly Texture2D walkableTexture;

        private readonly MinimapOrientationBasis
            orientationBasis;

        private Texture2D readableWalkableTexture;

        public MinimapStaticMapRenderer(
            float tileSize,
            int pixelsPerCell,
            Color blockedColor,
            Texture2D walkableTexture,
            MinimapOrientationBasis orientationBasis)
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

            if (walkableTexture == null)
            {
                throw new ArgumentNullException(
                    nameof(walkableTexture));
            }

            this.tileSize =
                tileSize;

            this.pixelsPerCell =
                pixelsPerCell;

            this.blockedColor =
                blockedColor;

            this.walkableTexture =
                walkableTexture;

            this.orientationBasis =
                orientationBasis;
        }

        //==========================================================
        // Render
        //==========================================================

        public Texture2D Render(
            MinimapMapData mapData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException(
                    nameof(mapData));
            }

            PrepareReadableWalkableTexture();

            try
            {
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
            finally
            {
                ReleaseReadableWalkableTexture();
            }
        }

        //==========================================================
        // Readable Walkable Texture
        //==========================================================

        private void PrepareReadableWalkableTexture()
        {
            if (walkableTexture == null)
            {
                throw new InvalidOperationException(
                    "Walkable texture is not configured.");
            }

            int sourceWidth =
                walkableTexture.width;

            int sourceHeight =
                walkableTexture.height;

            int targetWidth =
                sourceWidth;

            int targetHeight =
                sourceHeight;

            int maxDimension =
                Mathf.Max(
                    sourceWidth,
                    sourceHeight);

            if (maxDimension >
                MaxWalkableTextureResolution)
            {
                float scale =
                    (float)MaxWalkableTextureResolution /
                    maxDimension;

                targetWidth =
                    Mathf.Max(
                        1,
                        Mathf.RoundToInt(
                            sourceWidth * scale));

                targetHeight =
                    Mathf.Max(
                        1,
                        Mathf.RoundToInt(
                            sourceHeight * scale));
            }

            RenderTexture temporaryRenderTexture =
                RenderTexture.GetTemporary(
                    targetWidth,
                    targetHeight,
                    0,
                    RenderTextureFormat.ARGB32);

            RenderTexture previousActive =
                RenderTexture.active;

            try
            {
                Graphics.Blit(
                    walkableTexture,
                    temporaryRenderTexture);

                RenderTexture.active =
                    temporaryRenderTexture;

                readableWalkableTexture =
                    new Texture2D(
                        targetWidth,
                        targetHeight,
                        TextureFormat.RGBA32,
                        false);

                readableWalkableTexture.name =
                    "Minimap_ReadableWalkableTexture";

                readableWalkableTexture.filterMode =
                    FilterMode.Bilinear;

                readableWalkableTexture.wrapMode =
                    TextureWrapMode.Repeat;

                readableWalkableTexture.ReadPixels(
                    new Rect(
                        0,
                        0,
                        targetWidth,
                        targetHeight),
                    0,
                    0);

                readableWalkableTexture.Apply();
            }
            finally
            {
                RenderTexture.active =
                    previousActive;

                RenderTexture.ReleaseTemporary(
                    temporaryRenderTexture);
            }
        }

        private void ReleaseReadableWalkableTexture()
        {
            if (readableWalkableTexture == null)
                return;

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(
                    readableWalkableTexture);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(
                    readableWalkableTexture);
            }

            readableWalkableTexture =
                null;
        }

        //==========================================================
        // Texture Size
        //==========================================================

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

        //==========================================================
        // Clear
        //==========================================================

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

        //==========================================================
        // Tile
        //==========================================================

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

                    if (blocked)
                    {
                        DrawWorldCell(
                            texture,
                            mapData.WorldBounds,
                            worldPosition,
                            cellWidth,
                            cellDepth,
                            blockedColor);
                    }
                    else
                    {
                        DrawWorldTextureCell(
                            texture,
                            mapData.WorldBounds,
                            worldPosition,
                            cellWidth,
                            cellDepth);
                    }
                }
            }
        }

        //==========================================================
        // Tile Position
        //==========================================================

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

        //==========================================================
        // Cell Position
        //==========================================================

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

        //==========================================================
        // Tile Rotation
        //==========================================================

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

        //==========================================================
        // Solid World Cell
        //==========================================================

        private void DrawWorldCell(
            Texture2D texture,
            Bounds worldBounds,
            Vector3 worldPosition,
            float worldCellWidth,
            float worldCellDepth,
            Color color)
        {
            CalculatePixelRect(
                texture,
                worldBounds,
                worldPosition,
                worldCellWidth,
                worldCellDepth,
                out int centerX,
                out int centerZ,
                out int pixelWidth,
                out int pixelHeight);

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

        //==========================================================
        // Textured World Cell
        //==========================================================

        private void DrawWorldTextureCell(
            Texture2D texture,
            Bounds worldBounds,
            Vector3 worldPosition,
            float worldCellWidth,
            float worldCellDepth)
        {
            CalculatePixelRect(
                texture,
                worldBounds,
                worldPosition,
                worldCellWidth,
                worldCellDepth,
                out int centerX,
                out int centerZ,
                out int pixelWidth,
                out int pixelHeight);

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

                float worldZ =
                    GetWorldCoordinate(
                        textureZ,
                        texture.height,
                        worldBounds.min.z,
                        worldBounds.size.z);

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

                    float worldX =
                        GetWorldCoordinate(
                            textureX,
                            texture.width,
                            worldBounds.min.x,
                            worldBounds.size.x);

                    Color color =
                        SampleWalkableTexture(
                            worldX,
                            worldZ);

                    texture.SetPixel(
                        textureX,
                        textureZ,
                        color);
                }
            }
        }

        //==========================================================
        // Pixel Rectangle
        //==========================================================

        private void CalculatePixelRect(
            Texture2D texture,
            Bounds worldBounds,
            Vector3 worldPosition,
            float worldCellWidth,
            float worldCellDepth,
            out int centerX,
            out int centerZ,
            out int pixelWidth,
            out int pixelHeight)
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

            float normalizedWidth =
                worldCellWidth /
                worldBounds.size.x;

            float normalizedDepth =
                worldCellDepth /
                worldBounds.size.z;

            centerX =
                Mathf.RoundToInt(
                    normalizedX *
                    texture.width);

            centerZ =
                Mathf.RoundToInt(
                    normalizedY *
                    texture.height);

            pixelWidth =
                Mathf.Max(
                    1,
                    Mathf.RoundToInt(
                        normalizedWidth *
                        texture.width));

            pixelHeight =
                Mathf.Max(
                    1,
                    Mathf.RoundToInt(
                        normalizedDepth *
                        texture.height));
        }

        //==========================================================
        // World Coordinate
        //==========================================================

        private float GetWorldCoordinate(
            int pixel,
            int textureDimension,
            float worldMin,
            float worldSize)
        {
            float normalized =
                (pixel + 0.5f) /
                textureDimension;

            return
                worldMin +
                (normalized *
                 worldSize);
        }

        //==========================================================
        // Walkable Texture Sampling
        //==========================================================

        private Color SampleWalkableTexture(
            float worldX,
            float worldZ)
        {
            if (readableWalkableTexture == null)
            {
                throw new InvalidOperationException(
                    "Readable walkable texture has not been prepared.");
            }

            float textureU =
                Mathf.Repeat(
                    worldX /
                    tileSize,
                    1f);

            float textureV =
                Mathf.Repeat(
                    worldZ /
                    tileSize,
                    1f);

            int textureX =
                Mathf.Clamp(
                    Mathf.FloorToInt(
                        textureU *
                        readableWalkableTexture.width),
                    0,
                    readableWalkableTexture.width - 1);

            int textureZ =
                Mathf.Clamp(
                    Mathf.FloorToInt(
                        textureV *
                        readableWalkableTexture.height),
                    0,
                    readableWalkableTexture.height - 1);

            return readableWalkableTexture.GetPixel(
                textureX,
                textureZ);
        }
    }
}