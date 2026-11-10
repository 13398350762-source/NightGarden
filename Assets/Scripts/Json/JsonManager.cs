using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum JsonType
{
    Utlity,
    LitJson
}
public class JsonManager : BaseManager<JsonManager>
{
    public void Save(object data, string filename, JsonType type = JsonType.LitJson)
    {
        string path = Application.persistentDataPath + "/" + filename;
        string str = "";
        switch (type)
        {
            case JsonType.Utlity:
                str = JsonUtility.ToJson(data);
                break;
            case JsonType.LitJson:
                str = JsonMapper.ToJson(data);
                break;
        }
        File.WriteAllText(path, str);
    }

    public T Load<T>(string filename, JsonType type = JsonType.LitJson) where T : new()
    {
        string path = Application.streamingAssetsPath + "/" + filename;
        if (!File.Exists(path))
        {
            path = Application.persistentDataPath + "/" + filename;
        }
        if (!File.Exists(path))
            return new T();
        string str = File.ReadAllText(path);
        T data = default(T);
        switch (type)
        {
            case JsonType.Utlity:
                data = JsonUtility.FromJson<T>(str);
                break;
            case JsonType.LitJson:
                data = JsonMapper.ToObject<T>(str);
                break;
        }
        return data;
    }
}
