using UnityEngine;
using UnityEngine.Events;

// 控件重新绑定时只移除自己的旧监听，不影响 Inspector 中配置的事件。
public class EventBinding : MonoBehaviour
{
    private UnityAction removeListener;

    public void Replace(UnityAction remove)
    {
        removeListener?.Invoke();
        removeListener = remove;
    }

    private void OnDestroy()
    {
        removeListener?.Invoke();
        removeListener = null;
    }
}
