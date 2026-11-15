using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class LevelUpItem : MonoBehaviour
{
    public Image imgBK;
    public Image imgIcon;
    public Button btnSure;
    public Text txtTitle;
    public Text txtNum;
    public LevelUpData data;
    public void Init(LevelUpData data)
    {
        this.data = data;
        int level = data.level;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Components", (atlas) => {
            if (this == null) return;
            switch (level)
            {
                case 1:
                    imgBK.sprite = atlas.GetSprite("shop_card_accessory_common");
                    break;
                case 2:
                    imgBK.sprite = atlas.GetSprite("shop_card_accessory_rare");
                    break;
                case 3:
                    imgBK.sprite = atlas.GetSprite("shop_card_accessory_epic");
                    break;
                case 4:
                    imgBK.sprite = atlas.GetSprite("shop_card_accessory_legendary");
                    break;
            }
        });
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Stats", (atlas) =>{
            if (this == null) return;
            imgIcon.sprite = atlas.GetSprite(data.resName);
        });
        switch (data.statsName)
        {
            case "damage": txtTitle.text = "力量（增伤）"; break;
            case "STR": txtTitle.text = "物理伤害"; break;
            case "AST": txtTitle.text = "魔法伤害"; break;
            case "atkSpeed": txtTitle.text = "攻击速度"; break;
            case "range": txtTitle.text = "攻击范围"; break;
            case "moveSpeed": txtTitle.text = "移动速度"; break;
            case "hp": txtTitle.text = "生命上限"; break;
            case "armor": txtTitle.text = "护甲"; break;
            case "lucky": txtTitle.text = "幸运"; break;
            case "drain": txtTitle.text = "吸血"; break;
            case "recovery": txtTitle.text = "生命恢复"; break;
            case "money": txtTitle.text = "金币"; break;
        }
        btnSure.onClick.RemoveListener(Choose);
        btnSure.onClick.AddListener(Choose);
        btnSure.interactable = true;
        float value = LevelUpData.GetValue(data.statsName, data.num);
        switch (data.statsName)
        {
            case "damage":
            case "atkSpeed":
            case "range":
            case "moveSpeed":
                txtNum.text = value.ToString("+0.#%;-0.#%;+0%", CultureInfo.InvariantCulture);
                break;
            case "drain":
                txtNum.text = value.ToString("+0.#%;-0.#%;+0%", CultureInfo.InvariantCulture);
                break;
            case "STR":
            case "AST":
                txtNum.text = value.ToString("+0;-0;+0", CultureInfo.InvariantCulture);
                break;
            case "recovery":
                txtNum.text = value.ToString("+0;-0;+0", CultureInfo.InvariantCulture) + " / 秒";
                break;
            default:
                txtNum.text = value.ToString("+0;-0;+0", CultureInfo.InvariantCulture);
                break;
        }
    }

    private void Choose()
    {
        if (!LevelUpManager.Instance.Select(this)) return;

        UIManager.Instance.GetPanel<ShopPanel>(panel => panel.updateTXTDic());
        LevelUpManager.Instance.selectCount--;

        if (LevelUpManager.Instance.selectCount <= 0)
        {
            UIManager.Instance.HidePanel<LevelUpPanel>(() =>
            {
                UIManager.Instance.ShowPanel<ShopPanel>();
            });
            return;
        }

        UIManager.Instance.GetPanel<LevelUpPanel>(panel => panel.Refresh());
    }
}
