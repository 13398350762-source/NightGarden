using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class AccessoryShopItem : BaseShopItem<AccessoryData>
{
    public Text txtDescripiton;
    internal AccessoryData data;
    public override void Init(AccessoryData data,int money)
    {

        this.data = data;
        txtName.text = data.accessoryID;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Accessory", (atlas) => {
            if (this == null) return;
            imgIcon.sprite = atlas.GetSprite(data.resIcon); ;
        });
        string level = data.level;
        txtTitle.text = "饰品 · " + level;
        txtDescripiton.text = data.description;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Components", (atlas) => {
            if (this == null) return;
            switch (level)
            {
                case "普通":
                    imgbk.sprite = atlas.GetSprite("shop_card_accessory_common");
                    imgTitle.sprite = atlas.GetSprite("shop_card_header_common");
                    break;
                case "稀有":
                    imgbk.sprite = atlas.GetSprite("shop_card_accessory_rare");
                    imgTitle.sprite = atlas.GetSprite("shop_card_header_rare");
                    break;
                case "史诗":
                    imgbk.sprite = atlas.GetSprite("shop_card_accessory_epic");
                    imgTitle.sprite = atlas.GetSprite("shop_card_header_epic");
                    break;
                case "传说":
                    imgbk.sprite = atlas.GetSprite("shop_card_accessory_legendary");
                    imgTitle.sprite = atlas.GetSprite("shop_card_header_legendary");
                    break;
            }
        });
        EventCallBack.Instance.Bind(btnBuy, nameof(EventCallBack.BuyAccessory), this);
        txtMoney.text = "购买 " + money.ToString();
        cost = money;
    }
}
