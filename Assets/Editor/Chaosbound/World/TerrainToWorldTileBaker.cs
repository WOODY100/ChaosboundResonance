using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class TerrainToWorldTileBaker
{
    private const int MeshResolution = 129;
    private const int TextureResolution = 1024;
    private const int LayersPerPass = 4;

    private const string MenuPath =
        "Tools/Chaosbound/World/Terrain/World Tile Baker";

    private const string AlbedoBakeShaderName =
        "Hidden/Chaosbound/TerrainLayerAlbedoBake";

    private const string NormalBakeShaderName =
        "Hidden/Chaosbound/TerrainLayerNormalBake";

    [MenuItem(MenuPath)]
    private static void BakeSelectedTerrain()
    {
        GameObject selected = Selection.activeGameObject;
        Terrain terrain = selected != null
            ? selected.GetComponent<Terrain>()
            : null;

        if (terrain == null)
        {
            EditorUtility.DisplayDialog(
                "Chaosbound Terrain Baker",
                "Selecciona el GameObject que contiene el componente Terrain.",
                "OK");
            return;
        }

        if (terrain.terrainData == null)
        {
            EditorUtility.DisplayDialog(
                "Chaosbound Terrain Baker",
                "El Terrain seleccionado no tiene TerrainData.",
                "OK");
            return;
        }

        try
        {
            BakeTerrain(terrain);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            EditorUtility.DisplayDialog(
                "Chaosbound Terrain Baker",
                "El bake falló.\n\n" + ex.Message +
                "\n\nRevisa la Console para más detalles.",
                "OK");
        }
    }

    private static void BakeTerrain(Terrain terrain)
    {
        TerrainData data = terrain.terrainData;
        GameObject root = terrain.transform.root.gameObject;
        string sourceName = root.name;

        // Centralized runtime bake output folder.
        // Authoring Terrain prefabs remain in their original folders.
        const string bakedFolder =
            "Assets/Prefabs/Map/Tiles/Bake";

        EnsureFolderExists(bakedFolder);

        string prefabPath = AssetDatabase.GenerateUniqueAssetPath(
            bakedFolder + "/" + sourceName + "_Baked.prefab");
        string meshPath = AssetDatabase.GenerateUniqueAssetPath(
            bakedFolder + "/" + sourceName + "_Baked_Mesh.asset");
        string texturePath = AssetDatabase.GenerateUniqueAssetPath(
            bakedFolder + "/" + sourceName + "_Baked_Ground.png");
        string normalPath = bakedFolder + "/" +
            sourceName + "_Baked_Normal.png";
        string materialPath = AssetDatabase.GenerateUniqueAssetPath(
            bakedFolder + "/" + sourceName + "_Baked_Material.mat");

        Mesh mesh = BuildMesh(data, MeshResolution);
        mesh.name = sourceName + "_Baked_Mesh";
        AssetDatabase.CreateAsset(mesh, meshPath);

        Texture2D baked = BakeAlbedo(data);

        File.WriteAllBytes(texturePath, baked.EncodeToPNG());
        Object.DestroyImmediate(baked);

        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);

        ConfigureAlbedoTexture(texturePath);

        Texture2D imported =
            AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

        bool hasNormalMaps = HasAnyNormalMap(data);

        Texture2D bakedNormal = null;
        Texture2D importedNormal = null;

        if (hasNormalMaps)
        {
            bakedNormal = BakeNormal(data);

            File.WriteAllBytes(
                normalPath,
                bakedNormal.EncodeToPNG());

            Object.DestroyImmediate(bakedNormal);

            AssetDatabase.ImportAsset(
                normalPath,
                ImportAssetOptions.ForceUpdate);

            ConfigureNormalTexture(normalPath);

            importedNormal =
                AssetDatabase.LoadAssetAtPath<Texture2D>(
                    normalPath);
        }
        else if (File.Exists(normalPath))
        {
            AssetDatabase.DeleteAsset(normalPath);
        }

        Shader lit = Shader.Find("Universal Render Pipeline/Lit");
        if (lit == null)
            throw new System.Exception(
                "No se encontró Universal Render Pipeline/Lit.");

        Material finalMaterial = new Material(lit);
        finalMaterial.name = sourceName + "_Baked_Material";
        finalMaterial.SetTexture("_BaseMap", imported);
        finalMaterial.SetFloat("_Metallic", 0f);
        finalMaterial.SetFloat("_Smoothness", 0.25f);

        if (hasNormalMaps && importedNormal != null)
        {
            finalMaterial.SetTexture("_BumpMap", importedNormal);
            finalMaterial.SetFloat("_BumpScale", 1f);
            finalMaterial.EnableKeyword("_NORMALMAP");
        }

        AssetDatabase.CreateAsset(finalMaterial, materialPath);

        GameObject bakedRoot = new GameObject(sourceName + "_Baked");
        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(bakedRoot.transform, false);

        GameObject ground = new GameObject("Ground");
        ground.transform.SetParent(visual.transform, false);

        MeshFilter mf = ground.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        MeshRenderer mr = ground.AddComponent<MeshRenderer>();
        mr.sharedMaterial = finalMaterial;

        visual.transform.localPosition = terrain.transform.localPosition;
        visual.transform.localRotation = terrain.transform.localRotation;
        visual.transform.localScale = terrain.transform.localScale;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(
            bakedRoot, prefabPath);

        Object.DestroyImmediate(bakedRoot);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = prefab;

        EditorUtility.DisplayDialog(
            "Chaosbound Terrain Baker",
            "Bake 1024 completado.\n\n" +
            "Prefab:\n" + prefabPath +
            "\n\nMesh:\n" + meshPath +
            "\n\nTexture:\n" + texturePath +
            "\n\nNormal Map:\n" + (hasNormalMaps ? normalPath : "No se generó: ninguna Terrain Layer usa Normal Map.") + "\n\nMaterial:\n" + materialPath,
            "OK");
    }

    private static Texture2D BakeAlbedo(TerrainData data)
    {
        TerrainLayer[] layers = data.terrainLayers;

        if (layers == null || layers.Length == 0)
            throw new System.Exception(
                "El Terrain no tiene Terrain Layers.");

        Shader shader = Shader.Find(AlbedoBakeShaderName);
        if (shader == null)
            throw new System.Exception(
                "No se encontró el shader '" + AlbedoBakeShaderName +
                "'. Coloca TerrainLayerAlbedoBake.shader en Assets y espera a que Unity compile.");

        int alphaResolution = data.alphamapResolution;
        float[,,] alpha = data.GetAlphamaps(
            0, 0, alphaResolution, alphaResolution);

        RenderTexture target = new RenderTexture(
            TextureResolution,
            TextureResolution,
            0,
            RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.sRGB);

        target.name = "Chaosbound_TerrainAlbedo_2048";
        target.useMipMap = false;
        target.autoGenerateMips = false;
        target.filterMode = FilterMode.Bilinear;
        target.wrapMode = TextureWrapMode.Clamp;
        target.Create();

        RenderTexture previous = RenderTexture.active;

        Graphics.SetRenderTarget(target);
        GL.Clear(true, true, Color.clear);
        Graphics.SetRenderTarget(previous);

        Material mat = new Material(shader);
        mat.hideFlags = HideFlags.HideAndDontSave;

        try
        {
            for (int group = 0;
                 group < layers.Length;
                 group += LayersPerPass)
            {
                for (int slot = 0; slot < LayersPerPass; slot++)
                {
                    int index = group + slot;

                    Texture2D diffuse = null;
                    Vector4 tile = Vector4.one;

                    if (index < layers.Length &&
                        layers[index] != null)
                    {
                        diffuse =
                            layers[index].diffuseTexture;

                        Vector2 size =
                            layers[index].tileSize;

                        Vector2 offset =
                            layers[index].tileOffset;

                        tile = new Vector4(
                            size.x,
                            size.y,
                            offset.x,
                            offset.y);
                    }

                    mat.SetTexture("_Layer" + slot, diffuse);
                    mat.SetVector("_Tile" + slot, tile);
                }

                Texture2D groupAlpha = BuildAlphaTexture(
                    alpha,
                    alphaResolution,
                    group,
                    layers.Length);

                mat.SetTexture("_Alphamap", groupAlpha);
                mat.SetVector("_TerrainSize",
                    new Vector4(
                        data.size.x,
                        data.size.z,
                        0f,
                        0f));

                mat.SetInt("_LayerCount",
                    Mathf.Min(
                        LayersPerPass,
                        layers.Length - group));

                Graphics.Blit(
                    Texture2D.blackTexture,
                    target,
                    mat,
                    0);

                Object.DestroyImmediate(groupAlpha);
            }
        }
        finally
        {
            Object.DestroyImmediate(mat);
        }

        RenderTexture.active = target;

        Texture2D result = new Texture2D(
            TextureResolution,
            TextureResolution,
            TextureFormat.RGBA32,
            true,
            false);

        result.ReadPixels(
            new Rect(
                0, 0,
                TextureResolution,
                TextureResolution),
            0, 0);

        result.Apply(true, false);

        RenderTexture.active = previous;

        target.Release();
        Object.DestroyImmediate(target);

        return result;
    }


    private static bool HasAnyNormalMap(TerrainData data)
    {
        TerrainLayer[] layers = data.terrainLayers;

        if (layers == null)
            return false;

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i] != null &&
                layers[i].normalMapTexture != null)
            {
                return true;
            }
        }

        return false;
    }

    private static Texture2D BakeNormal(TerrainData data)
    {
        TerrainLayer[] layers = data.terrainLayers;

        if (layers == null || layers.Length == 0)
            throw new System.Exception(
                "El Terrain no tiene Terrain Layers.");

        Shader shader =
            Shader.Find(NormalBakeShaderName);

        if (shader == null)
            throw new System.Exception(
                "No se encontró el shader '" +
                NormalBakeShaderName +
                "'. Coloca TerrainLayerNormalBake.shader " +
                "en Assets y espera a que Unity compile.");

        int alphaResolution =
            data.alphamapResolution;

        float[,,] alpha =
            data.GetAlphamaps(
                0,
                0,
                alphaResolution,
                alphaResolution);

        Vector3[] accumulated =
            new Vector3[
                TextureResolution *
                TextureResolution];

        Material mat =
            new Material(shader);

        mat.hideFlags =
            HideFlags.HideAndDontSave;

        try
        {
            for (int group = 0;
                 group < layers.Length;
                 group += LayersPerPass)
            {
                RenderTexture groupTarget =
                    new RenderTexture(
                        TextureResolution,
                        TextureResolution,
                        0,
                        RenderTextureFormat.ARGBFloat,
                        RenderTextureReadWrite.Linear);

                groupTarget.name =
                    "Chaosbound_TerrainNormal_Group";

                groupTarget.useMipMap = false;
                groupTarget.autoGenerateMips = false;
                groupTarget.filterMode =
                    FilterMode.Bilinear;
                groupTarget.wrapMode =
                    TextureWrapMode.Clamp;
                groupTarget.Create();

                RenderTexture previous =
                    RenderTexture.active;

                Graphics.SetRenderTarget(
                    groupTarget);

                GL.Clear(
                    true,
                    true,
                    Color.clear);

                Graphics.SetRenderTarget(
                    previous);

                for (int slot = 0;
                     slot < LayersPerPass;
                     slot++)
                {
                    int index =
                        group + slot;

                    Texture2D normal = null;
                    Vector4 tile =
                        Vector4.one;
                    float normalScale = 1f;

                    if (index < layers.Length &&
                        layers[index] != null)
                    {
                        normal =
                            layers[index].normalMapTexture;

                        Vector2 size =
                            layers[index].tileSize;

                        Vector2 offset =
                            layers[index].tileOffset;

                        tile =
                            new Vector4(
                                size.x,
                                size.y,
                                offset.x,
                                offset.y);

                        normalScale =
                            layers[index].normalScale;
                    }

                    mat.SetTexture(
                        "_Layer" + slot,
                        normal);

                    mat.SetVector(
                        "_Tile" + slot,
                        tile);

                    mat.SetFloat(
                        "_NormalScale" + slot,
                        normalScale);

                    mat.SetFloat(
                        "_HasNormal" + slot,
                        normal != null ? 1f : 0f);
                }

                Texture2D groupAlpha =
                    BuildAlphaTexture(
                        alpha,
                        alphaResolution,
                        group,
                        layers.Length);

                mat.SetTexture(
                    "_Alphamap",
                    groupAlpha);

                mat.SetVector(
                    "_TerrainSize",
                    new Vector4(
                        data.size.x,
                        data.size.z,
                        0f,
                        0f));

                mat.SetInt(
                    "_LayerCount",
                    Mathf.Min(
                        LayersPerPass,
                        layers.Length - group));

                Graphics.Blit(
                    Texture2D.blackTexture,
                    groupTarget,
                    mat,
                    0);

                RenderTexture.active =
                    groupTarget;

                Texture2D groupReadback =
                    new Texture2D(
                        TextureResolution,
                        TextureResolution,
                        TextureFormat.RGBAFloat,
                        false,
                        true);

                groupReadback.ReadPixels(
                    new Rect(
                        0,
                        0,
                        TextureResolution,
                        TextureResolution),
                    0,
                    0);

                groupReadback.Apply(
                    false,
                    false);

                Color[] pixels =
                    groupReadback.GetPixels();

                for (int i = 0;
                     i < pixels.Length;
                     i++)
                {
                    accumulated[i] +=
                        new Vector3(
                            pixels[i].r,
                            pixels[i].g,
                            pixels[i].b);
                }

                RenderTexture.active =
                    previous;

                Object.DestroyImmediate(
                    groupReadback);

                groupTarget.Release();
                Object.DestroyImmediate(
                    groupTarget);

                Object.DestroyImmediate(
                    groupAlpha);
            }
        }
        finally
        {
            Object.DestroyImmediate(mat);
        }

        Texture2D result =
            new Texture2D(
                TextureResolution,
                TextureResolution,
                TextureFormat.RGBA32,
                true,
                true);

        Color[] resultPixels =
            new Color[
                accumulated.Length];

        for (int i = 0;
             i < accumulated.Length;
             i++)
        {
            Vector3 normal =
                accumulated[i];

            if (normal.sqrMagnitude < 0.000001f)
                normal = Vector3.forward;
            else
                normal.Normalize();

            resultPixels[i] =
                new Color(
                    normal.x * 0.5f + 0.5f,
                    normal.y * 0.5f + 0.5f,
                    normal.z * 0.5f + 0.5f,
                    1f);
        }

        result.SetPixels(resultPixels);
        result.Apply(true, false);

        return result;
    }

    private static void ConfigureAlbedoTexture(
        string texturePath)
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(texturePath)
            as TextureImporter;

        if (importer == null)
            return;

        importer.textureType =
            TextureImporterType.Default;

        importer.sRGBTexture = true;
        importer.mipmapEnabled = true;
        importer.wrapMode =
            TextureWrapMode.Clamp;
        importer.filterMode =
            FilterMode.Bilinear;

        importer.SaveAndReimport();
    }

    private static void ConfigureNormalTexture(
        string texturePath)
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(texturePath)
            as TextureImporter;

        if (importer == null)
            return;

        importer.textureType =
            TextureImporterType.NormalMap;

        importer.sRGBTexture = false;
        importer.mipmapEnabled = true;
        importer.wrapMode =
            TextureWrapMode.Clamp;
        importer.filterMode =
            FilterMode.Bilinear;

        importer.SaveAndReimport();
    }

    private static Texture2D BuildAlphaTexture(
        float[,,] alpha,
        int resolution,
        int layerBase,
        int layerCount)
    {
        Texture2D tex = new Texture2D(
            resolution,
            resolution,
            TextureFormat.RGBA32,
            false,
            true);

        Color[] pixels =
            new Color[resolution * resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float r = layerBase + 0 < layerCount
                    ? alpha[y, x, layerBase + 0]
                    : 0f;

                float g = layerBase + 1 < layerCount
                    ? alpha[y, x, layerBase + 1]
                    : 0f;

                float b = layerBase + 2 < layerCount
                    ? alpha[y, x, layerBase + 2]
                    : 0f;

                float a = layerBase + 3 < layerCount
                    ? alpha[y, x, layerBase + 3]
                    : 0f;

                pixels[y * resolution + x] =
                    new Color(r, g, b, a);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply(false, true);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        return tex;
    }

    private static Mesh BuildMesh(
        TerrainData data,
        int resolution)
    {
        int sourceResolution = data.heightmapResolution;
        int sampleResolution =
            Mathf.Clamp(resolution, 2, sourceResolution);

        Vector3 size = data.size;

        Mesh mesh = new Mesh();

        if (sampleResolution * sampleResolution > 65535)
            mesh.indexFormat = IndexFormat.UInt32;

        Vector3[] vertices =
            new Vector3[sampleResolution * sampleResolution];

        Vector2[] uvs =
            new Vector2[vertices.Length];

        int[] triangles =
            new int[(sampleResolution - 1) *
                     (sampleResolution - 1) * 6];

        for (int z = 0; z < sampleResolution; z++)
        {
            float v = z / (float)(sampleResolution - 1);

            for (int x = 0; x < sampleResolution; x++)
            {
                float u = x / (float)(sampleResolution - 1);
                int i = z * sampleResolution + x;

                vertices[i] = new Vector3(
                    u * size.x,
                    data.GetInterpolatedHeight(u, v),
                    v * size.z);

                uvs[i] = new Vector2(u, v);
            }
        }

        int t = 0;

        for (int z = 0; z < sampleResolution - 1; z++)
        {
            for (int x = 0; x < sampleResolution - 1; x++)
            {
                int i = z * sampleResolution + x;

                int a = i;
                int b = i + 1;
                int c = i + sampleResolution;
                int d = c + 1;

                triangles[t++] = a;
                triangles[t++] = c;
                triangles[t++] = b;

                triangles[t++] = b;
                triangles[t++] = c;
                triangles[t++] = d;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        mesh.name = "TerrainBakedMesh";
        return mesh;
    }

    private static void EnsureFolderExists(string path)
    {
        path = path.Replace("\\", "/");
        string[] parts = path.Split('/');

        if (parts.Length == 0)
            return;

        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];

            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);

            current = next;
        }
    }

    [MenuItem(MenuPath, true)]
    private static bool ValidateBakeSelectedTerrain()
    {
        return Selection.activeGameObject != null &&
               Selection.activeGameObject.GetComponent<Terrain>() != null;
    }
}
