using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.World.Themes.TileSets
{
    [CreateAssetMenu(
        fileName = "TileSetProfile",
        menuName = "Chaosbound/World/Tile Set Profile")]
    public sealed class TileSetProfile : ScriptableObject
    {
        [Header("Center Spawn Tile")]
        [SerializeField]
        private TileEntry centerSpawnTile;

        [Header("Center Tiles")]
        [SerializeField]
        private List<TileEntry> centerTiles = new();

        [Header("Edge Tiles")]
        [SerializeField]
        private List<TileEntry> edgeTiles = new();

        [Header("Corner Tiles")]
        [SerializeField]
        private List<TileEntry> cornerTiles = new();

        public TileEntry CenterSpawnTile => centerSpawnTile;

        public IReadOnlyList<TileEntry> CenterTiles => centerTiles;

        public IReadOnlyList<TileEntry> EdgeTiles => edgeTiles;

        public IReadOnlyList<TileEntry> CornerTiles => cornerTiles;

        public IReadOnlyList<TileEntry> GetTiles(TileContext context)
        {
            return context switch
            {
                TileContext.Center => centerTiles,
                TileContext.Edge => edgeTiles,
                TileContext.Corner => cornerTiles,

                _ => throw new ArgumentOutOfRangeException(
                    nameof(context),
                    context,
                    "Unsupported TileContext.")
            };
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            centerTiles ??= new List<TileEntry>();
            edgeTiles ??= new List<TileEntry>();
            cornerTiles ??= new List<TileEntry>();

            centerTiles.RemoveAll(entry => entry == null);
            edgeTiles.RemoveAll(entry => entry == null);
            cornerTiles.RemoveAll(entry => entry == null);

            ValidateCenterSpawnTile();

            ValidateCollection(centerTiles, "Center");
            ValidateCollection(edgeTiles, "Edge");
            ValidateCollection(cornerTiles, "Corner");
        }

        private void ValidateCenterSpawnTile()
        {
            if (centerSpawnTile == null)
            {
                Debug.LogWarning(
                    $"TileSetProfile '{name}' has no Center Spawn Tile configured.",
                    this);

                return;
            }

            if (centerSpawnTile.SizeX != 1 ||
                centerSpawnTile.SizeZ != 1)
            {
                Debug.LogError(
                    $"TileSetProfile '{name}' Center Spawn Tile must have a 1x1 footprint.",
                    this);
            }

            if (centerSpawnTile.AllowRotate90)
            {
                Debug.LogError(
                    $"TileSetProfile '{name}' Center Spawn Tile must not allow 90° rotation.",
                    this);
            }

            if (centerSpawnTile.RandomYRotation)
            {
                Debug.LogError(
                    $"TileSetProfile '{name}' Center Spawn Tile must not use random Y rotation.",
                    this);
            }
        }

        private void ValidateCollection(
            List<TileEntry> collection,
            string name)
        {
            if (collection.Count == 0)
            {
                Debug.LogWarning(
                    $"TileSetProfile '{this.name}' has no {name} Tiles configured.",
                    this);
            }
        }
#endif
    }
}