using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankPanel : BasePanel
{
    public Button btnReturn;
    public ScrollRect svRank;
    private bool isInit = false;
    protected override void Init()
    {
        if (isInit)
            return;
        btnReturn.onClick.AddListener(() =>{
            UIManager.Instance.HidePanel<RankPanel>();
            UIManager.Instance.ShowPanel<MainPanel>();
        });
        isInit = true;
    }

    public override void Show()
    {
        base.Show();
        //加载记录
    }
}
