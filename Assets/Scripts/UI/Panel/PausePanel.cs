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
            RoleData data = PlayerInfoManager.Instance.GetRole();
            PlayerInfoManager.Instance.Clear();
            PlayerInfoManager.Instance.SetRole(data);
            PlayerInfoManager.Instance.SetInitialWeapon(PlayerInfoManager.Instance.initialWeapon);
            GameManager.Instance.Clear();
            Time.timeScale = 1.0f;
            UIManager.Instance.HidePanel<PausePanel>();
            //重新加载游戏
            GameObject.Find("Player").GetComponent<PlayerController>().Init();
        });

        btnSetting.onClick.AddListener(() =>{
            UIManager.Instance.ShowPanel<SettingPanel>();
        });
        btnReturn.onClick.AddListener(() =>{
            //时间恢复正常
            Time.timeScale = 1.0f;
            PoolManager.Instance.Clear();
            UIManager.Instance.Clear();
            PlayerInfoManager.Instance.Clear();
            ScenesManager.Instance.LoadSceneAsync("BeginScene", () =>{
                UIManager.Instance.ShowPanel<MainPanel>();
            });
        });
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
