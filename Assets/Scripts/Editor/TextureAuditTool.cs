using UnityEngine;
using UnityEditor;

public class TextureAuditTool
{
    [MenuItem("Tools/Audit/Scan Textures")]
    static void ScanTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");

        int count4k = 0;
        int count2k = 0;
        int readWrite = 0;
        int noCompression = 0;
        int noMipmaps = 0;
        int badNormalMap = 0;

        Debug.Log("---- TEXTURE AUDIT START ----");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer == null)
                continue;

            int maxSize = importer.maxTextureSize;

            if (maxSize >= 4096)
            {
                count4k++;
                Debug.Log($"[4K TEXTURE] {path}");
            }

            if (maxSize >= 2048)
            {
                count2k++;
                Debug.Log($"[2K+ TEXTURE] {path}");
            }

            if (importer.isReadable)
            {
                readWrite++;
                Debug.Log($"[READ/WRITE ENABLED] {path}");
            }

            if (importer.textureCompression == TextureImporterCompression.Uncompressed)
            {
                noCompression++;
                Debug.Log($"[NO COMPRESSION] {path}");
            }

            if (!importer.mipmapEnabled && importer.textureType == TextureImporterType.Default)
            {
                noMipmaps++;
                Debug.Log($"[NO MIPMAPS] {path}");
            }

            if (importer.textureType == TextureImporterType.NormalMap)
            {
                if (importer.sRGBTexture)
                {
                    badNormalMap++;
                    Debug.Log($"[BAD NORMAL MAP SRGB] {path}");
                }
            }
        }

        Debug.Log("---- TEXTURE AUDIT SUMMARY ----");
        Debug.Log($"4K textures: {count4k}");
        Debug.Log($"2K+ textures: {count2k}");
        Debug.Log($"Read/Write enabled: {readWrite}");
        Debug.Log($"Uncompressed textures: {noCompression}");
        Debug.Log($"No mipmaps: {noMipmaps}");
        Debug.Log($"Bad normal maps: {badNormalMap}");

        Debug.Log("---- TEXTURE AUDIT END ----");
    }
}