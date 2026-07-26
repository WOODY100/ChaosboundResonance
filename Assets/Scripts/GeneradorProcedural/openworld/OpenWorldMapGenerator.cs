using Chaosbound.Content.Expeditions.Runtime.World;
using Chaosbound.Content.World.Runtime.Services;
using Chaosbound.Content.World.Themes.TileSets;
using System;
using UnityEngine;

public class OpenWorldMapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float tileSize = 12f;
    [SerializeField] private bool centerMapOnOrigin = true;

    [Header("Generated Parent")]
    [SerializeField] private Transform generatedParent;

    [Header("Generation")]
    [SerializeField] private bool generateOnStart = false;
    [SerializeField] private bool clearBeforeGenerate = true;
    [SerializeField] private bool generateDecorationAfterMap = false;

    private Transform terrainParent;
    private Transform decorationParent;
    private Transform propsParent;
    private Transform obstaclesParent;
    private Transform lightsParent;

    private TileSetProfile TileSet => runtimeWorldConfig.Theme.TileSet;

    private readonly TileSelector tileSelector = new();

    private RuntimeWorldConfig runtimeWorldConfig;

    private bool[,] occupied;

    public void Initialize(RuntimeWorldConfig config)
    {
        runtimeWorldConfig = config
            ?? throw new System.ArgumentNullException(nameof(config));
    }

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

        TileContext context =
            (isWest || isEast) && (isSouth || isNorth)
                ? TileContext.Corner
                : TileContext.Edge;

        TileEntry tile = tileSelector.Select(
            TileSet,
            context);

        Quaternion rotation = GetBorderRotation(
            isWest,
            isEast,
            isSouth,
            isNorth);

        Instantiate(
            tile.Prefab,
            GetWorldPosition(x, z),
            rotation,
            terrainParent);
    }

    private void SpawnCenterTile(int x, int z)
    {
        for (int attempts = 0; attempts < 20; attempts++)
        {
            TileEntry tile = tileSelector.Select(
                TileSet,
                TileContext.Center);

            int sizeX = tile.SizeX;
            int sizeZ = tile.SizeZ;

            Quaternion rotation = ApplyTileRotationModifiers(
                tile,
                ref sizeX,
                ref sizeZ);

            if (!CanPlaceTile(x, z, sizeX, sizeZ))
                continue;

            Vector3 position = GetWorldPositionForFootprint(x, z, sizeX, sizeZ);

            Instantiate(tile.Prefab, position, rotation, terrainParent);

            MarkOccupied(x, z, sizeX, sizeZ);
            return;
        }

        SpawnFallbackFloor(x, z);
    }

    private void SpawnFallbackFloor(int x, int z)
    {
        TileEntry tile = tileSelector.Select(
            TileSet,
            TileContext.Center);

        Quaternion rotation = ApplyTileRotationModifiers(tile);

        Instantiate(
            tile.Prefab,
            GetWorldPosition(x, z),
            rotation,
            terrainParent);
    }

    private Quaternion GetBorderRotation(
    bool isWest,
    bool isEast,
    bool isSouth,
    bool isNorth)
    {
        bool isCorner = (isWest || isEast) && (isSouth || isNorth);

        if (isCorner)
        {
            if (isWest && isSouth)
                return Quaternion.Euler(0f, 0f, 0f);

            if (isEast && isSouth)
                return Quaternion.Euler(0f, 270f, 0f);

            if (isEast && isNorth)
                return Quaternion.Euler(0f, 180f, 0f);

            return Quaternion.Euler(0f, 90f, 0f);
        }

        if (isWest)
            return Quaternion.Euler(0f, 0f, 0f);

        if (isNorth)
            return Quaternion.Euler(0f, 90f, 0f);

        if (isEast)
            return Quaternion.Euler(0f, 180f, 0f);

        return Quaternion.Euler(0f, 270f, 0f);
    }

    private Quaternion ApplyTileRotationModifiers(
    TileEntry tile)
    {
        int sizeX = tile.SizeX;
        int sizeZ = tile.SizeZ;

        return ApplyTileRotationModifiers(
            tile,
            ref sizeX,
            ref sizeZ);
    }

    private Quaternion ApplyTileRotationModifiers( TileEntry tile, ref int sizeX, ref int sizeZ)
    {
        if (tile == null)
            throw new ArgumentNullException(nameof(tile));

        if (tile.AllowRotate90 && UnityEngine.Random.value > 0.5f)
        {
            (sizeX, sizeZ) = (sizeZ, sizeX);

            return Quaternion.Euler(0f, 90f, 0f);
        }

        if (tile.RandomYRotation)
        {
            float angle = UnityEngine.Random.Range(0, 4) * 90f;

            return Quaternion.Euler(0f, angle, 0f);
        }

        return Quaternion.identity;
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

    private void CreateGeneratedHierarchy()
    {
        if (generatedParent == null)
        {
            GameObject parent = new GameObject("Generated_Map");
            parent.transform.SetParent(transform);
            parent.transform.localPosition = Vector3.zero;
            generatedParent = parent.transform;
        }

        terrainParent = CreateChild(
            generatedParent,
            "Terrain");

        decorationParent = CreateChild(
            generatedParent,
            "Decoration");

        propsParent = CreateChild(
            decorationParent,
            "Props");

        obstaclesParent = CreateChild(
            decorationParent,
            "Obstacles");

        lightsParent = CreateChild(
            decorationParent,
            "Lights");
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
    }
}