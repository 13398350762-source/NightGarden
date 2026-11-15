using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class WeaponShopItem : BaseShopItem<WeaponData>
{
    public Text txtAtk;
    public Text txtInternal;
    public Text txtType;
    public WeaponData data;
    public override void Init(WeaponData data,int money)
    {
        this.data = data;
        txtAtk.text = data.atk.ToString();
        txtInternal.text = data.internalTime.ToString();
        txtName.text = data.Name;
        switch (data.damageType)
        {
            case DamageType.AST:txtType.text = "魔法";
                break;
            case DamageType.STR:txtType.text = "物理";
                break;
        }
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "weapon", (atlas) => {
            if (this == null) return;
            imgIcon.sprite = atlas.GetSprite(data.resName); ;
        });
        int level = data.level;
        txtTitle.text = "武器 · " + (level == 4 ? "IV" : new string('I', level)) + "级";
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Components", (atlas) =>{
            if (this == null) return;
            switch (level)
            {
                case 1:
                    imgbk.sprite = atlas.GetSprite("shop_card_accessory_common");
                    imgTitle.sprite = atlas.GetSprite("shop_card_header_common");
                    break;
                case 2:
                    imgbk.sprite = atlas.GetSprite("shop_card_accessory_rare");
                    imgTitle.sprite = atlas.GetSprite("shop_card_header_rare");
                    break;
                case 3:
                    imgbk.sprite = atlas.GetSprite("shop_card_accessory_epic");
                    imgTitle.sprite = atlas.GetSprite("shop_card_header_epic");
                    break;
                case 4:
                    imgbk.sprite = atlas.GetSprite("shop_card_accessory_legendary");
                    imgTitle.sprite = atlas.GetSprite("shop_card_header_legendary");
                    break;
            }
        });
        btnBuy.onClick.AddListener(() =>{
            if (!btnBuy.interactable) return;
            //花钱买商品
            if (PlayerInfoManager.Instance.GetMoney() >= cost && PlayerInfoManager.Instance.SetWeapon(data))
            {
                PlayerInfoManager.Instance.AddMoney(-cost);
                RemoveFromShop();
                UIManager.Instance.GetPanel<ShopPanel>((panel) =>{
                    panel.RefreshWeapons();
                    panel.updateTXTDic();
                });
            }
            else
            {
                //播放购买失败音效
            }
        });
        txtMoney.text = "购买 " + money.ToString();
        cost = money;
    }
}
