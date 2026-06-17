using System.Collections.Generic;
using UnityEngine;

public class OpenWorldDecorationGenerator : MonoBehaviour
{
    [System.Serializable]
    public class DecorationPrefabEntry
    {
        public GameObject prefab;

        [Min(0)]
        public int weight = 1;
    }

    [Header("Decoration Prefabs")]
    [SerializeField] private List<DecorationPrefabEntry> propPrefabs;
    [SerializeField] private List<DecorationPrefabEntry> obstaclePrefabs;
    [SerializeField] private List<DecorationPrefabEntry> largeObstaclePrefabs;
    [SerializeField] private List<DecorationPrefabEntry> lightPrefabs;

    [Header("Density Per Tile")]
    [SerializeField, Range(0, 9)] private int minPropsPerTile = 2;
    [SerializeField, Range(0, 9)] private int maxPropsPerTile = 4;

    [SerializeField, Range(0, 4)] private int minObstaclesPerTile = 0;
    [SerializeField, Range(0, 4)] private int maxObstaclesPerTile = 1;

    [SerializeField, Range(0f, 1f)] private float largeObstacleChancePerTile = 0.03f;
    [SerializeField, Range(0f, 1f)] private float lightChancePerTile = 0.05f;

    [Header("Placement")]
    [SerializeField] private float spawnHeight = 0.1f;
    [SerializeField] private float largeObstacleSpawnHeight = 0.1f;
    [SerializeField] private float randomOffsetRadius = 0.8f;
    [SerializeField] private Vector2 randomScaleRange = new Vector2(0.85f, 1.15f);
    [SerializeField] private Vector2 largeObstacleScaleRange = new Vector2(1f, 1f);
    [SerializeField] private bool randomRotation = true;

    [Header("Generation")]
    [SerializeField] private bool clearBeforeGenerate = true;

    private Transform floorsParent;
    private Transform propsParent;
    private Transform obstaclesParent;
    private Transform lightsParent;

    private readonly List<GameObject> spawnedObjects = new List<GameObject>();

    [ContextMenu("Generate Decoration")]
    public void GenerateDecoration()
    {
        if (!FindGeneratedParents())
            return;

        if (clearBeforeGenerate)
            ClearDecoration();

        ValidateDensity();

        TileDecorationPoints[] tiles =
            floorsParent.GetComponentsInChildren<TileDecorationPoints>();

        foreach (TileDecorationPoints tile in tiles)
        {
            DecorateTile(tile);
        }

        Debug.Log($"Decoration generated. Tiles decorated: {tiles.Length}");
    }

    [ContextMenu("Clear Decoration")]
    public void ClearDecoration()
    {
        if (!FindGeneratedParents())
            return;

        ClearParent(propsParent);
        ClearParent(obstaclesParent);
        ClearParent(lightsParent);

        spawnedObjects.Clear();
    }

    private bool FindGeneratedParents()
    {
        Transform generatedMap = transform.Find("Generated_Map");

        if (generatedMap == null)
            generatedMap = transform;

        floorsParent = generatedMap.Find("Terrain/Floors");
        propsParent = generatedMap.Find("Decoration/Props");
        obstaclesParent = generatedMap.Find("Decoration/Obstacles");
        lightsParent = generatedMap.Find("Decoration/Lights");

        if (floorsParent == null || propsParent == null || obstaclesParent == null || lightsParent == null)
        {
            Debug.LogWarning("OpenWorldDecorationGenerator: Missing generated hierarchy references.");
            return false;
        }

        return true;
    }

    private void ValidateDensity()
    {
        if (minPropsPerTile > maxPropsPerTile)
            minPropsPerTile = maxPropsPerTile;

        if (minObstaclesPerTile > maxObstaclesPerTile)
            minObstaclesPerTile = maxObstaclesPerTile;

        if (randomScaleRange.x > randomScaleRange.y)
        {
            float temp = randomScaleRange.x;
            randomScaleRange.x = randomScaleRange.y;
            randomScaleRange.y = temp;
        }

        if (largeObstacleScaleRange.x > largeObstacleScaleRange.y)
        {
            float temp = largeObstacleScaleRange.x;
            largeObstacleScaleRange.x = largeObstacleScaleRange.y;
            largeObstacleScaleRange.y = temp;
        }
    }

