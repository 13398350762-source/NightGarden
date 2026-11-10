#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class NightgardenAnimationBuilder
{
    [Serializable] private class Manifest { public Atlas[] atlases; public Character[] characters; public ParticlePreset[] particles; }
    [Serializable] private class Atlas { public string id; public string path; public int width; public int height; public float ppu; public Frame[] sprites; }
    [Serializable] private class Frame { public string id; public int x; public int y; public int width; public int height; public float pivotX; public float pivotY; }
    [Serializable] private class Character { public string id; public string label; public string[] move; public string[] attack; public float moveFps; public float attackFps; }
    [Serializable] private class ParticlePreset { public string id; public string sprite; public int count; public bool loop; public float lifetime; public float speed; public float size; public float gravity; public bool expand; }

    [MenuItem("Tools/Nightgarden/Build Animations and Particles")]
    public static void Build()
    {
        var paths = AssetDatabase.FindAssets("NightgardenAnimationBuilder t:MonoScript").Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => p.EndsWith("/Unity/Editor/NightgardenAnimationBuilder.cs", StringComparison.Ordinal)).ToArray();
        if (paths.Length != 1) throw new InvalidOperationException("Keep exactly one Nightgarden animation builder in the package.");
        var root = paths[0].Substring(0, paths[0].Length - "Unity/Editor/NightgardenAnimationBuilder.cs".Length);
        var manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(root + "Animations/animation_manifest.json"));
        var output = root + "GeneratedV32";
        // A new output folder protects any previously generated assets that the user has edited.
        if (AssetDatabase.IsValidFolder(output)) output = AssetDatabase.GenerateUniqueAssetPath(output);
        Directory.CreateDirectory(output);
        Directory.CreateDirectory(output + "/Characters");
        Directory.CreateDirectory(output + "/Particles");
        AssetDatabase.Refresh();
        var sprites = new Dictionary<string, Sprite>();
        foreach (var atlas in manifest.atlases)
        {
            var path = root + atlas.path;
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) throw new FileNotFoundException(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 4096;
            importer.spritePixelsPerUnit = atlas.ppu;
            var entries = atlas.sprites.Select(s => new SpriteMetaData {
                name = s.id, rect = new Rect(s.x, atlas.height-s.y-s.height, s.width, s.height),
                alignment = (int)SpriteAlignment.Custom, pivot = new Vector2(s.pivotX, s.pivotY)
            }).ToArray();
#pragma warning disable 0618
            importer.spritesheet = entries;
#pragma warning restore 0618
            importer.SaveAndReimport();
            foreach (var sprite in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>()) sprites.Add(sprite.name, sprite);
        }
        var characterPrefabs = new List<GameObject>();
        foreach (var character in manifest.characters)
        {
            var dir = output + "/Characters/" + character.id;
            Directory.CreateDirectory(dir); AssetDatabase.Refresh();
            var idle = Clip(dir + "/Idle.anim", new[] { sprites[character.move[0]] }, 2, true);
            var move = Clip(dir + "/Move.anim", character.move.Select(id => sprites[id]).ToArray(), character.moveFps, true);
            var attack = character.attack.Length > 0 ? Clip(dir + "/Charge.anim", character.attack.Select(id => sprites[id]).ToArray(), character.attackFps, false) : null;
            var controller = AnimatorController.CreateAnimatorControllerAtPath(dir + "/Character.controller");
            controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
            if (attack != null) controller.AddParameter("Charge", AnimatorControllerParameterType.Trigger);
            var sm = controller.layers[0].stateMachine;
            var idleState = sm.AddState("Idle"); idleState.motion = idle;
            var moveState = sm.AddState("Move"); moveState.motion = move;
            AnimatorState attackState = null; if (attack != null) { attackState = sm.AddState("Charge"); attackState.motion = attack; }
            sm.defaultState = idleState;
            Transition(idleState, moveState, true, false);
            Transition(moveState, idleState, false, false);
            if (attack != null) {
            Transition(attackState, moveState, true, true);
            Transition(attackState, idleState, false, true);
            var attackTransition = sm.AddAnyStateTransition(attackState);
            attackTransition.hasExitTime = false; attackTransition.duration = 0; attackTransition.canTransitionToSelf = false;
            attackTransition.AddCondition(AnimatorConditionMode.If, 0, "Charge");
            }
            var go = new GameObject(character.id);
            go.AddComponent<SpriteRenderer>().sprite = sprites[character.move[0]];
            go.AddComponent<Animator>().runtimeAnimatorController = controller;
            go.AddComponent<NightgardenAnimationDriver>();
            characterPrefabs.Add(PrefabUtility.SaveAsPrefabAsset(go, dir + "/" + character.id + ".prefab"));
            UnityEngine.Object.DestroyImmediate(go);
        }
        Directory.CreateDirectory(output + "/Projectiles"); AssetDatabase.Refresh();
        foreach (var pair in sprites.Where(p => p.Key.StartsWith("enemy_orb_")))
        {
            var bullet = new GameObject(pair.Key);
            var renderer = bullet.AddComponent<SpriteRenderer>(); renderer.sprite = pair.Value; renderer.sortingOrder=30;
            bullet.AddComponent<CircleCollider2D>().isTrigger=true;
            bullet.GetComponent<CircleCollider2D>().radius=Mathf.Min(pair.Value.bounds.extents.x,pair.Value.bounds.extents.y)*.8f;
            var body=bullet.AddComponent<Rigidbody2D>(); body.bodyType=RigidbodyType2D.Kinematic; body.gravityScale=0;
            PrefabUtility.SaveAsPrefabAsset(bullet,output+"/Projectiles/"+pair.Key+".prefab");
            UnityEngine.Object.DestroyImmediate(bullet);
        }
        var shader = Shader.Find("Nightgarden/Particle");
        if (shader == null) throw new InvalidOperationException("Nightgarden/Particle shader was not imported.");
        var material = new Material(shader) { mainTexture = sprites[manifest.particles[0].sprite].texture };
        AssetDatabase.CreateAsset(material, output + "/Particles/NightgardenParticle.mat");
        foreach (var preset in manifest.particles) MakeParticle(preset, sprites[preset.sprite], material, output + "/Particles/");
        AssetDatabase.SaveAssets();
        // Batch validation owns its empty scene. In interactive use, leave unsaved scenes untouched.
        if (!Application.isBatchMode && Enumerable.Range(0, UnityEngine.SceneManagement.SceneManager.sceneCount)
            .Any(i => string.IsNullOrEmpty(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).path)))
        {
            Debug.LogWarning("Nightgarden assets built. Preview scene creation skipped because an untitled scene is open. Use the included AnimationPreview scene, or save the open scene before rebuilding.");
            return;
        }
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, Application.isBatchMode ? NewSceneMode.Single : NewSceneMode.Additive);
        var cameraGo = new GameObject("Preview Camera"); UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraGo, scene);
        var camera = cameraGo.AddComponent<Camera>(); camera.orthographic = true; camera.orthographicSize = 6.8f;
        camera.backgroundColor = new Color(.08f, .14f, .11f); camera.clearFlags = CameraClearFlags.SolidColor;
        cameraGo.transform.position = new Vector3(0, 0, -10);
        for (var i = 0; i < characterPrefabs.Count; i++)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(characterPrefabs[i], scene);
            go.transform.position = new Vector3((i%8-3.5f)*2.4f, 3.9f-(i/8)*2.8f, 0);
            go.GetComponent<NightgardenAnimationDriver>().previewAutomatically = true;
        }
        EditorSceneManager.SaveScene(scene, output + "/AnimationPreview.unity");
        EditorSceneManager.CloseScene(scene, true);
        AssetDatabase.SaveAssets();
        Debug.Log("Nightgarden animations built: " + manifest.characters.Length + " characters, " + manifest.particles.Length + " particle prefabs, output: " + output);
    }

    private static AnimationClip Clip(string path, Sprite[] sprites, float fps, bool loop)
    {
        var clip = new AnimationClip { name = Path.GetFileNameWithoutExtension(path), frameRate = fps };
        var frames = new ObjectReferenceKeyframe[sprites.Length];
        for (var i=0; i<sprites.Length; i++) frames[i] = new ObjectReferenceKeyframe { time=i/fps, value=sprites[i] };
        AnimationUtility.SetObjectReferenceCurve(clip, new EditorCurveBinding { path="", type=typeof(SpriteRenderer), propertyName="m_Sprite" }, frames);
        var settings = AnimationUtility.GetAnimationClipSettings(clip); settings.loopTime=loop; AnimationUtility.SetAnimationClipSettings(clip, settings);
        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    private static void Transition(AnimatorState from, AnimatorState to, bool moving, bool exit)
    {
        var transition = from.AddTransition(to);
        transition.hasExitTime=exit; transition.exitTime=1; transition.duration=0;
        transition.AddCondition(moving?AnimatorConditionMode.If:AnimatorConditionMode.IfNot, 0, "IsMoving");
    }

    private static void MakeParticle(ParticlePreset p, Sprite sprite, Material material, string path)
    {
        var go = new GameObject(p.id); var ps=go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main=ps.main; main.duration=1; main.loop=p.loop; main.playOnAwake=true;
        main.startLifetime=new ParticleSystem.MinMaxCurve(p.lifetime*.7f,p.lifetime);
        main.startSpeed=new ParticleSystem.MinMaxCurve(p.speed*.4f,p.speed);
        main.startSize=new ParticleSystem.MinMaxCurve(p.size*.7f,p.size);
        main.startRotation=new ParticleSystem.MinMaxCurve(0,Mathf.PI*2);
        main.gravityModifier=p.gravity; main.simulationSpace=ParticleSystemSimulationSpace.World; main.maxParticles=200;
        var emission=ps.emission; emission.rateOverTime=p.loop?p.count:0;
        if (!p.loop) emission.SetBursts(new[] {new ParticleSystem.Burst(0,(short)p.count)});
        var shape=ps.shape; shape.enabled=true; shape.shapeType=ParticleSystemShapeType.Circle; shape.radius=p.expand?0:.08f;
        var size=ps.sizeOverLifetime; size.enabled=true;
        size.size=new ParticleSystem.MinMaxCurve(1,AnimationCurve.Linear(0,p.expand?.15f:1,1,p.expand?4:0));
        var color=ps.colorOverLifetime; color.enabled=true;
        var gradient=new Gradient(); gradient.SetKeys(new[] {new GradientColorKey(Color.white,0),new GradientColorKey(Color.white,1)},new[] {new GradientAlphaKey(1,0),new GradientAlphaKey(.8f,.5f),new GradientAlphaKey(0,1)});
        color.color=new ParticleSystem.MinMaxGradient(gradient);
        var texture=ps.textureSheetAnimation; texture.enabled=true; texture.mode=ParticleSystemAnimationMode.Sprites; texture.AddSprite(sprite);
        texture.frameOverTime=new ParticleSystem.MinMaxCurve(0);
        var renderer=go.GetComponent<ParticleSystemRenderer>(); renderer.sharedMaterial=material;
        renderer.renderMode=ParticleSystemRenderMode.Billboard; renderer.sortingOrder=20;
        renderer.shadowCastingMode=ShadowCastingMode.Off; renderer.receiveShadows=false;
        PrefabUtility.SaveAsPrefabAsset(go,path+p.id+".prefab"); UnityEngine.Object.DestroyImmediate(go);
    }
}
#endif
