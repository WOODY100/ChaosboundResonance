using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.World.Themes.Decorations
{
    [CreateAssetMenu(
        fileName = "DecorationProfile",
        menuName = "Chaosbound/World/Decoration Profile")]
    public sealed class DecorationProfile : ScriptableObject
    {
        [Header("Prefab Catalog")]
        [SerializeField]
        private List<DecorationPrefabEntry> props = new();

        [SerializeField]
        private List<DecorationPrefabEntry> obstacles = new();

        [SerializeField]
        private List<DecorationPrefabEntry> largeObstacles = new();

        [SerializeField]
        private List<DecorationPrefabEntry> lights = new();

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

        [Header("Spawn Chances")]
        [Range(0f, 1f)]
        [SerializeField]
        private float largeObstacleChance = 0.25f;

        [Range(0f, 1f)]
        [SerializeField]
        private float lightChance = 0.15f;

        [Header("Placement")]
        [SerializeField]
        private float spawnHeight = 0f;

        [SerializeField]
        private float largeObstacleSpawnHeight = 0f;

        [Min(0f)]
        [SerializeField]
        private float randomOffsetRadius = 0.25f;

        public IReadOnlyList<DecorationPrefabEntry> Props => props;
        public IReadOnlyList<DecorationPrefabEntry> Obstacles => obstacles;
        public IReadOnlyList<DecorationPrefabEntry> LargeObstacles => largeObstacles;
        public IReadOnlyList<DecorationPrefabEntry> Lights => lights;

        public int MinPropsPerTile => minPropsPerTile;
        public int MaxPropsPerTile => maxPropsPerTile;
        public int MinObstaclesPerTile => minObstaclesPerTile;
        public int MaxObstaclesPerTile => maxObstaclesPerTile;

        public float LargeObstacleChance => largeObstacleChance;
        public float LightChance => lightChance;

        public float SpawnHeight => spawnHeight;
        public float LargeObstacleSpawnHeight => largeObstacleSpawnHeight;
        public float RandomOffsetRadius => randomOffsetRadius;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (minPropsPerTile > maxPropsPerTile)
                minPropsPerTile = maxPropsPerTile;

            if (minObstaclesPerTile > maxObstaclesPerTile)
                minObstaclesPerTile = maxObstaclesPerTile;

            props ??= new List<DecorationPrefabEntry>();
            obstacles ??= new List<DecorationPrefabEntry>();
            largeObstacles ??= new List<DecorationPrefabEntry>();
            lights ??= new List<DecorationPrefabEntry>();

            props.RemoveAll(entry => entry == null);
            obstacles.RemoveAll(entry => entry == null);
            largeObstacles.RemoveAll(entry => entry == null);
            lights.RemoveAll(entry => entry == null);
        }
#endif
    }
}