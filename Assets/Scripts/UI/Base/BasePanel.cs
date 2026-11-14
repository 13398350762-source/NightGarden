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

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup==null)
            canvasGroup=this.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        if (isShow)
        {
            canvasGroup.alpha += speed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
            }
        }
        else
        {
            canvasGroup.alpha -= speed * Time.deltaTime;
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
        isShow = true;
        callback = null;
        Init();
    }

    virtual public void Hide(UnityAction action)
    {
        isShow=false;
        callback = action;
    }
}
