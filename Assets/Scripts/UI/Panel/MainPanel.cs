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
        btnEnter.onClick.AddListener(() =>{
            //进入选人面板
            UIManager.Instance.ShowPanel<RoleSelectPanel>(); 
            UIManager.Instance.HidePanel<MainPanel>();
        });
        btnExit.onClick.AddListener(() =>{
            Application.Quit();
        });
        btnRank.onClick.AddListener(() =>{
            //进入排行榜面板
            UIManager.Instance.ShowPanel<RankPanel>();
            UIManager.Instance.HidePanel<MainPanel>();
        });
        btnSetting.onClick.AddListener(() =>{
            //进入设置面板
            UIManager.Instance.ShowPanel<SettingPanel>();
            UIManager.Instance.HidePanel<MainPanel>();
        });
        btnReturn.onClick.AddListener(() =>{
            UIManager.Instance.HidePanel<MainPanel>();
            ScenesManager.Instance.LoadSceneAsync("LoginScene", () =>{
                UIManager.Instance.ShowPanel<LoginPanel>();
            });
        });
        isInit = true;
    }

}
