using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class MonoController : MonoBehaviour
{
    private event UnityAction updateEvents;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (updateEvents != null)
            updateEvents();
    }

    public void AddUpdateEvent(UnityAction action)
    {
        updateEvents += action;
    }

    public void RemoveUpdateEvent(UnityAction action)
    {
        updateEvents -= action;
    }
}
