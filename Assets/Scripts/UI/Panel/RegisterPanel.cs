using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    public InputField inputUN;
    public InputField inputPW;
    public InputField inputSU;
    public Button btnRe;
    public Button btnBack;
    private bool isInit;
    protected override void Init()
    {
        if (isInit)
            return;
        EventCallBack.Instance.Bind(btnBack, nameof(EventCallBack.BackToLogin));

        EventCallBack.Instance.Bind(btnRe, nameof(EventCallBack.Register), this);
        isInit = true;
    }

    public override void Hide(UnityAction action)
    {
        base.Hide(action);
        inputUN.text = string.Empty;
        inputPW.text = string.Empty;
        inputSU.text = string.Empty;
    }
}
