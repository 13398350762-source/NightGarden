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

    public abstract void Init(T data,int money);
}
