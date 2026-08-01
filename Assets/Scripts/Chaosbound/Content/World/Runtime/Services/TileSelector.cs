using System;
using System.Collections.Generic;
using Chaosbound.Content.World.Themes.TileSets;
using UnityEngine;

namespace Chaosbound.Content.World.Runtime.Services
{
    public sealed class TileSelector
    {
        public TileEntry Select(
            TileSetProfile profile,
            TileContext context)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            IReadOnlyList<TileEntry> tiles = profile.GetTiles(context);

            if (tiles.Count == 0)
            {
                throw new InvalidOperationException(
                    $"TileSetProfile contains no tiles for context '{context}'.");
            }

            int totalWeight = CalculateTotalWeight(tiles);

            return SelectByWeight(
                tiles,
                totalWeight);
        }

        private static int CalculateTotalWeight(
            IReadOnlyList<TileEntry> tiles)
        {
            int total = 0;

            foreach (TileEntry tile in tiles)
            {
                total += tile.Weight;
            }

            return total;
        }

        private static TileEntry SelectByWeight(
            IReadOnlyList<TileEntry> tiles,
            int totalWeight)
        {
            int roll = UnityEngine.Random.Range(0, totalWeight);

            int accumulatedWeight = 0;

            foreach (TileEntry tile in tiles)
            {
                accumulatedWeight += tile.Weight;

                if (roll < accumulatedWeight)
                {
                    return tile;
                }
            }

            throw new InvalidOperationException(
                "Failed to select a TileEntry using weighted selection.");
        }
    }
}