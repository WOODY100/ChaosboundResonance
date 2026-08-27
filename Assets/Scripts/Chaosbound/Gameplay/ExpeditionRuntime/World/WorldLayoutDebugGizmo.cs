using Chaosbound.Content.World.Themes.TileSets;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World
{
    /// <summary>
    /// Editor/debug visualization of the logical WorldLayout.
    ///
    /// This component does not participate in runtime gameplay.
    /// It visualizes the cartographic masks produced by the
    /// procedural world generator directly in the Scene View.
    /// </summary>
    [ExecuteAlways]
    public sealed class WorldLayoutDebugGizmo :
        MonoBehaviour
    {
        [Header("Source")]
        [SerializeField]
        private OpenWorldMapGenerator mapGenerator;

        [Header("Visualization")]
        [SerializeField]
        private bool showMask = true;

        [SerializeField]
        private bool showGrid = true;

        [SerializeField]
        private bool showFootprint = true;

        [SerializeField]
        private bool showOnlyBlockedCells = true;

        [Header("World")]
        [Min(0.01f)]
        [SerializeField]
        private float tileSize = 12f;

        [Header("Height")]
        [SerializeField]
        private float gizmoHeight = 0.15f;

        [Header("Cell")]
        [SerializeField]
        private float cellHeight = 0.05f;

        private void OnDrawGizmos()
        {
            if (mapGenerator == null)
                return;

            WorldLayout layout =
                mapGenerator.WorldLayout;

            if (layout == null)
                return;

            foreach (
                WorldTileData tile
                in layout.Tiles)
            {
                DrawTile(
                    layout,
                    tile);
            }
        }

        private void DrawTile(
            WorldLayout layout,
            WorldTileData tile)
        {
            Vector3 tileCenter =
                GetTileCenter(
                    layout,
                    tile);

            if (showFootprint)
            {
                DrawFootprint(
                    tileCenter,
                    tile);
            }

            if (!showMask)
                return;

            DrawMask(
                tileCenter,
                tile);
        }

        private Vector3 GetTileCenter(
            WorldLayout layout,
            WorldTileData tile)
        {
            Bounds bounds =
                mapGenerator.GeneratedWorldBounds;

            float firstTileCenterX =
                bounds.min.x +
                (tileSize * 0.5f);

            float firstTileCenterZ =
                bounds.min.z +
                (tileSize * 0.5f);

            float centerX =
                firstTileCenterX +
                (tile.GridPosition.x * tileSize);

            float centerZ =
                firstTileCenterZ +
                (tile.GridPosition.y * tileSize);

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
                bounds.min.y + gizmoHeight,
                centerZ + footprintOffsetZ);
        }

        private void DrawFootprint(
            Vector3 center,
            WorldTileData tile)
        {
            Vector3 size =
                new Vector3(
                    tile.Footprint.x * tileSize,
                    0f,
                    tile.Footprint.y * tileSize);

            Gizmos.DrawWireCube(
                center,
                size);
        }

        private void DrawMask(
            Vector3 tileCenter,
            WorldTileData tile)
        {
            float totalWidth =
                tile.Footprint.x * tileSize;

            float totalDepth =
                tile.Footprint.y * tileSize;

            float cellWidth =
                totalWidth /
                MinimapTileMask.Resolution;

            float cellDepth =
                totalDepth /
                MinimapTileMask.Resolution;

            for (int z = 0;
                 z < MinimapTileMask.Resolution;
                 z++)
            {
                for (int x = 0;
                     x < MinimapTileMask.Resolution;
                     x++)
                {
                    bool blocked =
                        tile.IsBlocked(x, z);

                    if (showOnlyBlockedCells &&
                        !blocked)
                    {
                        if (showGrid)
                        {
                            DrawGridCell(
                                tileCenter,
                                tile,
                                x,
                                z,
                                cellWidth,
                                cellDepth);
                        }

                        continue;
                    }

                    if (blocked)
                    {
                        DrawBlockedCell(
                            tileCenter,
                            tile,
                            x,
                            z,
                            cellWidth,
                            cellDepth);
                    }
                    else if (showGrid)
                    {
                        DrawGridCell(
                            tileCenter,
                            tile,
                            x,
                            z,
                            cellWidth,
                            cellDepth);
                    }
                }
            }
        }

        private void DrawBlockedCell(
            Vector3 tileCenter,
            WorldTileData tile,
            int x,
            int z,
            float cellWidth,
            float cellDepth)
        {
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

            Gizmos.DrawCube(
                worldPosition,
                new Vector3(
                    cellWidth,
                    cellHeight,
                    cellDepth));
        }

        private void DrawGridCell(
            Vector3 tileCenter,
            WorldTileData tile,
            int x,
            int z,
            float cellWidth,
            float cellDepth)
        {
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

            Gizmos.DrawWireCube(
                worldPosition,
                new Vector3(
                    cellWidth,
                    0.01f,
                    cellDepth));
        }

        private Vector3 GetCellLocalPosition(
            WorldTileData tile,
            int x,
            int z,
            float cellWidth,
            float cellDepth)
        {
            float localX =
                (-tile.Footprint.x * tileSize * 0.5f) +
                (cellWidth * 0.5f) +
                (x * cellWidth);

            float localZ =
                (-tile.Footprint.y * tileSize * 0.5f) +
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
    }
}