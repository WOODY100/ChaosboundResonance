using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.World.Themes.Decorations
{
    [CreateAssetMenu(
        fileName = "DecorationProfile",
        menuName = "Chaosbound/World/Decoration Profile")]
    public sealed class DecorationProfile : ScriptableObject
    {
        //==========================================================
        // Prefab Catalog
        //==========================================================

        [Header("Prefab Catalog")]

        [SerializeField]
        private List<DecorationPrefabEntry> props = new();

        [SerializeField]
        private List<DecorationPrefabEntry> obstacles = new();

        [SerializeField]
        private List<DecorationPrefabEntry> largeObstacles = new();

        [SerializeField]
        private List<DecorationPrefabEntry> lights = new();

        [SerializeField]
        private List<DecorationPrefabEntry> modifierStructures = new();

        //==========================================================
        // Density
        //==========================================================

        [Header("Density")]

        [Min(0)]
        [SerializeField]
        private int minPropsPerTile = 1;

        [Min(0)]
        [SerializeField]
        private int maxPropsPerTile = 3;

        [Min(0)]
        [SerializeField]
        private int minObstaclesPerTile = 0;

        [Min(0)]
        [SerializeField]
        private int maxObstaclesPerTile = 2;

        [Min(0)]
        [SerializeField]
        private int maxModifierStructuresPerTile = 1;

        //==========================================================
        // Spawn Chances
        //==========================================================

        [Header("Spawn Chances")]

        [Range(0f, 1f)]
        [SerializeField]
        private float largeObstacleChance = 0.25f;

        [Range(0f, 1f)]
        [SerializeField]
        private float lightChance = 0.15f;

        [Range(0f, 1f)]
        [SerializeField]
        private float modifierStructureChance = 0.08f;

        //==========================================================
        // Decoration Separation
        //==========================================================

        [Header("Decoration Separation")]

        [Range(0, 8)]
        [SerializeField]
        private int maxBlockedNeighbors = 4;

        //==========================================================
        // Placement
        //==========================================================

        [Header("Placement")]

        [SerializeField]
        private float spawnHeight = 0f;

        [SerializeField]
        private float largeObstacleSpawnHeight = 0f;

        [Min(0f)]
        [SerializeField]
        private float randomOffsetRadius = 0.25f;

        //==========================================================
        // Prefab Catalog Properties
        //==========================================================

        public IReadOnlyList<DecorationPrefabEntry> Props =>
            props;

        public IReadOnlyList<DecorationPrefabEntry> Obstacles =>
            obstacles;

        public IReadOnlyList<DecorationPrefabEntry> LargeObstacles =>
            largeObstacles;

        public IReadOnlyList<DecorationPrefabEntry> Lights =>
            lights;

        public IReadOnlyList<DecorationPrefabEntry> ModifierStructures =>
            modifierStructures;

        //==========================================================
        // Density Properties
        //==========================================================

        public int MinPropsPerTile =>
            minPropsPerTile;

        public int MaxPropsPerTile =>
            maxPropsPerTile;

        public int MinObstaclesPerTile =>
            minObstaclesPerTile;

        public int MaxObstaclesPerTile =>
            maxObstaclesPerTile;

        public int MaxModifierStructuresPerTile =>
            maxModifierStructuresPerTile;

        //==========================================================
        // Spawn Chance Properties
        //==========================================================

        public float LargeObstacleChance =>
            largeObstacleChance;

        public float LightChance =>
            lightChance;

        public float ModifierStructureChance =>
            modifierStructureChance;

        //==========================================================
        // Decoration Separation Properties
        //==========================================================

        public int MaxBlockedNeighbors =>
            maxBlockedNeighbors;

        //==========================================================
        // Placement Properties
        //==========================================================

        public float SpawnHeight =>
            spawnHeight;

        public float LargeObstacleSpawnHeight =>
            largeObstacleSpawnHeight;

        public float RandomOffsetRadius =>
            randomOffsetRadius;

#if UNITY_EDITOR

        //==========================================================
        // Validation
        //==========================================================

        private void OnValidate()
        {
            if (minPropsPerTile > maxPropsPerTile)
                minPropsPerTile = maxPropsPerTile;

            if (minObstaclesPerTile > maxObstaclesPerTile)
                minObstaclesPerTile = maxObstaclesPerTile;

            maxModifierStructuresPerTile =
                Mathf.Max(
                    0,
                    maxModifierStructuresPerTile);

            maxBlockedNeighbors =
                Mathf.Clamp(
                    maxBlockedNeighbors,
                    0,
                    8);

            props ??=
                new List<DecorationPrefabEntry>();

            obstacles ??=
                new List<DecorationPrefabEntry>();

            largeObstacles ??=
                new List<DecorationPrefabEntry>();

            lights ??=
                new List<DecorationPrefabEntry>();

            modifierStructures ??=
                new List<DecorationPrefabEntry>();

            props.RemoveAll(
                entry => entry == null);

            obstacles.RemoveAll(
                entry => entry == null);

            largeObstacles.RemoveAll(
                entry => entry == null);

            lights.RemoveAll(
                entry => entry == null);

            modifierStructures.RemoveAll(
                entry => entry == null);
        }
#endif
    }
}