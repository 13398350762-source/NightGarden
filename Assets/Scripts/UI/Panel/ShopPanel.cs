using System.Collections;
using System.Collections.Generic;
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
            txtDic.Add(child.gameObject.name,child.GetComponent<Text>());
        }
        btnNext.onClick.AddListener(() =>{
            UIManager.Instance.HidePanel<ShopPanel>();
        });
        btnRefresh.onClick.AddListener(() =>
        {
            ShopManager.Instance.Refresh();
            foreach (GameObject obj in ShopManager.Instance.items)
            {
                obj.transform.SetParent(svShop.content, false);
            }
        });
        isInit = true;
    }

    public override void Show()
    {
        base.Show();
        ShopManager.Instance.Refresh();
        foreach(GameObject obj in ShopManager.Instance.items)
        {
            obj.transform.SetParent(svShop.content, false);
        }
        foreach (AccessoryData data in PlayerInfoManager.Instance.accessoryList)
        {
            GameObject obj=ResManager.Instance.LoadRes<GameObject>("item", "ItemAccessory");
            ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Accessory", (atlas) =>{
                obj.GetComponent<Image>().sprite = atlas.GetSprite(data.resIcon);
            });
        }
    }
    public override void Hide(UnityAction action)
    {
        base.Hide(action);
    }

}
