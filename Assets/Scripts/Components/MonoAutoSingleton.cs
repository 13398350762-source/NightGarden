using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoAutoSingleton<T> : MonoBehaviour where T: MonoBehaviour
{
    private GameObject obj;
    private T instance;
    public T Instance
    {
        get 
        {
            if (instance == null)
            {
                obj = new GameObject(typeof(T).Name);
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

}
