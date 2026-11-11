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
        btnSure.onClick.AddListener(() =>{
            UIManager.Instance.HidePanel<TipPanel>();
        });
        isInit = true;
    }

    public void ChangeInfo(string tip)
    {
        txtTip.text = tip;
    }
}
