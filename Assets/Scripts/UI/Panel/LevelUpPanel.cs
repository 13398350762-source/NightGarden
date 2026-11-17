using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelUpPanel : BasePanel
{
    public Button btnRefresh;
    public ScrollRect svOptions;
    public Text txtMoney;
    private bool isInit;
    internal bool isChoosing;
    protected override void Init()
    {
        if (isInit) return;
        EventCallBack.Instance.Bind(btnRefresh, nameof(EventCallBack.RefreshUpgrades), this);
        svOptions.vertical = false;
        svOptions.horizontal = false;
        isInit = true;
    }

    public override void Show()
    {
        if (LevelUpManager.Instance.selectCount == 0)
        {
            UIManager.Instance.ShowPanel<ShopPanel>();
            return;
        }
        base.Show();
        isChoosing = true;
        LevelUpManager.Instance.refreshCount = 0;
        LevelUpManager.Instance.selectCount--;
        Refresh();
    }

    public void Refresh()
    {
        if (!isChoosing) return;
        LevelUpManager.Instance.Refresh();
        foreach (GameObject obj in LevelUpManager.Instance.items)
            obj.transform.SetParent(svOptions.content, false);
        txtMoney.text = PlayerInfoManager.Instance.GetMoney().ToString();
        UpdateRefreshInfo();
    }

    internal void UpdateRefreshInfo()
    {
        int price = LevelUpManager.Instance.GetRefreshPrice();
        btnRefresh.GetComponentInChildren<Text>(true).text = "刷新选项 " + price;
        btnRefresh.interactable = isChoosing && PlayerInfoManager.Instance.GetMoney() >= price;
    }

    public override void Hide(UnityAction action)
    {
        isChoosing = false;
        btnRefresh.interactable = false;
        LevelUpManager.Instance.Clear();
        base.Hide(action);
    }
}
