using Chaosbound.Content.Expeditions.Runtime.World;
using Chaosbound.Content.World.Runtime.Services;
using Chaosbound.Content.World.Themes.Decorations;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OpenWorldDecorationGenerator : MonoBehaviour
{
    [Header("Generation")]
    [SerializeField] private bool clearBeforeGenerate = true;

    private Transform floorsParent;
    private Transform propsParent;
    private Transform obstaclesParent;
    private Transform lightsParent;

    private DecorationProfile Decoration => runtimeWorldConfig.Theme.Decoration;

    private readonly DecorationSelector decorationSelector = new();

    private RuntimeWorldConfig runtimeWorldConfig;

    public void Initialize(RuntimeWorldConfig config)
    {
        runtimeWorldConfig = config
            ?? throw new ArgumentNullException(nameof(config));
    }

    [ContextMenu("Generate Decoration")]
    public void GenerateDecoration()
    {
        if (runtimeWorldConfig == null)
        {
            Debug.LogError(
                $"{nameof(OpenWorldDecorationGenerator)} has not been initialized.");

            return;
        }

        if (!FindGeneratedParents())
            return;

        if (clearBeforeGenerate)
            ClearDecoration();

        TileDecorationPoints[] tiles =
            floorsParent.GetComponentsInChildren<TileDecorationPoints>();

        foreach (TileDecorationPoints tile in tiles)
        {
            DecorateTile(tile);
        }
    }

    [ContextMenu("Clear Decoration")]
    public void ClearDecoration()
    {
        if (!FindGeneratedParents())
            return;

        ClearParent(propsParent);
        ClearParent(obstaclesParent);
        ClearParent(lightsParent);
    }

    private bool FindGeneratedParents()
    {
        Transform generatedMap = transform.Find("Generated_Map");

        if (generatedMap == null)
            generatedMap = transform;

        floorsParent = generatedMap.Find("Terrain");
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

    private void DecorateTile(TileDecorationPoints tile)
    {
        if (TrySpawnLargeObstacle(tile))
            return;

        Transform[] points = tile.SpawnPoints;

        if (points == null || points.Length == 0)
            return;

        List<Transform> availablePoints = new List<Transform>(points);

        int propsToSpawn = UnityEngine.Random.Range( Decoration.MinPropsPerTile, Decoration.MaxPropsPerTile + 1);
        int obstaclesToSpawn = UnityEngine.Random.Range( Decoration.MinObstaclesPerTile, Decoration.MaxObstaclesPerTile + 1);

        SpawnFromPoints(DecorationContext.Prop, propsParent, availablePoints, propsToSpawn);
        SpawnFromPoints(DecorationContext.Obstacle, obstaclesParent, availablePoints, obstaclesToSpawn);

        if (UnityEngine.Random.value <= Decoration.LightChance)
            SpawnFromPoints( DecorationContext.Light, lightsParent, availablePoints, 1);
    }

    private bool TrySpawnLargeObstacle(TileDecorationPoints tile)
    {
        if (tile.CenterPoint == null)
            return false;

        if (UnityEngine.Random.value > Decoration.LargeObstacleChance)
            return false;

        DecorationPrefabEntry entry = decorationSelector.Select(Decoration, DecorationContext.LargeObstacle);

        if (entry == null)
            return false;

        GameObject prefab = entry.Prefab;

        if (prefab == null)
            return false;

        Vector3 spawnPosition = tile.CenterPoint.position;
        spawnPosition.y = Decoration.LargeObstacleSpawnHeight;

        Quaternion spawnRotation = entry.RandomYRotation
        ? Quaternion.Euler( 0f, UnityEngine.Random.Range(0f, 360f), 0f)
        : prefab.transform.rotation;

        GameObject instance = Instantiate(prefab, spawnPosition, spawnRotation, obstaclesParent);

        float scale = UnityEngine.Random.Range( entry.ScaleRange.x, entry.ScaleRange.y);

        instance.transform.localScale = prefab.transform.localScale * scale;

        return true;
    }

    private void SpawnFromPoints(DecorationContext context, Transform parent, List<Transform> availablePoints, int amount)
    {
        if (parent == null)
            return;

        amount = Mathf.Min(amount, availablePoints.Count);

        for (int i = 0; i < amount; i++)
        {
            Transform point = TakeRandomPoint(availablePoints);

            if (point == null)
                return;

            DecorationPrefabEntry entry = decorationSelector.Select( Decoration, context);

            if (entry == null)
                continue;

            GameObject prefab = entry.Prefab;

            if (prefab == null)
                continue;

            Vector3 offset = new Vector3(
                UnityEngine.Random.Range(-Decoration.RandomOffsetRadius, Decoration.RandomOffsetRadius),
                0f,
                UnityEngine.Random.Range(-Decoration.RandomOffsetRadius, Decoration.RandomOffsetRadius)
            );

            Vector3 spawnPosition = point.position + offset;
            spawnPosition.y = Decoration.SpawnHeight;

            Quaternion spawnRotation = entry.RandomYRotation
                ? Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f)
                : prefab.transform.rotation;

            GameObject instance = Instantiate(prefab, spawnPosition, spawnRotation, parent);

            float scale = UnityEngine.Random.Range(entry.ScaleRange.x, entry.ScaleRange.y);
            instance.transform.localScale = prefab.transform.localScale * scale;
        }
    }

    private Transform TakeRandomPoint(List<Transform> points)
    {
        if (points.Count == 0)
            return null;

        int index = UnityEngine.Random.Range(0, points.Count);
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