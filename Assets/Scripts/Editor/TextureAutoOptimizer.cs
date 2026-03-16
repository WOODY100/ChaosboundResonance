using UnityEngine;
using UnityEditor;

public class TextureAutoOptimizer
{
    [MenuItem("Tools/Optimize/Optimize All Textures")]
    static void OptimizeTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");

        int optimized = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer == null)
                continue;

            bool changed = false;

            // Reduce 4K / 2K textures
            if (importer.maxTextureSize > 1024)
            {
                importer.maxTextureSize = 1024;
                changed = true;
            }

            // Disable Read/Write
            if (importer.isReadable)
            {
                importer.isReadable = false;
                changed = true;
            }

            // Enable compression
            if (importer.textureCompression == TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Compressed;
                changed = true;
            }

            // Mipmaps for world textures
            if (importer.textureType == TextureImporterType.Default)
            {
                if (!importer.mipmapEnabled)
                {
                    importer.mipmapEnabled = true;
                    changed = true;
                }
            }

            // Fix normal maps
            if (importer.textureType == TextureImporterType.NormalMap)
            {
                importer.sRGBTexture = false;
                importer.mipmapEnabled = true;
                changed = true;
            }

            if (changed)
            {
                importer.SaveAndReimport();
                optimized++;
                Debug.Log("Optimized: " + path);
            }
        }

        Debug.Log("Textures optimized: " + optimized);
    }
}