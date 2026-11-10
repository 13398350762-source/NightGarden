using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResManager : MonoAutoSingleton<ResManager>
{
    private AssetBundle MainAB = null;
    private AssetBundleManifest manifest = null;
    private Dictionary<string, AssetBundle> abDic;
    private string PathUrl = Application.streamingAssetsPath + "/";
    private string MainABName = "PC";

    private void LoadAB(string abName)
    {
        if(abDic==null)
            abDic = new Dictionary<string, AssetBundle>();
        if (MainAB == null)
        {
            MainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            manifest = MainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        AssetBundle ab = null;
        string[] dependecies = manifest.GetAllDependencies(abName);
        for (int i = 0; i < dependecies.Length; i++)
        {
            if (!abDic.ContainsKey(dependecies[i]))
            {
                ab = AssetBundle.LoadFromFile(PathUrl + dependecies[i]);
                abDic.Add(dependecies[i], ab);
            }
        }
        if (!abDic.ContainsKey(abName))
        {
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            abDic.Add(abName, ab);
        }
    }

    public Object LoadRes(string abName,string resName)
    {
        LoadAB(abName);
        Object obj = abDic[abName].LoadAsset(resName);
        if (obj is GameObject)
            return Instantiate(obj);
        else 
            return obj;
    }

    public Object LoadRes(string abName, string resName, System.Type type)
    {
        LoadAB (abName);
        Object obj = abDic[abName].LoadAsset(resName, type);
        if (obj is GameObject)
            return Instantiate(obj);
        else
            return obj;
    }

    public T LoadRes<T>(string abName, string resName)where T : Object
    {
        LoadAB(abName);
        T obj=abDic[abName].LoadAsset<T>(resName);
        if (obj is GameObject)
            return Instantiate(obj);
        else
            return obj;

    }

    public void LoadResAsync(string abName,string resName,UnityAction<Object> callback)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, callback));
    }

    private IEnumerator ReallyLoadResAsync(string abName,string resName,UnityAction<Object> callback)
    {
        LoadAB(abName);
        AssetBundleRequest request = abDic[abName].LoadAssetAsync(resName);
        yield return request;
        Object obj = request.asset;
        if (obj is GameObject)
            callback?.Invoke(Instantiate(obj));
        else
            callback?.Invoke(obj);
    }

    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callback)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName,type, callback));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callback)
    {
        LoadAB(abName);
        AssetBundleRequest request = abDic[abName].LoadAssetAsync(resName,type);
        yield return request;
        Object obj = request.asset;
        if (obj is GameObject)
            callback?.Invoke(Instantiate(obj));
        else
            callback?.Invoke(obj);
    }

    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callback)where T:Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callback));
    }
    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName,UnityAction<T> callback)where T:Object
    {
        LoadAB(abName);
        AssetBundleRequest request = abDic[abName].LoadAssetAsync<T>(resName);
        yield return request;
        T obj = request.asset as T;
        callback?.Invoke(obj is GameObject ? Instantiate(obj) : obj);
    }
}
