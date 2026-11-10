using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoAutoSingleton<T> : MonoBehaviour where T: MonoBehaviour
{
    static private T instance;
    static public T Instance
    {
        get 
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(T).Name);
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

}
