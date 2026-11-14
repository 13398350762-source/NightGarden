using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class AccessoryShopItem : BaseShopItem<AccessoryData>
{
    public Text txtDescripiton;
    public override void Init(AccessoryData data,int money)
    {
        txtName.text = data.accessoryID;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "weapon", (atlas) => {
            imgIcon.sprite = atlas.GetSprite(data.resIcon); ;
        });
        string level = data.level;
        txtTitle.text = "武器 · " + level;
        txtDescripiton.text = data.description;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Componets", (atlas) => {
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
        btnBuy.onClick.AddListener(() => {
            //花钱买商品
        });
        txtMoney.text = "购买 " + money.ToString();
        cost = money;
    }
}
