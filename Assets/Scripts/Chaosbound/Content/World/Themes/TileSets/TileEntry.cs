using System;
using UnityEngine;

namespace Chaosbound.Content.World.Themes.TileSets
{
    [Serializable]
    public class TileEntry
    {
        [Header("Tile")]
        [SerializeField]
        private GameObject prefab;

        [Header("Selection")]
        [Min(1)]
        [SerializeField]
        private int weight = 1;

        [Header("Footprint")]
        [Min(1)]
        [SerializeField]
        private int sizeX = 1;

        [Min(1)]
        [SerializeField]
        private int sizeZ = 1;

        [Header("Placement")]
        [SerializeField]
        private bool allowRotate90 = false;

        [SerializeField]
        private bool randomYRotation = false;

        [Header("Minimap")]
        [SerializeField]
        private MinimapTileMask minimapMask = new MinimapTileMask();

        public GameObject Prefab => prefab;

        public int Weight => weight;

        public int SizeX => sizeX;

        public int SizeZ => sizeZ;

        public bool AllowRotate90 => allowRotate90;

        public bool RandomYRotation => randomYRotation;

        public MinimapTileMask MinimapMask
        {
            get
            {
                if (minimapMask == null)
                    minimapMask = new MinimapTileMask();

                minimapMask.EnsureStorage();
                return minimapMask;
            }
        }
    }
}
