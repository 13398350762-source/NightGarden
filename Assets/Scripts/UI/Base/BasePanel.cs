using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public abstract class BasePanel : MonoBehaviour
{
    private bool isShow;
    private CanvasGroup canvasGroup;
    public float speed=5.0f;
    private UnityAction callback;
    protected abstract void Init();

    virtual protected void Start()
    {
        InitCanvasGroup();
    }

    private void InitCanvasGroup()
    {
        if (canvasGroup != null) return;
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup==null)
            canvasGroup=this.AddComponent<CanvasGroup>();
    }

    virtual protected void Update()
    {
        if (isShow)
        {
            canvasGroup.alpha += speed * Time.unscaledDeltaTime;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
            }
        }
        else
        {
            canvasGroup.alpha -= speed * Time.unscaledDeltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                UnityAction action = callback;
                callback = null;
                action?.Invoke();
            }
        }
    }

    virtual public void Show()
    {
        InitCanvasGroup();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        isShow = true;
        callback = null;
        Init();
    }

    virtual public void Hide(UnityAction action)
    {
        InitCanvasGroup();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        isShow=false;
        callback = action;
    }
}
