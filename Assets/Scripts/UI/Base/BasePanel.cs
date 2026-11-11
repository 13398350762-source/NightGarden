using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasePanel : MonoBehaviour
{
    private bool isShow;
    private CanvasGroup canvasGroup;
    public float speed=1.0f;
    private UnityAction callback;
    protected abstract void Init();
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup==null)
            canvasGroup=this.AddComponent<CanvasGroup>();
    }

    // Update is called once per frame
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
