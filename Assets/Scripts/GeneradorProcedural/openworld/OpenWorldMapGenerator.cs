using Chaosbound.Content.Expeditions.Runtime.World;
using Chaosbound.Content.World.Runtime.Services;
using Chaosbound.Content.World.Themes;
using Chaosbound.Content.World.Themes.TileSets;
using System;
using UnityEngine;

public class OpenWorldMapGenerator : MonoBehaviour
{
    [Header("Editor Preview")]
    [SerializeField]
    private int width = 11;

    [SerializeField]
    private int height = 11;

    [SerializeField]
    private float tileSize = 12f;

    [SerializeField]
    private bool centerMapOnOrigin = true;

    [SerializeField]
    private WorldThemeAsset previewTheme;

    [Header("Generated Parent")]
    [SerializeField]
    private Transform generatedParent;

    [Header("Generation")]
    [SerializeField]
    private bool generateOnStart = false;

    [SerializeField]
    private bool clearBeforeGenerate = true;

    [SerializeField]
    private bool generateDecorationAfterMap = false;

    private Transform terrainParent;
    private Transform decorationParent;
    private Transform propsParent;
    private Transform obstaclesParent;
    private Transform lightsParent;

    private TileSetProfile TileSet
    {
        get
        {
            WorldThemeAsset theme =
                runtimeWorldConfig != null
                    ? runtimeWorldConfig.Theme
                    : previewTheme;

            if (theme == null)
            {
                throw new InvalidOperationException(
                    "OpenWorldMapGenerator requires a WorldThemeAsset. " +
                    "Assign Preview Theme for editor generation or initialize the generator at runtime.");
            }

            if (theme.TileSet == null)
            {
                throw new InvalidOperationException(
                    $"WorldThemeAsset '{theme.name}' has no TileSet configured.");
            }

            return theme.TileSet;
        }
    }

    private readonly TileSelector tileSelector = new();

    private RuntimeWorldConfig runtimeWorldConfig;

    private bool[,] occupied;

    private Bounds generatedWorldBounds;

    public Bounds GeneratedWorldBounds =>
        generatedWorldBounds;

    public bool IsGenerated { get; private set; }

    private int GenerationWidth =>
        runtimeWorldConfig != null
            ? GetRuntimeWidth()
            : width;

    private int GenerationHeight =>
        runtimeWorldConfig != null
            ? GetRuntimeHeight()
            : height;

    public void Initialize(RuntimeWorldConfig config)
    {
        runtimeWorldConfig = config
            ?? throw new ArgumentNullException(nameof(config));
    }

    private void Start()
    {
        if (generateOnStart)
            GenerateMap();
    }

    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        int mapWidth = GenerationWidth;
        int mapHeight = GenerationHeight;

        ValidateGenerationDimensions(
            mapWidth,
            mapHeight);

        if (clearBeforeGenerate)
            ClearMap();

        occupied = new bool[mapWidth, mapHeight];

        CreateGeneratedHierarchy();

        GenerateBorders();
        GenerateCenterSpawnTile();
        GenerateCenterTiles();

        UpdateGeneratedWorldBounds();

        IsGenerated = true;

