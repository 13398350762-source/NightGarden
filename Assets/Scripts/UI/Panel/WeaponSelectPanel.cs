using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectPanel : BasePanel
{
    public List<Toggle> togWeaponList= new List<Toggle>();
    public Button btnStart;
    public Button btnBack;
    public Text txtWeaponName;
    public Text txtAtk;
    public Text txtInternal;
    public Text txtMode;
    public Text txtType;
    public Image imgWeapon;
    protected override void Init()
    {
        btnBack.onClick.AddListener(() =>{
            UIManager.Instance.HidePanel<WeaponSelectPanel>();
            //显示选人界面
        });

        btnStart.onClick.AddListener(() =>{
            UIManager.Instance.HidePanel<WeaponSelectPanel>();
            ScenesManager.Instance.LoadSceneAsync("GameScene", () =>{
                //进入游戏场景后需要处理的事件
            });
        });
    }
}
