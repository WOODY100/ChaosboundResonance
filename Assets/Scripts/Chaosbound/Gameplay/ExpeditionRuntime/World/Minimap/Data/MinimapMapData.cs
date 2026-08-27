using Chaosbound.Gameplay.ExpeditionRuntime.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Data
{
    /// <summary>
    /// Immutable cartographic representation of the
    /// generated expedition world.
    ///
    /// This data represents the static world geometry
    /// used by the minimap. It does not contain dynamic
    /// gameplay markers.
    /// </summary>
    public sealed class MinimapMapData
    {
        private readonly List<MinimapTileData> tiles;

        public int Width
        {
            get;
        }

        public int Height
        {
            get;
        }

        public IReadOnlyList<MinimapTileData> Tiles =>
            tiles;

        public int TileCount =>
            tiles.Count;

        public Bounds WorldBounds
        {
            get;
        }

        public MinimapMapData(
            int width,
            int height,
            Bounds worldBounds,
            IReadOnlyList<MinimapTileData> tiles)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(height));

            if (tiles == null)
                throw new ArgumentNullException(
                    nameof(tiles));

            Width = width;
            Height = height;
            WorldBounds = worldBounds;

            this.tiles =
                new List<MinimapTileData>(
                    tiles);
        }
    }
}