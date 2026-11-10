#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Reads atlas_manifest.json next to this package. Does not alter other assets.
public static class NightgardenImporter
{
    [Serializable] private class Manifest { public Sheet[] atlases; }
    [Serializable] private class Sheet { public string id; public string path; public int width; public int height; public int ppu; public SpriteEntry[] sprites; }
    [Serializable] private class SpriteEntry { public string id; public int x; public int y; public int width; public int height; public float pivotX; public float pivotY; }

    [MenuItem("Tools/Nightgarden/Import and Slice Asset Pack")]
    public static void Import()
    {
        var matches = AssetDatabase.FindAssets("NightgardenImporter t:MonoScript")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => p.EndsWith("/Unity/Editor/NightgardenImporter.cs", StringComparison.Ordinal)).ToArray();
        if (matches.Length != 1) { Debug.LogError("Keep exactly one NightgardenImporter.cs inside the package's Unity/Editor folder."); return; }
        var root = matches[0].Substring(0, matches[0].Length - "Unity/Editor/NightgardenImporter.cs".Length);
        var manifestPath = root + "atlas_manifest.json";
        if (!File.Exists(manifestPath)) { Debug.LogError("Missing " + manifestPath); return; }
        var manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(manifestPath));
        var count = 0;
        foreach (var sheet in manifest.atlases)
        {
            var path = root + sheet.path;
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) { Debug.LogError("Missing texture " + path); continue; }
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = sheet.id == "arena" ? SpriteImportMode.Single : SpriteImportMode.Multiple;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 2048;
            importer.spritePixelsPerUnit = sheet.ppu;
            importer.spritePivot = new Vector2(.5f, .5f);
            if (sheet.id != "arena")
            {
                var entries = new SpriteMetaData[sheet.sprites.Length];
                for (var i = 0; i < entries.Length; i++)
                {
                    var s = sheet.sprites[i];
                    entries[i] = new SpriteMetaData {
                        name = s.id,
                        rect = new Rect(s.x, sheet.height - s.y - s.height, s.width, s.height),
                        alignment = (int)SpriteAlignment.Custom,
                        pivot = new Vector2(s.pivotX, s.pivotY),
                        border = Vector4.zero
                    };
                }
                // Legacy API avoids a hard dependency on the separate 2D Sprite editor package.
                // Newer Unity versions may show an obsolete warning for this compatibility path.
#pragma warning disable 0618
                importer.spritesheet = entries;
#pragma warning restore 0618
            }
            importer.SaveAndReimport();
            count += sheet.sprites.Length;
        }
        Debug.Log("Nightgarden: imported " + count + " sprites/backgrounds. Music loops must be enabled on your AudioSource.");
    }
}
#endif
