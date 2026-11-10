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
        obj.transform.SetParent(Parentobj.transform, false);
        objList.Add(obj);
    }
    public void GetObject(string abName,UnityAction<Object> action)
    {
        if (objList.Count != 0)
        {
            GameObject obj= objList[0];
            objList.RemoveAt(0);
            obj.transform.SetParent(null, false);
            action?.Invoke(obj);
        }
        else
        {
            ResManager.Instance.LoadResAsync(abName,Parentobj.name,action);
        }
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
        if(poolobj==null)
            poolobj = new GameObject("pool");
        poolobj.SetActive(false);
        if (!poolDic.ContainsKey(name))
            poolDic.Add(name, new PoolData(name,poolobj));
        poolDic[name].PushObject(obj);
    }
    public void GetObj(string resName,string abName,UnityAction<Object> action)
    {
        if (poolDic.ContainsKey(resName))
        {
            poolDic[resName].GetObject(abName,action);
        }
        else
        {
            ResManager.Instance.LoadResAsync(abName, resName, action);
        }
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
