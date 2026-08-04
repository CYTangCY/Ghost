#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Ghost.Presentation.Characters.Editor
{
    public static class LilyPixelSpriteImporter
    {
        private static readonly string[] AssetPaths =
        {
            "Assets/Resources/Characters/LilyPixelFullBody.png",
            "Assets/Resources/Characters/LilyPixelPortrait.png"
        };

        [MenuItem("Ghost/Characters/Repair Lily Pixel Sprite Imports")]
        public static void RepairLilyPixelSpriteImports()
        {
            foreach (string assetPath in AssetPaths)
            {
                ConfigureSprite(assetPath);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Repaired Lily pixel sprite imports.");
        }

        private static void ConfigureSprite(string assetPath)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                throw new InvalidOperationException($"Texture importer not found for {assetPath}.");
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100f;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Point;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }
}
#endif
