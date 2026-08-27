using Chaosbound.Gameplay.ExpeditionRuntime.World;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Builders
{
    /// <summary>
    /// Builds the static cartographic minimap representation
    /// from the generated WorldLayout.
    /// </summary>
    public sealed class MinimapMapBuilder
    {
        public MinimapMapData Build(
            WorldLayout layout,
            Bounds worldBounds)
        {
            if (layout == null)
                throw new ArgumentNullException(
                    nameof(layout));

            List<MinimapTileData> tiles =
                new List<MinimapTileData>(
                    layout.TileCount);

            foreach (
                WorldTileData worldTile
                in layout.Tiles)
            {
                if (worldTile == null)
                    continue;

                tiles.Add(
                    new MinimapTileData(
                        worldTile));
            }

            return new MinimapMapData(
                layout.Width,
                layout.Height,
                worldBounds,
                tiles);
        }
    }
}