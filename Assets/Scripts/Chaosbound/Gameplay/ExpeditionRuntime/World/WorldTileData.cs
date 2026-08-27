using Chaosbound.Content.World.Themes.TileSets;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World
{
    /// <summary>
    /// Immutable runtime snapshot of a tile
    /// actually materialized by the procedural world generator.
    ///
    /// The minimap mask is copied from the TileEntry
    /// so runtime world state does not share mutable
    /// authoring data.
    /// </summary>
    public sealed class WorldTileData
    {
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

        private readonly bool[] blockedCells;

        public WorldTileData(
            Vector2Int gridPosition,
            Vector2Int footprint,
            TileRotation rotation,
            TileContext context,
            MinimapTileMask minimapMask)
        {
            if (footprint.x <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(footprint),
                    "Footprint X must be greater than zero.");

            if (footprint.y <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(footprint),
                    "Footprint Z must be greater than zero.");

            if (minimapMask == null)
                throw new ArgumentNullException(
                    nameof(minimapMask));

            GridPosition =
                gridPosition;

            Footprint =
                footprint;

            Rotation =
                rotation;

            Context =
                context;

            blockedCells =
                minimapMask.GetCopy();
        }

        /// <summary>
        /// Determines whether a local 4x4 cartographic
        /// cell is blocked in the canonical tile orientation.
        /// </summary>
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

        /// <summary>
        /// Gets a copy of the canonical minimap mask.
        /// </summary>
        public bool[] GetMaskCopy()
        {
            return (bool[])blockedCells.Clone();
        }
    }
}