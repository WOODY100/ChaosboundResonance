using System.Collections.Generic;
using UnityEngine;

public class OpenWorldMapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class TilePrefabEntry
    {
        public GameObject prefab;

        [Min(0)]
        public int weight = 1;
    }

    [Header("Map Settings")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float tileSize = 12f;
    [SerializeField] private bool centerMapOnOrigin = true;

    [Header("Center Tile Prefabs")]
    [SerializeField] private List<TilePrefabEntry> centerTilePrefabs;

    [Header("Border Prefabs")]
    [SerializeField] private List<GameObject> edgePrefabs;
    [SerializeField] private List<GameObject> cornerPrefabs;

    [Header("Generated Parent")]
    [SerializeField] private Transform generatedParent;

    [Header("Generation")]
    [SerializeField] private bool generateOnStart = true;
    [SerializeField] private bool clearBeforeGenerate = true;
    [SerializeField] private bool generateDecorationAfterMap = true;

    private Transform terrainParent;
    private Transform floorsParent;
    private Transform edgesParent;
    private Transform cornersParent;

    private Transform decorationParent;
    private Transform propsParent;
    private Transform obstaclesParent;
    private Transform lightsParent;

    private void Start()
    {
        if (generateOnStart)
            GenerateMap();
    }

    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        if (clearBeforeGenerate)
            ClearMap();

        CreateGeneratedHierarchy();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                SpawnTile(x, z);
            }
        }

        if (generateDecorationAfterMap)
        {
            OpenWorldDecorationGenerator decorationGenerator =
                GetComponent<OpenWorldDecorationGenerator>();

            if (decorationGenerator != null)
                decorationGenerator.GenerateDecoration();
        }
    }

    private void CreateGeneratedHierarchy()
    {
        if (generatedParent == null)
        {
            GameObject parent = new GameObject("Generated_Map");
            parent.transform.SetParent(transform);
            parent.transform.localPosition = Vector3.zero;
            generatedParent = parent.transform;
        }

        terrainParent = CreateChild(generatedParent, "Terrain");
        floorsParent = CreateChild(terrainParent, "Floors");
        edgesParent = CreateChild(terrainParent, "Edges");
        cornersParent = CreateChild(terrainParent, "Corners");

        decorationParent = CreateChild(generatedParent, "Decoration");
        propsParent = CreateChild(decorationParent, "Props");
        obstaclesParent = CreateChild(decorationParent, "Obstacles");
        lightsParent = CreateChild(decorationParent, "Lights");
    }

    private Transform CreateChild(Transform parent, string childName)
    {
        Transform existing = parent.Find(childName);

        if (existing != null)
            return existing;

        GameObject child = new GameObject(childName);
        child.transform.SetParent(parent);
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;

        return child.transform;
    }

    private void SpawnTile(int x, int z)
    {
        bool isWest = x == 0;
        bool isEast = x == width - 1;
        bool isSouth = z == 0;
        bool isNorth = z == height - 1;

        Vector3 position = GetWorldPosition(x, z);
        GameObject prefab;
        Quaternion rotation = Quaternion.identity;
        Transform parent;

        if (isWest && isSouth)
        {
            prefab = GetRandom(cornerPrefabs);
            rotation = Quaternion.Euler(0f, 0f, 0f);
            parent = cornersParent;
        }
        else if (isEast && isSouth)
        {
            prefab = GetRandom(cornerPrefabs);
            rotation = Quaternion.Euler(0f, 270f, 0f);
            parent = cornersParent;
        }
        else if (isEast && isNorth)
        {
            prefab = GetRandom(cornerPrefabs);
            rotation = Quaternion.Euler(0f, 180f, 0f);
            parent = cornersParent;
        }
        else if (isWest && isNorth)
        {
            prefab = GetRandom(cornerPrefabs);
            rotation = Quaternion.Euler(0f, 90f, 0f);
            parent = cornersParent;
        }
        else if (isWest)
        {
            prefab = GetRandom(edgePrefabs);
            rotation = Quaternion.Euler(0f, 0f, 0f);
            parent = edgesParent;
        }
        else if (isNorth)
        {
            prefab = GetRandom(edgePrefabs);
            rotation = Quaternion.Euler(0f, 90f, 0f);
            parent = edgesParent;
        }
        else if (isEast)
        {
            prefab = GetRandom(edgePrefabs);
            rotation = Quaternion.Euler(0f, 180f, 0f);
            parent = edgesParent;
        }
        else if (isSouth)
        {
            prefab = GetRandom(edgePrefabs);
            rotation = Quaternion.Euler(0f, 270f, 0f);
            parent = edgesParent;
        }
        else
        {
            prefab = GetWeightedRandom(centerTilePrefabs);
            parent = floorsParent;
        }

        if (prefab == null)
            return;

        Instantiate(prefab, position, rotation, parent);
    }

    private Vector3 GetWorldPosition(int x, int z)
    {
        float posX = x * tileSize;
        float posZ = z * tileSize;

        if (centerMapOnOrigin)
        {
            posX -= (width - 1) * tileSize * 0.5f;
            posZ -= (height - 1) * tileSize * 0.5f;
        }

        return new Vector3(posX, 0f, posZ);
    }

    private GameObject GetRandom(List<GameObject> prefabs)
    {
        if (prefabs == null || prefabs.Count == 0)
            return null;

        return prefabs[Random.Range(0, prefabs.Count)];
    }

    private GameObject GetWeightedRandom(List<TilePrefabEntry> entries)
    {
        if (entries == null || entries.Count == 0)
            return null;

        int totalWeight = 0;

        foreach (TilePrefabEntry entry in entries)
        {
            if (entry == null || entry.prefab == null || entry.weight <= 0)
                continue;

            totalWeight += entry.weight;
        }

        if (totalWeight <= 0)
            return null;

        int roll = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (TilePrefabEntry entry in entries)
        {
            if (entry == null || entry.prefab == null || entry.weight <= 0)
                continue;

            currentWeight += entry.weight;

            if (roll < currentWeight)
                return entry.prefab;
        }

        return null;
    }

    [ContextMenu("Clear Map")]
    public void ClearMap()
    {
        if (generatedParent == null)
            return;

        for (int i = generatedParent.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(generatedParent.GetChild(i).gameObject);
            else
                DestroyImmediate(generatedParent.GetChild(i).gameObject);
        }

        terrainParent = null;
        floorsParent = null;
        edgesParent = null;
        cornersParent = null;

        decorationParent = null;
        propsParent = null;
        obstaclesParent = null;
        lightsParent = null;
    }
}