    private void DecorateTile(TileDecorationPoints tile)
    {
        if (TrySpawnLargeObstacle(tile))
            return;

        Transform[] points = tile.SpawnPoints;

        if (points == null || points.Length == 0)
            return;

        List<Transform> availablePoints = new List<Transform>(points);

        int propsToSpawn = Random.Range(minPropsPerTile, maxPropsPerTile + 1);
        int obstaclesToSpawn = Random.Range(minObstaclesPerTile, maxObstaclesPerTile + 1);

        SpawnFromPoints(propPrefabs, propsParent, availablePoints, propsToSpawn);
        SpawnFromPoints(obstaclePrefabs, obstaclesParent, availablePoints, obstaclesToSpawn);

        if (Random.value <= lightChancePerTile)
            SpawnFromPoints(lightPrefabs, lightsParent, availablePoints, 1);
    }

    private bool TrySpawnLargeObstacle(TileDecorationPoints tile)
    {
        if (largeObstaclePrefabs == null || largeObstaclePrefabs.Count == 0)
            return false;

        if (tile.CenterPoint == null)
            return false;

        if (Random.value > largeObstacleChancePerTile)
            return false;

        GameObject prefab = GetWeightedRandom(largeObstaclePrefabs);

        if (prefab == null)
            return false;

        Vector3 spawnPosition = tile.CenterPoint.position;
        spawnPosition.y = largeObstacleSpawnHeight;

        Quaternion spawnRotation = randomRotation
            ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
            : prefab.transform.rotation;

        GameObject instance = Instantiate(prefab, spawnPosition, spawnRotation, obstaclesParent);

        float scale = Random.Range(largeObstacleScaleRange.x, largeObstacleScaleRange.y);
        instance.transform.localScale = prefab.transform.localScale * scale;

        spawnedObjects.Add(instance);

        return true;
    }

    private void SpawnFromPoints(
        List<DecorationPrefabEntry> prefabs,
        Transform parent,
        List<Transform> availablePoints,
        int amount)
    {
        if (prefabs == null || prefabs.Count == 0)
            return;

        if (parent == null)
            return;

        amount = Mathf.Min(amount, availablePoints.Count);

        for (int i = 0; i < amount; i++)
        {
            Transform point = TakeRandomPoint(availablePoints);

            if (point == null)
                return;

            GameObject prefab = GetWeightedRandom(prefabs);

            if (prefab == null)
                continue;

            Vector3 offset = new Vector3(
                Random.Range(-randomOffsetRadius, randomOffsetRadius),
                0f,
                Random.Range(-randomOffsetRadius, randomOffsetRadius)
            );

            Vector3 spawnPosition = point.position + offset;
            spawnPosition.y = spawnHeight;

            Quaternion spawnRotation = randomRotation
                ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                : prefab.transform.rotation;

            GameObject instance = Instantiate(prefab, spawnPosition, spawnRotation, parent);

            float scale = Random.Range(randomScaleRange.x, randomScaleRange.y);
            instance.transform.localScale = prefab.transform.localScale * scale;

            spawnedObjects.Add(instance);
        }
    }

    private GameObject GetWeightedRandom(List<DecorationPrefabEntry> entries)
    {
        int totalWeight = 0;

        foreach (DecorationPrefabEntry entry in entries)
        {
            if (entry == null || entry.prefab == null || entry.weight <= 0)
                continue;

            totalWeight += entry.weight;
        }

        if (totalWeight <= 0)
            return null;

        int roll = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (DecorationPrefabEntry entry in entries)
        {
            if (entry == null || entry.prefab == null || entry.weight <= 0)
                continue;

            currentWeight += entry.weight;

            if (roll < currentWeight)
                return entry.prefab;
        }

        return null;
    }

    private Transform TakeRandomPoint(List<Transform> points)
    {
        if (points.Count == 0)
            return null;

        int index = Random.Range(0, points.Count);
        Transform selected = points[index];
        points.RemoveAt(index);

        return selected;
    }

    private void ClearParent(Transform parent)
    {
        if (parent == null)
            return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(parent.GetChild(i).gameObject);
            else
                DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
}