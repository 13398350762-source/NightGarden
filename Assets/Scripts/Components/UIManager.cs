using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : BaseManager<UIManager>
{
    private Dictionary<string, BasePanel> panelDic;
    private Transform targetTrans;
    public UIManager() 
    {
        panelDic = new Dictionary<string, BasePanel>();
        targetTrans = GameObject.Find("Canvas").transform;
        GameObject.DontDestroyOnLoad(targetTrans);
    }
    public void ShowPanel<T>(UnityAction<T> callback=null)where T:BasePanel
    {
        string panelName=typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            callback?.Invoke(panelDic[panelName] as T);
            return;
        }
        T panelScript = null;
        PoolManager.Instance.GetObj(panelName, "Panel", (panel) => { 
            (panel as GameObject).transform.SetParent(targetTrans);
            panelScript = (panel as GameObject).GetComponent<T>();
            panelScript.Show();
            panelDic.Add(panelName, panelScript);
            callback?.Invoke(panelScript as T);
        });
    }

    public void HidePanel<T>(UnityAction callback = null) where T: BasePanel
    {
        string panelName=typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            T panelScript=panelDic[panelName] as T;
            panelScript.Hide(() => {
                panelDic.Remove(panelName);
                PoolManager.Instance.PushObj(panelName, panelScript.gameObject);
                callback?.Invoke();
            });
        }
    }

    public void GetPanel<T>(UnityAction<T> callback=null) where T : BasePanel
    {
        if (panelDic.ContainsKey(typeof(T).Name))
            callback?.Invoke(panelDic[typeof(T).Name] as T);
    }
}
