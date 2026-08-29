using Chaosbound.Content.Expeditions.Runtime.World;
using Chaosbound.Content.World.Runtime.Services;
using Chaosbound.Content.World.Themes.Decorations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OpenWorldDecorationGenerator : MonoBehaviour
{
    //==========================================================
    // Constants
    //==========================================================

    private const float TileSize = 12f;

    //==========================================================
    // Generation
    //==========================================================

    [Header("Generation")]

    [SerializeField]
    private bool clearBeforeGenerate = true;

    //==========================================================
    // Generated Parents
    //==========================================================

    private Transform floorsParent;
    private Transform propsParent;
    private Transform obstaclesParent;
    private Transform lightsParent;
    private Transform modifierStructuresParent;

    //==========================================================
    // Decoration
    //==========================================================

    private DecorationProfile Decoration =>
        runtimeWorldConfig.Theme.Decoration;

    private readonly DecorationSelector decorationSelector =
        new();

    private readonly List<TileDecorationPoints>
        occupiedTiles =
        new List<TileDecorationPoints>();

    private readonly HashSet<TileDecorationPoints>
        blockedTiles =
        new HashSet<TileDecorationPoints>();

    private readonly List<Vector3>
        modifierStructurePositions =
        new List<Vector3>();

    private RuntimeWorldConfig runtimeWorldConfig;

    //==========================================================
    // Initialization
    //==========================================================

    public void Initialize(
        RuntimeWorldConfig config)
    {
        runtimeWorldConfig =
            config
            ?? throw new ArgumentNullException(
                nameof(config));
    }

    //==========================================================
    // Generated Modifier Structures
    //==========================================================

    public IReadOnlyList<Vector3>
        ModifierStructurePositions =>
        modifierStructurePositions;

    //==========================================================
    // Generation
    //==========================================================

    [ContextMenu("Generate Decoration")]
    public void GenerateDecoration()
    {
        if (runtimeWorldConfig == null)
        {
            Debug.LogError(
                $"{nameof(OpenWorldDecorationGenerator)} " +
                "has not been initialized.");

            return;
        }

        if (!FindGeneratedParents())
            return;

        if (clearBeforeGenerate)
            ClearDecoration();

        occupiedTiles.Clear();
        blockedTiles.Clear();
        modifierStructurePositions.Clear();

        TileDecorationPoints[] tileArray =
            floorsParent.GetComponentsInChildren<
                TileDecorationPoints>();

        if (tileArray == null ||
            tileArray.Length == 0)
        {
            return;
        }

        List<TileDecorationPoints> tiles =
            new List<TileDecorationPoints>(
                tileArray);

        ShuffleTiles(
            tiles);

        foreach (
            TileDecorationPoints tile
            in tiles)
        {
            if (tile == null)
                continue;

            if (IsTileBlocked(tile))
                continue;

            if (!DecorateTile(tile))
                continue;

            occupiedTiles.Add(
                tile);

            BlockRandomNeighbors(
                tile,
                tiles);
        }
    }

    //==========================================================
    // Clear
    //==========================================================

    [ContextMenu("Clear Decoration")]
    public void ClearDecoration()
    {
        if (!FindGeneratedParents())
            return;

        ClearParent(
            propsParent);

        ClearParent(
            obstaclesParent);

        ClearParent(
            lightsParent);

        ClearParent(
            modifierStructuresParent);

        occupiedTiles.Clear();
        blockedTiles.Clear();
        modifierStructurePositions.Clear();
    }

    //==========================================================
    // Generated Hierarchy
    //==========================================================

    private bool FindGeneratedParents()
    {
        Transform generatedMap =
            transform.Find(
                "Generated_Map");

        if (generatedMap == null)
        {
            generatedMap =
                transform;
        }

        floorsParent =
            generatedMap.Find(
                "Terrain");

        propsParent =
            generatedMap.Find(
                "Decoration/Props");

        obstaclesParent =
            generatedMap.Find(
                "Decoration/Obstacles");

        lightsParent =
            generatedMap.Find(
        "Decoration/Lights");

        Transform decorationParent =
            generatedMap.Find(
                "Decoration");

        if (decorationParent == null)
        {
            Debug.LogWarning(
                "OpenWorldDecorationGenerator: " +
                "Missing generated Decoration parent.");

            return false;
        }

        modifierStructuresParent =
            decorationParent.Find(
                "ModifierStructures");

        if (modifierStructuresParent == null)
        {
            GameObject modifierStructuresObject =
                new GameObject(
                    "ModifierStructures");

            modifierStructuresParent =
                modifierStructuresObject.transform;

            modifierStructuresParent.SetParent(
                decorationParent,
                false);
        }

        if (floorsParent == null ||
            propsParent == null ||
            obstaclesParent == null ||
            lightsParent == null)
        {
            Debug.LogWarning(
                "OpenWorldDecorationGenerator: " +
                "Missing generated hierarchy references.");

            return false;
        }

        return true;
    }

    //==========================================================
    // Tile Decoration
    //==========================================================

    private bool DecorateTile(
        TileDecorationPoints tile)
    {
        if (TrySpawnModifierStructure(tile))
        {
            return true;
        }

        if (TrySpawnLargeObstacle(tile))
        {
            return true;
        }

        Transform[] points =
            tile.SpawnPoints;

        if (points == null ||
            points.Length == 0)
        {
            return false;
        }

        List<Transform> availablePoints =
            new List<Transform>(
                points);

        int propsToSpawn =
            UnityEngine.Random.Range(
                Decoration.MinPropsPerTile,
                Decoration.MaxPropsPerTile + 1);

        int obstaclesToSpawn =
            UnityEngine.Random.Range(
                Decoration.MinObstaclesPerTile,
                Decoration.MaxObstaclesPerTile + 1);

        bool spawned =
            false;

        if (SpawnFromPoints(
            DecorationContext.Prop,
            propsParent,
            availablePoints,
            propsToSpawn))
        {
            spawned = true;
        }

        if (SpawnFromPoints(
            DecorationContext.Obstacle,
            obstaclesParent,
            availablePoints,
            obstaclesToSpawn))
        {
            spawned = true;
        }

        if (UnityEngine.Random.value <=
            Decoration.LightChance)
        {
            if (SpawnFromPoints(
                DecorationContext.Light,
                lightsParent,
                availablePoints,
                1))
            {
                spawned = true;
            }
        }

        return spawned;
    }

    //==========================================================
    // Modifier Structures
    //==========================================================

    private bool TrySpawnModifierStructure(
        TileDecorationPoints tile)
    {
        if (tile == null)
            return false;

        if (tile.CenterPoint == null)
            return false;

        if (Decoration.ModifierStructures == null ||
            Decoration.ModifierStructures.Count == 0)
        {
            return false;
        }

        if (Decoration.MaxModifierStructuresPerTile <= 0)
            return false;

        if (UnityEngine.Random.value >
            Decoration.ModifierStructureChance)
        {
            return false;
        }

        int amount =
            Mathf.Min(
                Decoration.MaxModifierStructuresPerTile,
                1);

        bool spawned =
            false;

        for (int i = 0;
             i < amount;
             i++)
        {
            DecorationPrefabEntry entry =
                TakeRandomModifierStructure();

            if (entry == null)
                continue;

            GameObject prefab =
                entry.Prefab;

            if (prefab == null)
                continue;

            Vector3 spawnPosition =
                tile.CenterPoint.position;

            spawnPosition.y =
                Decoration.SpawnHeight;

            Quaternion spawnRotation =
                entry.RandomYRotation
                    ? Quaternion.Euler(
                        0f,
                        UnityEngine.Random.Range(
                            0f,
                            360f),
                        0f)
                    : prefab.transform.rotation;

            GameObject instance =
                Instantiate(
                    prefab,
                    spawnPosition,
                    spawnRotation,
                    modifierStructuresParent);

            float scale =
                UnityEngine.Random.Range(
                    entry.ScaleRange.x,
                    entry.ScaleRange.y);

            instance.transform.localScale =
                prefab.transform.localScale *
                scale;

            modifierStructurePositions.Add(
                instance.transform.position);

            spawned = true;
        }

        return spawned;
    }

    private DecorationPrefabEntry
        TakeRandomModifierStructure()
    {
        if (Decoration.ModifierStructures == null ||
            Decoration.ModifierStructures.Count == 0)
        {
            return null;
        }

        int index =
            UnityEngine.Random.Range(
                0,
                Decoration.ModifierStructures.Count);

        return Decoration.ModifierStructures[index];
    }

    //==========================================================
    // Large Obstacles
    //==========================================================

    private bool TrySpawnLargeObstacle(
        TileDecorationPoints tile)
    {
        if (tile.CenterPoint == null)
            return false;

        if (UnityEngine.Random.value >
            Decoration.LargeObstacleChance)
        {
            return false;
        }

        DecorationPrefabEntry entry =
            decorationSelector.Select(
                Decoration,
                DecorationContext.LargeObstacle);

        if (entry == null)
            return false;

        GameObject prefab =
            entry.Prefab;

        if (prefab == null)
            return false;

        Vector3 spawnPosition =
            tile.CenterPoint.position;

        spawnPosition.y =
            Decoration.LargeObstacleSpawnHeight;

        Quaternion spawnRotation =
            entry.RandomYRotation
                ? Quaternion.Euler(
                    0f,
                    UnityEngine.Random.Range(
                        0f,
                        360f),
                    0f)
                : prefab.transform.rotation;

        GameObject instance =
            Instantiate(
                prefab,
                spawnPosition,
                spawnRotation,
                obstaclesParent);

        float scale =
            UnityEngine.Random.Range(
                entry.ScaleRange.x,
                entry.ScaleRange.y);

        instance.transform.localScale =
            prefab.transform.localScale *
            scale;

        return true;
    }

    //==========================================================
    // Point-Based Decoration
    //==========================================================

    private bool SpawnFromPoints(
        DecorationContext context,
        Transform parent,
        List<Transform> availablePoints,
        int amount)
    {
        if (parent == null)
            return false;

        if (availablePoints == null ||
            availablePoints.Count == 0)
        {
            return false;
        }

        amount =
            Mathf.Min(
                amount,
                availablePoints.Count);

        bool spawned =
            false;

        for (int i = 0;
             i < amount;
             i++)
        {
            Transform point =
                TakeRandomPoint(
                    availablePoints);

            if (point == null)
                break;

            DecorationPrefabEntry entry =
                decorationSelector.Select(
                    Decoration,
                    context);

            if (entry == null)
                continue;

            GameObject prefab =
                entry.Prefab;

            if (prefab == null)
                continue;

            Vector3 offset =
                new Vector3(
                    UnityEngine.Random.Range(
                        -Decoration.RandomOffsetRadius,
                        Decoration.RandomOffsetRadius),
                    0f,
                    UnityEngine.Random.Range(
                        -Decoration.RandomOffsetRadius,
                        Decoration.RandomOffsetRadius));

            Vector3 spawnPosition =
                point.position +
                offset;

            spawnPosition.y =
                Decoration.SpawnHeight;

            Quaternion spawnRotation =
                entry.RandomYRotation
                    ? Quaternion.Euler(
                        0f,
                        UnityEngine.Random.Range(
                            0f,
                            360f),
                        0f)
                    : prefab.transform.rotation;

            GameObject instance =
                Instantiate(
                    prefab,
                    spawnPosition,
                    spawnRotation,
                    parent);

            float scale =
                UnityEngine.Random.Range(
                    entry.ScaleRange.x,
                    entry.ScaleRange.y);

            instance.transform.localScale =
                prefab.transform.localScale *
                scale;

            spawned = true;
        }

        return spawned;
    }

    //==========================================================
    // Decoration Occupancy
    //==========================================================

    private bool IsTileBlocked(
        TileDecorationPoints tile)
    {
        if (tile == null)
            return true;

        return blockedTiles.Contains(
            tile);
    }

    private void BlockRandomNeighbors(
        TileDecorationPoints occupiedTile,
        List<TileDecorationPoints> allTiles)
    {
        if (occupiedTile == null)
            return;

        int maxNeighbors =
            Decoration.MaxBlockedNeighbors;

        if (maxNeighbors <= 0)
            return;

        if (occupiedTile.CenterPoint == null)
            return;

        List<TileDecorationPoints> neighbors =
            FindNeighbors(
                occupiedTile,
                allTiles);

        if (neighbors.Count == 0)
            return;

        ShuffleTiles(
            neighbors);

        int amountToBlock =
            Mathf.Min(
                maxNeighbors,
                neighbors.Count);

        for (int i = 0;
             i < amountToBlock;
             i++)
        {
            TileDecorationPoints neighbor =
                neighbors[i];

            if (neighbor == null)
                continue;

            blockedTiles.Add(
                neighbor);
        }
    }

    private List<TileDecorationPoints> FindNeighbors(
        TileDecorationPoints source,
        List<TileDecorationPoints> allTiles)
    {
        List<TileDecorationPoints> neighbors =
            new List<TileDecorationPoints>();

        if (source == null ||
            source.CenterPoint == null ||
            allTiles == null)
        {
            return neighbors;
        }

        Vector3 sourcePosition =
            source.CenterPoint.position;

        foreach (
            TileDecorationPoints candidate
            in allTiles)
        {
            if (candidate == null ||
                candidate == source)
            {
                continue;
            }

            if (candidate.CenterPoint == null)
                continue;

            Vector3 candidatePosition =
                candidate.CenterPoint.position;

            float deltaX =
                Mathf.Abs(
                    candidatePosition.x -
                    sourcePosition.x);

            float deltaZ =
                Mathf.Abs(
                    candidatePosition.z -
                    sourcePosition.z);

            bool isNeighbor =
                deltaX <= TileSize &&
                deltaZ <= TileSize;

            if (isNeighbor)
            {
                neighbors.Add(
                    candidate);
            }
        }

        return neighbors;
    }

    //==========================================================
    // Tile Ordering
    //==========================================================

    private void ShuffleTiles(
        List<TileDecorationPoints> tiles)
    {
        for (int i = tiles.Count - 1;
             i > 0;
             i--)
        {
            int randomIndex =
                UnityEngine.Random.Range(
                    0,
                    i + 1);

            TileDecorationPoints temporary =
                tiles[i];

            tiles[i] =
                tiles[randomIndex];

            tiles[randomIndex] =
                temporary;
        }
    }

    //==========================================================
    // Point Selection
    //==========================================================

    private Transform TakeRandomPoint(
        List<Transform> points)
    {
        if (points == null ||
            points.Count == 0)
        {
            return null;
        }

        int index =
            UnityEngine.Random.Range(
                0,
                points.Count);

        Transform selected =
            points[index];

        points.RemoveAt(
            index);

        return selected;
    }

    //==========================================================
    // Destruction
    //==========================================================

    private void ClearParent(
        Transform parent)
    {
        if (parent == null)
            return;

        for (int i = parent.childCount - 1;
             i >= 0;
             i--)
        {
            if (Application.isPlaying)
            {
                Destroy(
                    parent.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(
                    parent.GetChild(i).gameObject);
            }
        }
    }
}