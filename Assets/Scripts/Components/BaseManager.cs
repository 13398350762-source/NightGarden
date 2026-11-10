using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager<T> where T:new()
{
    static private T instance;
    static public T Instance 
    {
        get 
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
}
