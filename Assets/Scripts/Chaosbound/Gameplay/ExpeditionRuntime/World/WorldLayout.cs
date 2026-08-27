using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World
{
    /// <summary>
    /// Represents the logical layout produced by
    /// the procedural world generator.
    ///
    /// This class contains world-generation information,
    /// not minimap presentation.
    /// </summary>
    public sealed class WorldLayout
    {
        private readonly List<WorldTileData> tiles =
            new List<WorldTileData>();

        public int Width
        {
            get;
        }

        public int Height
        {
            get;
        }

        public IReadOnlyList<WorldTileData> Tiles =>
            tiles;

        public int TileCount =>
            tiles.Count;

        public WorldLayout(
            int width,
            int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(height));

            Width =
                width;

            Height =
                height;
        }

        public void Add(
            WorldTileData tile)
        {
            if (tile == null)
                throw new ArgumentNullException(
                    nameof(tile));

            tiles.Add(tile);
        }

        public void Clear()
        {
            tiles.Clear();
        }
    }
}