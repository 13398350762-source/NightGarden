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
    public ToggleGroup toggleGroup;
    private bool isInit=false;
    internal bool isReady = false;
    protected override void Init()
    {
        if (isInit)
            return;
        toggleGroup.allowSwitchOff = true;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "weapon", (atlas) =>{
            if (this == null || atlas == null) return;
            for (int i = 0; i < togWeaponList.Count; i++)
            {
                Image image = togWeaponList[i].transform .Find("weapon").GetComponent<Image>();
                image.sprite = atlas.GetSprite(weaponDatas[i].resName);
                image.preserveAspect = true;
            }
        });
        EventCallBack.Instance.Bind(btnBack, nameof(EventCallBack.BackToRole));

        EventCallBack.Instance.Bind(btnStart, nameof(EventCallBack.StartGame), this);
        for (int i = 0; i < togWeaponList.Count; i++)
        {
            EventCallBack.Instance.BindSelection(togWeaponList[i], nameof(EventCallBack.SelectWeapon), this, i);
        }
        isInit = true;
        toggleGroup.SetAllTogglesOff();
    }

    internal void ChangeSelectWeapon(WeaponData data)
    {
        isReady = true;
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
            if (this == null || atlas == null) return;
            imgWeapon.sprite = atlas.GetSprite(data.resName);
        });
        PlayerInfoManager.Instance.SetInitialWeapon(data);
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
        imgWeapon.enabled = false;
        imgWeapon.sprite = null;
        toggleGroup.SetAllTogglesOff();
    }

    public override void Show()
    {
        base.Show();
        isReady = false;
        txtAtk.text = string.Empty;
        txtInternal.text = string.Empty;
        txtMode.text = string.Empty;
        txtRange.text = string.Empty;
        txtType.text = string.Empty;
        txtWeaponName.text = string.Empty;
        imgWeapon.enabled = false;
    }
}
