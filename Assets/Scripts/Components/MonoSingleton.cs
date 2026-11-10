using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private T instance;
    public T Instance
    {
        get 
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this as T;
    }
}
