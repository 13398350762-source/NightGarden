using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    public Button btnEnter;
    public Button btnExit;
    public Button btnRank;
    public Button btnSetting;
    public Button btnReturn;
    private bool isInit=false;
    protected override void Init()
    {
        if (isInit)
            return;
        EventCallBack.Instance.Bind(btnEnter, nameof(EventCallBack.EnterRoleSelect));
        EventCallBack.Instance.Bind(btnExit, nameof(EventCallBack.QuitGame));
        EventCallBack.Instance.Bind(btnRank, nameof(EventCallBack.OpenRank));
        EventCallBack.Instance.Bind(btnSetting, nameof(EventCallBack.OpenSettings), (BasePanel)this);
        EventCallBack.Instance.Bind(btnReturn, nameof(EventCallBack.Logout));
        isInit = true;
    }

}
