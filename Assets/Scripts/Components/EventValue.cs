using UnityEngine;

// 通过事件传递控件所属对象和本次输入，不在全局监听中保存面板实例。
public class EventValue<T>
{
    public MonoBehaviour sender;
    public T value;

    public EventValue(MonoBehaviour sender, T value)
    {
        this.sender = sender;
        this.value = value;
    }
}
