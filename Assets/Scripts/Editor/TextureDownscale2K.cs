using UnityEngine;
using UnityEditor;

public class TextureDownscale2K
{
    [MenuItem("Tools/Optimize/Downscale 2K Textures")]
    static void DownscaleTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");

        int changed = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer == null)
                continue;

            // Ignorar tipos sensibles
            if (importer.textureType == TextureImporterType.Sprite ||
                importer.textureType == TextureImporterType.Lightmap ||
                importer.textureType == TextureImporterType.SingleChannel)
                continue;

            // Ignorar skybox
            if (path.ToLower().Contains("sky"))
                continue;

            if (importer.maxTextureSize == 2048)
            {
                importer.maxTextureSize = 1024;

                importer.textureCompression = TextureImporterCompression.Compressed;
                importer.isReadable = false;

                importer.SaveAndReimport();

                Debug.Log("Downscaled: " + path);
                changed++;
            }
        }

        Debug.Log("Textures downscaled from 2K → 1K: " + changed);
    }
}
