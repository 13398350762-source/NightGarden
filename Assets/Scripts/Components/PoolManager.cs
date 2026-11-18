using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PoolData
{
    public GameObject Parentobj;
    public List<GameObject> objList=null;
    public void PushObject(GameObject obj)
    {
        if (objList == null)
            objList = new List<GameObject>();
        if (objList.Contains(obj)) return;
        obj.transform.SetParent(Parentobj.transform, false);
        objList.Add(obj);
    }
    public void GetObject(string abName,UnityAction<Object> action)
    {
        GameObject obj = TakeObject(false);
        if (obj != null)
            action?.Invoke(obj);
        else
            ResManager.Instance.LoadResAsync(abName, Parentobj.name, action);
    }

    public GameObject TakeObject(bool keepInactive)
    {
        while (objList.Count != 0)
        {
            GameObject obj= objList[0];
            objList.RemoveAt(0);
            if (obj == null) continue;
            if (keepInactive) obj.SetActive(false);
            obj.transform.SetParent(null, false);
            return obj;
        }
        return null;
    }
    public void Clear()
    {
        objList.Clear();
        GameObject.Destroy(Parentobj);
        Parentobj = null;
    }

    public PoolData(string name,GameObject obj)
    {
        Parentobj = new GameObject(name);
        Parentobj.transform.SetParent(obj.transform, false);
    }
}
public class PoolManager : BaseManager<PoolManager>
{
    private GameObject poolobj;
    private Dictionary<string, PoolData> poolDic;
    public PoolManager() 
    {
        poolDic=new Dictionary<string, PoolData>();
    }

    public void PushObj(string name, GameObject obj)
    {
        if (obj == null) return;
        GameObject root = GetPoolRoot();
        if (!poolDic.ContainsKey(name) || poolDic[name].Parentobj == null)
            poolDic[name] = new PoolData(name, root);
        poolDic[name].PushObject(obj);
    }

    private GameObject GetPoolRoot()
    {
        if(poolobj==null)
        {
            poolobj = new GameObject("pool");
            GameObject.DontDestroyOnLoad(poolobj);
        }
        poolobj.SetActive(false);
        return poolobj;
    }
    public void GetObj(string resName,string abName,UnityAction<Object> action)
    {
        if (poolDic.ContainsKey(resName) && poolDic[resName].Parentobj != null)
        {
            poolDic[resName].GetObject(abName,action);
        }
        else
        {
            poolDic.Remove(resName);
            ResManager.Instance.LoadResAsync(abName, resName, action);
        }
    }

    public void GetInactiveObj(string resName, string abName, UnityAction<GameObject> action)
    {
        if (poolDic.TryGetValue(resName, out PoolData pool) && pool.Parentobj != null)
        {
            GameObject existing = pool.TakeObject(true);
            if (existing != null)
            {
                action(existing);
                return;
            }
        }

        ResManager.Instance.LoadAssetAsync<GameObject>(abName, resName, prefab => {
            if (prefab == null)
            {
                action(null);
                return;
            }

            GameObject obj = Object.Instantiate(prefab, GetPoolRoot().transform, false);
            obj.SetActive(false);
            obj.transform.SetParent(null, false);
            action(obj);
        });
    }
    public void Clear()
    {
        foreach(var item in poolDic)
        {
            item.Value.Clear();
        }
        poolDic.Clear();
        if (poolobj != null)
        {
            GameObject.Destroy(poolobj);
            poolobj = null;
        }
    }
}
