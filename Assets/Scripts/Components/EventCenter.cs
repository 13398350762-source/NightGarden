using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfo
{
    
}

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions; 
}
public class EventCenter : BaseManager<EventCenter>
{
    private Dictionary<string, IEventInfo> eventsDic;
    public EventCenter()
    {
        eventsDic = new Dictionary<string, IEventInfo>();
    }
    public void AddListener<T>(string name,UnityAction<T> action)
    {

        if (!eventsDic.ContainsKey(name))
        {
            EventInfo<T> eventinfo = new EventInfo<T>();
            eventinfo.actions = action;
            eventsDic.Add(name, eventinfo);
        }
        else
        {
            (eventsDic[name] as EventInfo<T>).actions += action;
        }
    }
    public void RemoveListener<T>(string name,UnityAction<T> action)
    {
        if (eventsDic.ContainsKey(name))
        {
            (eventsDic[name] as EventInfo<T>).actions -= action;
        }
    }
    public void EventTrigger<T>(string name,T obj)
    {
        if (eventsDic.ContainsKey(name))
        {
            (eventsDic[name] as EventInfo<T>).actions?.Invoke(obj);
        }
    }
    public void Clear()
    {
        eventsDic.Clear();
    }
}
