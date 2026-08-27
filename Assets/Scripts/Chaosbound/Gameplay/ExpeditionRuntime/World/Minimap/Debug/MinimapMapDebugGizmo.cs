using Chaosbound.Content.World.Themes.TileSets;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Builders;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Data;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Debug
{
    /// <summary>
    /// Editor/debug visualization of MinimapMapData.
    ///
    /// This component does not participate in runtime gameplay.
    /// It visualizes the cartographic representation generated
    /// from the procedural world.
    /// </summary>
    [ExecuteAlways]
    public sealed class MinimapMapDebugGizmo :
        MonoBehaviour
    {
        [Header("Debug Source")]
        [SerializeField]
        private OpenWorldMapGenerator mapGenerator;

        [Header("Visualization")]
        [SerializeField]
        private bool showBlockedCells = true;

        [SerializeField]
        private bool showWalkableGrid = true;

        [SerializeField]
        private bool showTileFootprints = true;

        [SerializeField]
        private bool showWorldBounds = true;

        [Header("World")]
        [Min(0.01f)]
        [SerializeField]
        private float tileSize = 12f;

        [Header("Height")]
        [SerializeField]
        private float gizmoHeight = 0.25f;

        [SerializeField]
        private float cellHeight = 0.08f;

        private MinimapMapData mapData;

        private void OnDrawGizmos()
        {
            RefreshMapData();

            if (mapData == null)
                return;

            DrawMap();
        }

        /// <summary>
        /// Assigns a MinimapMapData snapshot directly.
        ///
        /// Intended for runtime systems and isolated debug tests.
        /// </summary>
        public void SetMapData(
            MinimapMapData data)
        {
            mapData =
                data
                ?? throw new ArgumentNullException(
                    nameof(data));
        }

        private void RefreshMapData()
        {
            if (mapGenerator == null)
                return;

            WorldLayout layout =
                mapGenerator.WorldLayout;

            if (layout == null)
            {
                mapData = null;
                return;
            }

            MinimapMapBuilder builder =
                new MinimapMapBuilder();

            mapData =
                builder.Build(
                    layout,
                    mapGenerator.GeneratedWorldBounds);
        }

        private void DrawMap()
        {
            if (showWorldBounds)
            {
                DrawWorldBounds(
                    mapData.WorldBounds);
            }

            foreach (
                MinimapTileData tile
                in mapData.Tiles)
            {
                DrawTile(tile);
            }
        }

        private void DrawWorldBounds(
            Bounds bounds)
        {
            Vector3 center =
                bounds.center;

            center.y =
                transform.position.y +
                gizmoHeight;

            Vector3 size =
                bounds.size;

            size.y = 0f;

            Gizmos.DrawWireCube(
                center,
                size);
        }

        private void DrawTile(
            MinimapTileData tile)
        {
            Vector3 tileCenter =
                GetTileCenter(tile);

            if (showTileFootprints)
            {
                DrawTileFootprint(
                    tileCenter,
                    tile);
            }

            DrawMask(
                tileCenter,
                tile);
        }

        private Vector3 GetTileCenter(
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
                bounds.min.y + gizmoHeight,
                centerZ + footprintOffsetZ);
        }

        private void DrawTileFootprint(
            Vector3 center,
            MinimapTileData tile)
        {
            Vector3 size =
                new Vector3(
                    tile.Footprint.x *
                    tileSize,
                    0f,
                    tile.Footprint.y *
                    tileSize);

            Gizmos.DrawWireCube(
                center,
                size);
        }

        private void DrawMask(
            Vector3 tileCenter,
            MinimapTileData tile)
        {
            float totalWidth =
                tile.Footprint.x *
                tileSize;

            float totalDepth =
                tile.Footprint.y *
                tileSize;

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
                        tile.IsBlocked(
                            x,
                            z);

                    if (!blocked &&
                        !showWalkableGrid)
                    {
                        continue;
                    }

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
                        if (showBlockedCells)
                        {
                            DrawBlockedCell(
                                worldPosition,
                                cellWidth,
                                cellDepth);
                        }
                    }
                    else
                    {
                        DrawWalkableCell(
                            worldPosition,
                            cellWidth,
                            cellDepth);
                    }
                }
            }
        }

        private void DrawBlockedCell(
            Vector3 position,
            float width,
            float depth)
        {
            Gizmos.DrawCube(
                position,
                new Vector3(
                    width,
                    cellHeight,
                    depth));
        }

        private void DrawWalkableCell(
            Vector3 position,
            float width,
            float depth)
        {
            Gizmos.DrawWireCube(
                position,
                new Vector3(
                    width,
                    0.01f,
                    depth));
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
    }
}