using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    public Button btnSure;
    public Text txtTip;
    private bool isInit=false;
    protected override void Init()
    {
        if (isInit)
            return;
        EventCallBack.Instance.Bind(btnSure, nameof(EventCallBack.CloseTip));
        isInit = true;
    }

    public void ChangeInfo(string tip)
    {
        txtTip.text = tip;
    }
}
