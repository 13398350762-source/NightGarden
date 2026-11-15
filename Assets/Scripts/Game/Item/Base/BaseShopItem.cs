using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseShopItem<T> : MonoBehaviour where T:ScriptableObject
{
    public Button btnBuy;
    public Toggle togLock;
    public Image imgIcon;
    public Image imgbk;
    public Image imgTitle;
    public Text txtTitle;
    public Text txtName;
    public Text txtMoney;
    public int cost;

    protected void RemoveFromShop()
    {
        int index = System.Array.IndexOf(ShopManager.Instance.items, gameObject);
        if (index >= 0)
            ShopManager.Instance.items[index] = null;
        btnBuy.interactable = false;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public abstract void Init(T data,int money);
}
