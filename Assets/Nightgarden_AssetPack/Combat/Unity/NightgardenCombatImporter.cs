#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nightgarden.CombatKit
{
    public static class NightgardenCombatImporter
    {
        [Serializable] public class Manifest { public Sheet[] sheets; public Component[] components; }
        [Serializable] public class Sheet { public string file; public int height; public Slice[] sprites; }
        [Serializable] public class Slice { public string id; public int x, y, width, height; }
        [Serializable] public class Component { public string file; public int[] border; }

        static string Root()
        {
            var scripts = AssetDatabase.FindAssets("NightgardenCombatImporter t:MonoScript");
            foreach (var guid in scripts)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileName(path) == "NightgardenCombatImporter.cs")
                    return Path.GetDirectoryName(Path.GetDirectoryName(path)).Replace('\\', '/');
            }
            throw new FileNotFoundException("Place the complete Combat folder under Assets.");
        }

        static TextureImporter Configure(string path)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer == null) throw new FileNotFoundException(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 2048;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.filterMode = FilterMode.Bilinear;
            importer.spritePixelsPerUnit = 100;
            return importer;
        }

        [MenuItem("Tools/Nightgarden/Import Weapon Visuals")]
        public static void Import()
        {
            string root = Root();
            var manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(root + "/Data/unity_slices.json"));
            foreach (var sheet in manifest.sheets)
            {
                var importer = Configure(root + "/" + sheet.file);
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.spritesheet = sheet.sprites.Select(s => new SpriteMetaData {
                    name = s.id, rect = new Rect(s.x, sheet.height-s.y-s.height, s.width, s.height),
                    alignment = (int)SpriteAlignment.Center, pivot = new Vector2(.5f,.5f)
                }).ToArray();
                importer.SaveAndReimport();
            }
            foreach (var item in manifest.components)
            {
                var importer = Configure(root + "/" + item.file);
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spriteBorder = new Vector4(item.border[0],item.border[1],item.border[2],item.border[3]);
                importer.SaveAndReimport();
            }
            AssetDatabase.SaveAssets();
            Debug.Log("Nightgarden UI: imported sprite regions and sliced UI components.");
        }

        public static void Validate()
        {
            Import();
            string root = Root();
            var manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(root + "/Data/unity_slices.json"));
            int count=0;
            foreach(var sheet in manifest.sheets)
            {
                var loaded = AssetDatabase.LoadAllAssetsAtPath(root+"/"+sheet.file).OfType<Sprite>().ToArray();
                foreach(var slice in sheet.sprites)
                {
                    var sprite=loaded.SingleOrDefault(s=>s.name==slice.id);
                    if(sprite==null || sprite.rect.width!=slice.width || sprite.rect.height!=slice.height)
                        throw new Exception("Invalid slice: "+slice.id);
                    count++;
                }
            }
            foreach(var component in manifest.components)
                if(AssetDatabase.LoadAssetAtPath<Sprite>(root+"/"+component.file)==null)
                    throw new Exception("Invalid UI component: "+component.file);
            File.WriteAllText(Path.Combine(Application.dataPath,"../ui_kit_validation.txt"),
                "PASS: "+count+" sprite regions; "+manifest.components.Length+" components; Unity import verified.");
        }
    }
}
#endif
