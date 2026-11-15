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
    private bool isChoosing;
    protected override void Init()
    {
        if (isInit) return;
        btnRefresh.onClick.AddListener(() => {
            if (!isChoosing) return;
            int price = LevelUpManager.Instance.GetRefreshPrice();
            if (PlayerInfoManager.Instance.GetMoney() < price) return;
            // 先成功生成，再扣费，配置或资源错误时不消耗金币。
            PlayerInfoManager.Instance.AddMoney(-price);
            Refresh();
            LevelUpManager.Instance.refreshCount++;
            UpdateRefreshInfo();
            UIManager.Instance.GetPanel<ShopPanel>(panel => panel.updateTXTDic());
        });
        svOptions.vertical = false;
        svOptions.horizontal = false;
        isInit = true;
    }

    public override void Show()
    {
        base.Show();
        isChoosing = true;
        LevelUpManager.Instance.refreshCount = 0;
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

    private void UpdateRefreshInfo()
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
