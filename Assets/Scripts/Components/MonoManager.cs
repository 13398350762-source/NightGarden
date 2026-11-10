using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoManager:BaseManager<MonoManager>
{
    private MonoController controller;
    public MonoManager() 
    {
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<MonoController>();
        GameObject.DontDestroyOnLoad(obj);
    }
    public void AddUpdateEvent(UnityAction action)
    {
        controller.AddUpdateEvent(action);
    }
    public void RemoveUpdateEvent(UnityAction action)
    {
        controller.RemoveUpdateEvent(action);
    }
    public Coroutine StartCoroutine(string methond)
    {
        return controller.StartCoroutine(methond);
    }
    public Coroutine StartCoroutine(string methodName,object value)
    {
        return controller.StartCoroutine(methodName, value);
    }
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }
    public void StopCoroutine(IEnumerator routine)
    {
        controller.StopCoroutine(routine);
    }
}