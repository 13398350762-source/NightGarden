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
        EventCallBack.Instance.Bind(btnReturn, nameof(EventCallBack.BackToMain), (BasePanel)this);
        isInit = true;
    }

    public override void Show()
    {
        base.Show();

        Transform content = svRank.content;
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            GameObject oldItem = content.GetChild(i).gameObject;
            oldItem.SetActive(false);
            Destroy(oldItem);
        }

        foreach (RankData data in DataManager.Instance.dataList)
        {
            GameObject obj = ResManager.Instance.LoadRes<GameObject>("item", "RankItem");
            if (obj == null)
                break;

            obj.transform.SetParent(content, false);
            obj.GetComponent<RankItem>().Init(data);
        }
    }
}
