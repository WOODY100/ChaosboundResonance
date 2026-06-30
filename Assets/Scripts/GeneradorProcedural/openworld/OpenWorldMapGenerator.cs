using System.Collections.Generic;
using UnityEngine;

public class OpenWorldMapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class TilePrefabEntry
    {
        public GameObject prefab;

        [Min(1)] public int sizeX = 1;
        [Min(1)] public int sizeZ = 1;

        [Min(0)] public int weight = 1;

        public bool allowRotate90 = false;
        public bool randomYRotation = false;
    }

    [System.Serializable]
    public class TileCategory
    {
        public string categoryName = "Floors";
        public string parentName = "Floors";

        [Min(0)] public int categoryWeight = 1;

        public List<TilePrefabEntry> prefabs = new List<TilePrefabEntry>();
    }

    [Header("Map Settings")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float tileSize = 12f;
    [SerializeField] private bool centerMapOnOrigin = true;

    [Header("Center Tile Categories")]
    [SerializeField] private List<TileCategory> centerCategories = new List<TileCategory>();

    [Header("Edge Tile Categories")]
    [SerializeField] private List<TileCategory> edgeCategories = new List<TileCategory>();

    [Header("Corner Tile Categories")]
    [SerializeField] private List<TileCategory> cornerCategories = new List<TileCategory>();

    [Header("Generated Parent")]
    [SerializeField] private Transform generatedParent;

    [Header("Generation")]
    [SerializeField] private bool generateOnStart = true;
    [SerializeField] private bool clearBeforeGenerate = true;
    [SerializeField] private bool generateDecorationAfterMap = true;

    private Transform terrainParent;
    private Transform decorationParent;
    private Transform propsParent;
    private Transform obstaclesParent;
    private Transform lightsParent;

    private readonly Dictionary<string, Transform> categoryParents = new Dictionary<string, Transform>();
    private bool[,] occupied;

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

        occupied = new bool[width, height];

        CreateGeneratedHierarchy();

        GenerateBorders();
        GenerateCenterTiles();

        if (generateDecorationAfterMap)
        {
            OpenWorldDecorationGenerator decorationGenerator = GetComponent<OpenWorldDecorationGenerator>();

            if (decorationGenerator != null)
                decorationGenerator.GenerateDecoration();
        }
    }

    private void GenerateBorders()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                bool isWest = x == 0;
                bool isEast = x == width - 1;
                bool isSouth = z == 0;
                bool isNorth = z == height - 1;

                if (!isWest && !isEast && !isSouth && !isNorth)
                    continue;

                SpawnBorderTile(x, z);
                occupied[x, z] = true;
            }
        }
    }

    private void GenerateCenterTiles()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int z = 1; z < height - 1; z++)
            {
                if (occupied[x, z])
                    continue;

                SpawnCenterTile(x, z);
            }
        }
    }

    private void SpawnBorderTile(int x, int z)
    {
        bool isWest = x == 0;
        bool isEast = x == width - 1;
        bool isSouth = z == 0;
        bool isNorth = z == height - 1;

        bool isCorner = (isWest || isEast) && (isSouth || isNorth);

        TileCategory category = isCorner
            ? GetWeightedRandomCategory(cornerCategories)
            : GetWeightedRandomCategory(edgeCategories);

        if (category == null)
            return;

        TilePrefabEntry entry = GetWeightedRandomEntry(category.prefabs);

        if (entry == null || entry.prefab == null)
            return;

        Quaternion rotation = Quaternion.identity;

        if (isCorner)
        {
            if (isWest && isSouth)
                rotation = Quaternion.Euler(0f, 0f, 0f);
            else if (isEast && isSouth)
                rotation = Quaternion.Euler(0f, 270f, 0f);
            else if (isEast && isNorth)
                rotation = Quaternion.Euler(0f, 180f, 0f);
            else if (isWest && isNorth)
                rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            if (isWest)
                rotation = Quaternion.Euler(0f, 0f, 0f);
            else if (isNorth)
                rotation = Quaternion.Euler(0f, 90f, 0f);
            else if (isEast)
                rotation = Quaternion.Euler(0f, 180f, 0f);
            else if (isSouth)
                rotation = Quaternion.Euler(0f, 270f, 0f);
        }

        Transform parent = GetCategoryParent(category.parentName);

        Instantiate(entry.prefab, GetWorldPosition(x, z), rotation, parent);
    }

    private void SpawnCenterTile(int x, int z)
    {
        for (int attempts = 0; attempts < 20; attempts++)
        {
            TileCategory category = GetWeightedRandomCategory();

            if (category == null)
                continue;

            TilePrefabEntry entry = GetWeightedRandomEntry(category.prefabs);

            if (entry == null || entry.prefab == null)
                continue;

            int sizeX = entry.sizeX;
            int sizeZ = entry.sizeZ;
            float yRotation = 0f;

            if (entry.allowRotate90 && Random.value > 0.5f)
            {
                (sizeX, sizeZ) = (sizeZ, sizeX);
                yRotation = 90f;
            }
            else if (entry.randomYRotation)
            {
                yRotation = Random.Range(0, 4) * 90f;
            }

            if (!CanPlaceTile(x, z, sizeX, sizeZ))
                continue;

            Vector3 position = GetWorldPositionForFootprint(x, z, sizeX, sizeZ);
            Quaternion rotation = Quaternion.Euler(0f, yRotation, 0f);

            Transform parent = GetCategoryParent(category.parentName);

            Instantiate(entry.prefab, position, rotation, parent);

            MarkOccupied(x, z, sizeX, sizeZ);
            return;
        }

        SpawnFallbackFloor(x, z);
    }

    private void SpawnFallbackFloor(int x, int z)
    {
        TileCategory floorCategory = centerCategories.Find(c => c.categoryName == "Floors");

        if (floorCategory == null)
            floorCategory = centerCategories.Count > 0 ? centerCategories[0] : null;

        if (floorCategory == null)
            return;

        TilePrefabEntry entry = GetWeightedRandomEntry(floorCategory.prefabs);

        if (entry == null || entry.prefab == null)
            return;

        Transform parent = GetCategoryParent(floorCategory.parentName);

        Instantiate(entry.prefab, GetWorldPosition(x, z), Quaternion.identity, parent);
        occupied[x, z] = true;
    }

    private bool CanPlaceTile(int startX, int startZ, int sizeX, int sizeZ)
    {
        if (startX < 1 || startZ < 1)
            return false;

        if (startX + sizeX > width - 1)
            return false;

        if (startZ + sizeZ > height - 1)
            return false;

        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int z = startZ; z < startZ + sizeZ; z++)
            {
                if (occupied[x, z])
                    return false;
            }
        }

        return true;
    }

    private void MarkOccupied(int startX, int startZ, int sizeX, int sizeZ)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int z = startZ; z < startZ + sizeZ; z++)
            {
                occupied[x, z] = true;
            }
        }
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

    private Vector3 GetWorldPositionForFootprint(int x, int z, int sizeX, int sizeZ)
    {
        Vector3 firstTile = GetWorldPosition(x, z);

        float offsetX = (sizeX - 1) * tileSize * 0.5f;
        float offsetZ = (sizeZ - 1) * tileSize * 0.5f;

        return firstTile + new Vector3(offsetX, 0f, offsetZ);
    }

    private TileCategory GetWeightedRandomCategory(List<TileCategory> categories)
    {
        if (categories == null || categories.Count == 0)
            return null;

        int totalWeight = 0;

        foreach (TileCategory category in categories)
        {
            if (category == null || category.categoryWeight <= 0)
                continue;

            if (category.prefabs == null || category.prefabs.Count == 0)
                continue;

            totalWeight += category.categoryWeight;
        }

        if (totalWeight <= 0)
            return null;

        int roll = Random.Range(0, totalWeight);
        int current = 0;

        foreach (TileCategory category in categories)
        {
            if (category == null || category.categoryWeight <= 0)
                continue;

            if (category.prefabs == null || category.prefabs.Count == 0)
                continue;

            current += category.categoryWeight;

            if (roll < current)
                return category;
        }

        return null;
    }

    private TileCategory GetWeightedRandomCategory()
    {
        return GetWeightedRandomCategory(centerCategories);
    }

    private TilePrefabEntry GetWeightedRandomEntry(List<TilePrefabEntry> entries)
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
        int current = 0;

        foreach (TilePrefabEntry entry in entries)
        {
            if (entry == null || entry.prefab == null || entry.weight <= 0)
                continue;

            current += entry.weight;

            if (roll < current)
                return entry;
        }

        return null;
    }

    private void CreateGeneratedHierarchy()
    {
        categoryParents.Clear();

        if (generatedParent == null)
        {
            GameObject parent = new GameObject("Generated_Map");
            parent.transform.SetParent(transform);
            parent.transform.localPosition = Vector3.zero;
            generatedParent = parent.transform;
        }

        terrainParent = CreateChild(generatedParent, "Terrain");

        foreach (TileCategory category in centerCategories)
        {
            if (category == null)
                continue;

            string parentName = string.IsNullOrWhiteSpace(category.parentName)
                ? category.categoryName
                : category.parentName;

            GetCategoryParent(parentName);
        }

        foreach (TileCategory category in edgeCategories)
        {
            if (category == null)
                continue;

            string parentName = string.IsNullOrWhiteSpace(category.parentName)
                ? category.categoryName
                : category.parentName;

            GetCategoryParent(parentName);
        }

        foreach (TileCategory category in cornerCategories)
        {
            if (category == null)
                continue;

            string parentName = string.IsNullOrWhiteSpace(category.parentName)
                ? category.categoryName
                : category.parentName;

            GetCategoryParent(parentName);
        }

        decorationParent = CreateChild(generatedParent, "Decoration");
        propsParent = CreateChild(decorationParent, "Props");
        obstaclesParent = CreateChild(decorationParent, "Obstacles");
        lightsParent = CreateChild(decorationParent, "Lights");
    }

    private Transform GetCategoryParent(string parentName)
    {
        if (string.IsNullOrWhiteSpace(parentName))
            parentName = "Misc";

        if (categoryParents.TryGetValue(parentName, out Transform existing))
            return existing;

        Transform parent = CreateChild(terrainParent, parentName);
        categoryParents.Add(parentName, parent);

        return parent;
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

        decorationParent = null;
        propsParent = null;
        obstaclesParent = null;
        lightsParent = null;

        categoryParents.Clear();
    }
}