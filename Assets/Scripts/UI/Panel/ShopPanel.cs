using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : BasePanel
{
    private  Dictionary<string,Text> txtDic=new Dictionary<string, Text>();
    public Text txtMoney;
    public Transform txtParent;
    public ScrollRect svAccessories;
    public ScrollRect svWeapons;
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
        isInit = true;
    }
}
