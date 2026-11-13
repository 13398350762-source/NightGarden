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
        btnContinue.onClick.AddListener(() =>{
            //暂停取消
            UIManager.Instance.HidePanel<PausePanel>();
        });

        btnReStart.onClick.AddListener(() =>{
            //重新开始
            UIManager.Instance.HidePanel<PausePanel>();
        });

        btnSetting.onClick.AddListener(() =>{
            UIManager.Instance.ShowPanel<SettingPanel>();
        });
        btnReturn.onClick.AddListener(() =>{
            //时间恢复正常
            PoolManager.Instance.Clear();
            UIManager.Instance.Clear();
            UIManager.Instance.ShowPanel<MainPanel>();
        });
        isInit = true;
    }

    public override void Hide(UnityAction action)
    {
        base.Hide(action);

        //时间流逝恢复正常
    }
}
