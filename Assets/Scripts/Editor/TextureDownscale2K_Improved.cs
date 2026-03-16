using UnityEngine;
using UnityEditor;

public class TextureDownscale2K_Improved
{
    [MenuItem("Tools/Optimize/Downscale >=2K Textures (Safe)")]
    static void DownscaleTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");
        int changed = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;

            // Saltar tipos sensibles
            if (importer.textureType == TextureImporterType.Sprite ||
                importer.textureType == TextureImporterType.Lightmap ||
                importer.textureType == TextureImporterType.SingleChannel)
                continue;

            // Saltar skybox/cubemaps por nombre
            string p = path.ToLower();
            if (p.Contains("sky") || importer.textureShape == TextureImporterShape.TextureCube)
                continue;

            bool changedLocal = false;

            // Bajar cualquier >= 2048
            if (importer.maxTextureSize >= 2048)
            {
                importer.maxTextureSize = 1024;
                changedLocal = true;
            }

            // Compresión y Read/Write
            if (importer.textureCompression == TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Compressed;
                changedLocal = true;
            }

            if (importer.isReadable)
            {
                importer.isReadable = false;
                changedLocal = true;
            }

            // Limpiar overrides de plataforma (evita que sigan en 2048)
            string[] platforms = { "Standalone", "Android", "iPhone" };
            foreach (var plat in platforms)
            {
                var ps = importer.GetPlatformTextureSettings(plat);
                if (ps != null && ps.overridden && ps.maxTextureSize >= 2048)
                {
                    ps.maxTextureSize = 1024;
                    importer.SetPlatformTextureSettings(ps);
                    changedLocal = true;
                }
            }

            if (changedLocal)
            {
                importer.SaveAndReimport();
                changed++;
                Debug.Log("Downscaled >=2K: " + path);
            }
        }

        Debug.Log("Textures downscaled (>=2K → 1K): " + changed);
    }
}