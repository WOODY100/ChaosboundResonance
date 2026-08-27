using Chaosbound.Content.World.Themes.TileSets;
using Chaosbound.Gameplay.ExpeditionRuntime.World;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Data
{
    /// <summary>
    /// Immutable cartographic representation of a
    /// generated world tile.
    /// </summary>
    public sealed class MinimapTileData
    {
        private readonly bool[] blockedCells;

        public Vector2Int GridPosition
        {
            get;
        }

        public Vector2Int Footprint
        {
            get;
        }

        public TileRotation Rotation
        {
            get;
        }

        public TileContext Context
        {
            get;
        }

        public MinimapTileData(
            WorldTileData source)
        {
            if (source == null)
                throw new ArgumentNullException(
                    nameof(source));

            GridPosition =
                source.GridPosition;

            Footprint =
                source.Footprint;

            Rotation =
                source.Rotation;

            Context =
                source.Context;

            blockedCells =
                source.GetMaskCopy();
        }

        public bool IsBlocked(
            int x,
            int z)
        {
            if (x < 0 ||
                x >= MinimapTileMask.Resolution)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(x));
            }

            if (z < 0 ||
                z >= MinimapTileMask.Resolution)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(z));
            }

            return blockedCells[
                (z * MinimapTileMask.Resolution) + x];
        }

        public bool[] GetMaskCopy()
        {
            return (bool[])blockedCells.Clone();
        }
    }
}