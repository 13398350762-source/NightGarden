using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UI;

public class WeaponSelectPanel : BasePanel
{
    public List<Toggle> togWeaponList= new List<Toggle>();
    public List<WeaponData> weaponDatas = new List<WeaponData>();
    public Button btnStart;
    public Button btnBack;
    public Text txtWeaponName;
    public Text txtAtk;
    public Text txtInternal;
    public Text txtMode;
    public Text txtType;
    public Text txtRange;
    public Image imgWeapon;
    private bool isInit=false;

    protected override void Init()
    {
        if (isInit)
            return;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "weapon", (atlas) =>{
            for (int i = 0; i < togWeaponList.Count; i++)
            {
                Image image = togWeaponList[i].transform .Find("weapon").GetComponent<Image>();
                image.sprite = atlas.GetSprite(weaponDatas[i].resName);
                image.preserveAspect = true;
            }
        });
        btnBack.onClick.AddListener(() =>{
            UIManager.Instance.HidePanel<WeaponSelectPanel>();
            //显示选人界面
            UIManager.Instance.ShowPanel<RoleSelectPanel>();
        });

        btnStart.onClick.AddListener(() =>{
            UIManager.Instance.HidePanel<WeaponSelectPanel>();
            ScenesManager.Instance.LoadSceneAsync("GameScene", () =>{
                //进入游戏场景后需要处理的事件
            });
        });
        togWeaponList[0].onValueChanged.AddListener((value) =>{
            if (value)
            {

                ChangeSelectWeapon(weaponDatas[0]);
            }
        });
        togWeaponList[1].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ChangeSelectWeapon(weaponDatas[1]);
            }
        });
        togWeaponList[2].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ChangeSelectWeapon(weaponDatas[2]);
            }
        });
        togWeaponList[3].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ChangeSelectWeapon(weaponDatas[3]);
            }
        });
        togWeaponList[4].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ChangeSelectWeapon(weaponDatas[4]);
            }
        });
        togWeaponList[5].onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ChangeSelectWeapon(weaponDatas[5]);
            }
        });
        isInit = true;
        txtAtk.text = string.Empty;
        txtInternal.text = string.Empty;
        txtMode.text = string.Empty;
        txtRange.text = string.Empty;
        txtType.text = string.Empty;
        txtWeaponName.text = string.Empty;
        imgWeapon.enabled = false;
    }

    private void ChangeSelectWeapon(WeaponData data)
    {
        imgWeapon.enabled = true;
        txtAtk.text = data.atk.ToString();
        txtInternal.text=data.internalTime.ToString();
        txtMode.text = data.Tip;
        txtRange.text = data.range.ToString();
        txtWeaponName.text = data.Name+" I";
        switch (data.damageType)
        {
            case DamageType.AST:txtType.text = "魔法";
                break;
            case DamageType.STR:txtType.text = "物理";
                break;
        }
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "weapon", (atlas) =>{
            imgWeapon.sprite = atlas.GetSprite(data.resName);
        });
    }

    public override void Hide(UnityAction action)
    {
        base.Hide(action);
        isInit = true;
        txtAtk.text = string.Empty;
        txtInternal.text = string.Empty;
        txtMode.text = string.Empty;
        txtRange.text = string.Empty;
        txtType.text = string.Empty;
        txtWeaponName.text = string.Empty;
        imgWeapon.sprite = null;
    }
}