        if (generateDecorationAfterMap)
        {
            OpenWorldDecorationGenerator decorationGenerator =
                GetComponent<OpenWorldDecorationGenerator>();

            if (decorationGenerator != null)
                decorationGenerator.GenerateDecoration();
        }
    }

    private void UpdateGeneratedWorldBounds()
    {
        int mapWidth = GenerationWidth;
        int mapHeight = GenerationHeight;

        float worldWidth =
            mapWidth * tileSize;

        float worldDepth =
            mapHeight * tileSize;

        Vector3 center;

        if (centerMapOnOrigin)
        {
            center = Vector3.zero;
        }
        else
        {
            center = new Vector3(
                (mapWidth - 1) * tileSize * 0.5f,
                0f,
                (mapHeight - 1) * tileSize * 0.5f);
        }

        generatedWorldBounds =
            new Bounds(
                center,
                new Vector3(
                    worldWidth,
                    0f,
                    worldDepth));
    }

    private int GetRuntimeWidth()
    {
        float value = runtimeWorldConfig.Bounds.Size.Width;

        if (!IsWholeNumber(value))
        {
            throw new InvalidOperationException(
                $"Runtime world width '{value}' must represent a whole number of tiles.");
        }

        return Mathf.RoundToInt(value);
    }

    private int GetRuntimeHeight()
    {
        float value = runtimeWorldConfig.Bounds.Size.Depth;

        if (!IsWholeNumber(value))
        {
            throw new InvalidOperationException(
                $"Runtime world height '{value}' must represent a whole number of tiles.");
        }

        return Mathf.RoundToInt(value);
    }

    private bool IsWholeNumber(float value)
    {
        return Mathf.Approximately(
            value,
            Mathf.Round(value));
    }

    private void ValidateGenerationDimensions(
        int mapWidth,
        int mapHeight)
    {
        if (mapWidth < 3)
        {
            throw new InvalidOperationException(
                $"Map width must be at least 3. Current value: {mapWidth}.");
        }

        if (mapHeight < 3)
        {
            throw new InvalidOperationException(
                $"Map height must be at least 3. Current value: {mapHeight}.");
        }

        if (mapWidth % 2 == 0)
        {
            throw new InvalidOperationException(
                $"Map width must be odd. Current value: {mapWidth}.");
        }

        if (mapHeight % 2 == 0)
        {
            throw new InvalidOperationException(
                $"Map height must be odd. Current value: {mapHeight}.");
        }
    }

    private void GenerateBorders()
    {
        int mapWidth = GenerationWidth;
        int mapHeight = GenerationHeight;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                bool isWest = x == 0;
                bool isEast = x == mapWidth - 1;
                bool isSouth = z == 0;
                bool isNorth = z == mapHeight - 1;

                if (!isWest && !isEast && !isSouth && !isNorth)
                    continue;

                SpawnBorderTile(
                    x,
                    z,
                    mapWidth,
                    mapHeight);

                occupied[x, z] = true;
            }
        }
    }

    private void GenerateCenterSpawnTile()
    {
        int mapWidth = GenerationWidth;
        int mapHeight = GenerationHeight;

        int centerX = mapWidth / 2;
        int centerZ = mapHeight / 2;

        if (occupied[centerX, centerZ])
        {
            throw new InvalidOperationException(
                "The center spawn cell is already occupied.");
        }

        TileEntry tile = TileSet.CenterSpawnTile;

        if (tile == null)
        {
            throw new InvalidOperationException(
                $"TileSetProfile '{TileSet.name}' has no Center Spawn Tile configured.");
        }

        if (tile.Prefab == null)
        {
            throw new InvalidOperationException(
                $"TileSetProfile '{TileSet.name}' Center Spawn Tile has no prefab assigned.");
        }

        if (tile.SizeX != 1 || tile.SizeZ != 1)
        {
            throw new InvalidOperationException(
                $"Center Spawn Tile '{tile.Prefab.name}' must have a 1x1 footprint.");
        }

        if (tile.AllowRotate90 || tile.RandomYRotation)
        {
            throw new InvalidOperationException(
                $"Center Spawn Tile '{tile.Prefab.name}' must not use rotation.");
        }

        Instantiate(
            tile.Prefab,
            GetWorldPosition(centerX, centerZ),
            Quaternion.identity,
            terrainParent);

        occupied[centerX, centerZ] = true;
    }

    private void GenerateCenterTiles()
    {
        int mapWidth = GenerationWidth;
        int mapHeight = GenerationHeight;

        for (int x = 1; x < mapWidth - 1; x++)
        {
            for (int z = 1; z < mapHeight - 1; z++)
            {
                if (occupied[x, z])
                    continue;

                SpawnCenterTile(
                    x,
                    z,
                    mapWidth,
                    mapHeight);
            }
        }
    }

    private void SpawnBorderTile(
        int x,
        int z,
        int mapWidth,
        int mapHeight)
    {
        bool isWest = x == 0;
        bool isEast = x == mapWidth - 1;
        bool isSouth = z == 0;
        bool isNorth = z == mapHeight - 1;

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
            GetWorldPosition(x, z, mapWidth, mapHeight),
            rotation,
            terrainParent);
    }

    private void SpawnCenterTile(
        int x,
        int z,
        int mapWidth,
        int mapHeight)
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

            if (!CanPlaceTile(
                    x,
                    z,
                    sizeX,
                    sizeZ,
                    mapWidth,
                    mapHeight))
            {
                continue;
            }

            Vector3 position =
                GetWorldPositionForFootprint(
                    x,
                    z,
                    sizeX,
                    sizeZ,
                    mapWidth,
                    mapHeight);

            Instantiate(
                tile.Prefab,
                position,
                rotation,
                terrainParent);

            MarkOccupied(
                x,
                z,
                sizeX,
                sizeZ);

            return;
        }

        SpawnFallbackFloor(x, z, mapWidth, mapHeight);
    }

    private void SpawnFallbackFloor(
        int x,
        int z,
        int mapWidth,
        int mapHeight)
    {
        TileEntry tile = tileSelector.Select(
            TileSet,
            TileContext.Center);

        Quaternion rotation =
            ApplyTileRotationModifiers(tile);

        Instantiate(
            tile.Prefab,
            GetWorldPosition(
                x,
                z,
                mapWidth,
                mapHeight),
            rotation,
            terrainParent);
    }

    private Quaternion GetBorderRotation(
        bool isWest,
        bool isEast,
        bool isSouth,
        bool isNorth)
    {
        bool isCorner =
            (isWest || isEast) &&
            (isSouth || isNorth);

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

    private Quaternion ApplyTileRotationModifiers(
        TileEntry tile,
        ref int sizeX,
        ref int sizeZ)
    {
        if (tile == null)
            throw new ArgumentNullException(nameof(tile));

        if (tile.AllowRotate90 &&
            UnityEngine.Random.value > 0.5f)
        {
            (sizeX, sizeZ) =
                (sizeZ, sizeX);

            return Quaternion.Euler(
                0f,
                90f,
                0f);
        }

        if (tile.RandomYRotation)
        {
            float angle =
                UnityEngine.Random.Range(0, 4) * 90f;

            return Quaternion.Euler(
                0f,
                angle,
                0f);
        }

        return Quaternion.identity;
    }

    private bool CanPlaceTile(
        int startX,
        int startZ,
        int sizeX,
        int sizeZ,
        int mapWidth,
        int mapHeight)
    {
        if (startX < 1 || startZ < 1)
            return false;

        if (startX + sizeX > mapWidth - 1)
            return false;

        if (startZ + sizeZ > mapHeight - 1)
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

    private void MarkOccupied(
        int startX,
        int startZ,
        int sizeX,
        int sizeZ)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int z = startZ; z < startZ + sizeZ; z++)
            {
                occupied[x, z] = true;
            }
        }
    }

    private Vector3 GetWorldPosition(
        int x,
        int z)
    {
        return GetWorldPosition(
            x,
            z,
            GenerationWidth,
            GenerationHeight);
    }

    private Vector3 GetWorldPosition(
        int x,
        int z,
        int mapWidth,
        int mapHeight)
    {
        float posX = x * tileSize;
        float posZ = z * tileSize;

        if (centerMapOnOrigin)
        {
            posX -=
                (mapWidth - 1) *
                tileSize *
                0.5f;

            posZ -=
                (mapHeight - 1) *
                tileSize *
                0.5f;
        }

        return new Vector3(
            posX,
            0f,
            posZ);
    }

    private Vector3 GetWorldPositionForFootprint(
        int x,
        int z,
        int sizeX,
        int sizeZ,
        int mapWidth,
        int mapHeight)
    {
        Vector3 firstTile =
            GetWorldPosition(
                x,
                z,
                mapWidth,
                mapHeight);

        float offsetX =
            (sizeX - 1) *
            tileSize *
            0.5f;

        float offsetZ =
            (sizeZ - 1) *
            tileSize *
            0.5f;

        return firstTile +
               new Vector3(
                   offsetX,
                   0f,
                   offsetZ);
    }

    private void CreateGeneratedHierarchy()
    {
        if (generatedParent == null)
        {
            GameObject parent =
                new GameObject("Generated_Map");

            parent.transform.SetParent(transform);
            parent.transform.localPosition =
                Vector3.zero;

            generatedParent =
                parent.transform;
        }

        terrainParent =
            CreateChild(
                generatedParent,
                "Terrain");

        decorationParent =
            CreateChild(
                generatedParent,
                "Decoration");

        propsParent =
            CreateChild(
                decorationParent,
                "Props");

        obstaclesParent =
            CreateChild(
                decorationParent,
                "Obstacles");

        lightsParent =
            CreateChild(
                decorationParent,
                "Lights");
    }

    private Transform CreateChild(
        Transform parent,
        string childName)
    {
        Transform existing =
            parent.Find(childName);

        if (existing != null)
            return existing;

        GameObject child =
            new GameObject(childName);

        child.transform.SetParent(parent);
        child.transform.localPosition =
            Vector3.zero;

        child.transform.localRotation =
            Quaternion.identity;

        child.transform.localScale =
            Vector3.one;

        return child.transform;
    }

    [ContextMenu("Clear Map")]
    public void ClearMap()
    {
        if (generatedParent == null)
            return;

        for (int i = generatedParent.childCount - 1;
             i >= 0;
             i--)
        {
            if (Application.isPlaying)
            {
                Destroy(
                    generatedParent
                        .GetChild(i)
                        .gameObject);
            }
            else
            {
                DestroyImmediate(
                    generatedParent
                        .GetChild(i)
                        .gameObject);
            }
        }

        terrainParent = null;

        decorationParent = null;
        propsParent = null;
        obstaclesParent = null;
        lightsParent = null;

        generatedWorldBounds =
            default;

        IsGenerated = false;
    }
}