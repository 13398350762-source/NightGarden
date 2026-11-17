using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PausePanel : BasePanel
{
    public Button btnContinue;
    public Button btnReStart;
    public Button btnSetting;
    public Button btnReturn;
    private bool isInit=false;
    protected override void Init()
    {
        if (isInit)
            return;
        EventCallBack.Instance.Bind(btnContinue, nameof(EventCallBack.ContinueGame));

        EventCallBack.Instance.Bind(btnReStart, nameof(EventCallBack.RestartGame));

        EventCallBack.Instance.Bind(btnSetting, nameof(EventCallBack.OpenSettings), (BasePanel)this);
        EventCallBack.Instance.Bind(btnReturn, nameof(EventCallBack.ReturnToMain));
        isInit = true;
    }

    public override void Hide(UnityAction action)
    {
        base.Hide(action);

        //时间流逝恢复正常
        Time.timeScale = 1.0f;
    }

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0;
    }
}
