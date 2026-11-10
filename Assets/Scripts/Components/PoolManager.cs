using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    public GameObject GetObject()
    {
        GameObject obj = null;
        if (objList.Count != 0)
        {
            obj= objList[0];
            objList.RemoveAt(0);
        }
        else
        {
            obj=GameObject.Instantiate(Resources.Load<GameObject>(Parentobj.name));
        }
        return obj;
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
    public GameObject GetObj(string name)
    {
        GameObject obj = null;
        if (poolDic.ContainsKey(name))
        {
            obj = poolDic[name].GetObject();
        }
        else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
        }
        return obj;
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
