using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public InputField inputUN;
    public InputField inputPW;
    public Button btnLogin;
    public Button btnRe;
    private bool isInit=false;
    protected override void Init()
    {
        if (isInit)
            return;
        EventCallBack.Instance.Bind(btnLogin, nameof(EventCallBack.Login), this);

        EventCallBack.Instance.Bind(btnRe, nameof(EventCallBack.OpenRegister));
        isInit = true;
    }

    public override void Hide(UnityAction action)
    {
        base.Hide(action);
        inputPW.text = string.Empty;
        inputUN.text = string.Empty;
    }
}
