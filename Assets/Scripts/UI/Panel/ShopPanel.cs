using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ShopPanel : BasePanel
{
    private  Dictionary<string,Text> txtDic=new Dictionary<string, Text>();
    public Text txtMoney;
    public Transform txtParent;
    public ScrollRect svAccessories;
    public ScrollRect svWeapons;
    public ScrollRect svShop;
    public Button btnRefresh;
    public Button btnNext;
    public Image imgRole;
    public Text txtName;
    public Text txtLevel;
    private bool isInit = false;
    protected override void Init()
    {
        if (isInit)
            return;
        txtDic.Add("money", txtMoney);
        foreach (Transform child in txtParent)
        {
            string key = child.name == "txtatkSpeed" ? "atkSpeed" : child.name;
            txtDic.Add(key, child.GetComponent<Text>());
        }
        btnNext.onClick.AddListener(() =>{
            UIManager.Instance.HidePanel<ShopPanel>();
        });
        btnRefresh.onClick.AddListener(() =>
        {
            int price = ShopManager.Instance.GetRefreshPrice();
            if (PlayerInfoManager.Instance.GetMoney() < price)
                return;

            PlayerInfoManager.Instance.AddMoney(-price);
            ShopManager.Instance.refreshCount++;
            Refresh();
        });
        svShop.vertical = false;
        svShop.horizontal = false;
        svWeapons.vertical = false;
        svWeapons.horizontal = false;
        isInit = true;
    }

    public override void Show()
    {
        base.Show();
        ShopManager.Instance.refreshCount = 0;
        Refresh();
        foreach (Transform child in svAccessories.content)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        foreach (Transform child in svWeapons.content)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        foreach (AccessoryData data in PlayerInfoManager.Instance.accessoryList)
        {
            GameObject obj=ResManager.Instance.LoadRes<GameObject>("item", "ItemAccessory");
            ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Accessory", (atlas) =>{
                if (obj == null) return;
                obj.GetComponent<Image>().sprite = atlas.GetSprite(data.resIcon);
            });
            obj.transform.SetParent(svAccessories.content, false);
        }
        foreach (WeaponData data in PlayerInfoManager.Instance.weaponList)
        {
            if (data == null)
                break;
            GameObject obj = ResManager.Instance.LoadRes<GameObject>("item", "ItemWeapon");
            int level = data.level;
            obj.transform.GetChild(0).GetComponent<Text>().text = level == 4 ? "IV" : new string('I', level);
            ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Weapon", (atlas) => {
                if (obj == null) return;
                obj.transform.GetChild(1).GetComponent<Image>().sprite= atlas.GetSprite(data.resName);
            });
            ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Components", (atlas) => {
                if (obj == null) return;
                string name = "weapon_frame_" + level.ToString();
                obj.GetComponent<Image>().sprite = atlas.GetSprite(name);
            });
            obj.transform.SetParent(svWeapons.content, false);
        }
        //角色面板显示（未完成）
    }
    public void Refresh()
    {
        ShopManager.Instance.Refresh();
        foreach (GameObject obj in ShopManager.Instance.items)
        {
            obj.transform.SetParent(svShop.content, false);
        }
        updateTXTDic();
    }

    public void updateTXTDic()
    {
        PlayerInfoManager player = PlayerInfoManager.Instance;
        txtDic["money"].text = player.GetMoney().ToString(CultureInfo.InvariantCulture);
        txtDic["damage"].text = FormatMultiplier(player.GetDamage());
        txtDic["STR"].text = player.GetSTR().ToString("+0;-0;0", CultureInfo.InvariantCulture);
        txtDic["AST"].text = player.GetAST().ToString("+0;-0;0", CultureInfo.InvariantCulture);
        txtDic["atkSpeed"].text = FormatMultiplier(player.GetAtkSpeed());
        txtDic["range"].text = FormatMultiplier(player.GetRange());
        txtDic["moveSpeed"].text = FormatMultiplier(player.GetMoveSpeedMultiplier());
        txtDic["hp"].text = player.GetHp().ToString(CultureInfo.InvariantCulture);
        txtDic["armor"].text = player.GetArmor().ToString(CultureInfo.InvariantCulture);
        txtDic["lucky"].text = player.GetLucky().ToString(CultureInfo.InvariantCulture);
        txtDic["drain"].text = player.GetDrain().ToString("0.#%", CultureInfo.InvariantCulture);
        txtDic["recovery"].text = player.GetRecovery().ToString(CultureInfo.InvariantCulture) + " / 秒";
        btnRefresh.transform.GetChild(0).GetComponent<Text>().text = "刷新 " + ShopManager.Instance.GetRefreshPrice().ToString();
        txtMoney.text=PlayerInfoManager.Instance.GetMoney().ToString();
    }

    private static string FormatMultiplier(float multiplier)
    {
        // 保留一位小数，先取整以免浮点误差显示成 -0%。
        float percent = Mathf.Round((multiplier - 1f) * 1000f) / 10f;
        return percent.ToString("+0.#;-0.#;0", CultureInfo.InvariantCulture) + "%";
    }
}
