
using UnityEditor;
using UnityEngine;

public static class TerrainDataDiagnostic
{
    [MenuItem("Tools/Chaosbound/Debug/Inspect Selected Terrain Data")]
    private static void InspectSelectedTerrainData()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            Debug.LogWarning("Selecciona un objeto Terrain.");
            return;
        }

        Terrain terrain = selected.GetComponent<Terrain>();
        TerrainCollider terrainCollider =
            selected.GetComponent<TerrainCollider>();

        if (terrain == null)
        {
            Debug.LogWarning("El objeto no contiene un Terrain.");
            return;
        }

        TerrainData renderData = terrain.terrainData;
        TerrainData colliderData =
            terrainCollider != null
                ? terrainCollider.terrainData
                : null;

        string renderPath = renderData != null
            ? AssetDatabase.GetAssetPath(renderData)
            : "NULL";

        string colliderPath = colliderData != null
            ? AssetDatabase.GetAssetPath(colliderData)
            : "NULL";

        float renderMaxHeight = 0f;

        if (renderData != null)
        {
            int resolution = renderData.heightmapResolution;

            float[,] heights = renderData.GetHeights(
                0, 0, resolution, resolution);

            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    if (heights[z, x] > renderMaxHeight)
                        renderMaxHeight = heights[z, x];
                }
            }

            renderMaxHeight *= renderData.size.y;
        }

        Debug.Log(
            $"OBJECT: {selected.name}\n" +
            $"Render Data: {renderData?.name ?? "NULL"}\n" +
            $"Render Path: {renderPath}\n" +
            $"Collider Data: {colliderData?.name ?? "NULL"}\n" +
            $"Collider Path: {colliderPath}\n" +
            $"Max Height: {renderMaxHeight}\n" +
            $"Same Reference: {renderData == colliderData}",
            selected
            );
    }
}
