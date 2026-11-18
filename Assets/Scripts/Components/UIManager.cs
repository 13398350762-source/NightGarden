using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIManager : BaseManager<UIManager>
{
    private Dictionary<string, BasePanel> panelDic;
    private Transform targetTrans;
    private Dictionary<string, List<UnityAction<BasePanel>>> loadingPanels = new Dictionary<string, List<UnityAction<BasePanel>>>();
    private int loadVersion;
    public UIManager() 
    {
        panelDic = new Dictionary<string, BasePanel>();
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
            canvas = ResManager.Instance.LoadRes<GameObject>("canvas", "Canvas");
        if (canvas == null)
        {
            canvas = new GameObject("Canvas", typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject events = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem));
            events.transform.SetParent(canvas.transform, false);
            events.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>().AssignDefaultActions();
        }
        targetTrans = canvas.transform;
        GameObject.DontDestroyOnLoad(targetTrans);
        SceneManager.sceneLoaded += RemoveDuplicateCanvas;
    }

    private void RemoveDuplicateCanvas(Scene scene, LoadSceneMode mode)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.transform == targetTrans || root.name != "Canvas" || !root.TryGetComponent<Canvas>(out _))
                continue;
            root.SetActive(false);
            Object.Destroy(root);
        }
    }
    public void ShowPanel<T>(UnityAction<T> callback=null)where T:BasePanel
    {
        string panelName=typeof(T).Name;
        if (panelDic.TryGetValue(panelName, out BasePanel existing) && existing != null)
        {
            existing.Show();
            callback?.Invoke(existing as T);
            return;
        }
        if (loadingPanels.TryGetValue(panelName, out List<UnityAction<BasePanel>> waiting))
        {
            if (callback != null) waiting.Add(panel => callback(panel as T));
            return;
        }
        waiting = new List<UnityAction<BasePanel>>();
        if (callback != null) waiting.Add(panel => callback(panel as T));
        loadingPanels.Add(panelName, waiting);
        int version = loadVersion;
        PoolManager.Instance.GetObj(panelName, "panel", (panel) => { 
            GameObject obj = panel as GameObject;
            if (obj == null || version != loadVersion || targetTrans == null)
            {
                if (obj != null) GameObject.Destroy(obj);
                if (version == loadVersion) loadingPanels.Remove(panelName);
                return;
            }
            loadingPanels.Remove(panelName);
            obj.transform.SetParent(targetTrans,false);
            T panelScript = obj.GetComponent<T>();
            panelDic[panelName] = panelScript;
            panelScript.Show();
            foreach (var action in waiting)
            {
                if (version != loadVersion || panelScript == null) break;
                action(panelScript);
            }
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

    public void Clear()
    {
        loadVersion++;
        loadingPanels.Clear();
        foreach (var item in panelDic.Values)
        {
            if (item != null)
            {
                item.gameObject.SetActive(false);
                GameObject.Destroy(item.gameObject);
            }
        }
        panelDic.Clear();
    }
}
