using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class RoleSelectPanel : BasePanel
{
    public Button btnBack;
    public Button btnSure;
    private bool isInit=false;
    protected override void Init()
    {
        if (isInit)
            return;
        btnBack.onClick.AddListener(() =>{
            UIManager.Instance.HidePanel<RoleSelectPanel>();
            UIManager.Instance.ShowPanel<MainPanel>();
        });

        isInit = true;
    }
}
