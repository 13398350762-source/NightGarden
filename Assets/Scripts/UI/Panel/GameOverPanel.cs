using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GameOverPanel : BasePanel
{
    public Image imgIcon;
    public Text txtName;
    public Text txtScore;
    public Text txtRemain;
    public Text txtDeath;
    public Text txtCollection;
    public Text txtWave;
    public Button btnReStart;
    public Button btnRank;
    public Button btnReturn;
    private bool isInit = false;
    protected override void Init()
    {
        if (isInit)
            return;
        EventCallBack.Instance.Bind(btnReStart, nameof(EventCallBack.RestartGame));
        EventCallBack.Instance.Bind(btnRank, nameof(EventCallBack.ReturnToRank));
        EventCallBack.Instance.Bind(btnReturn, nameof(EventCallBack.ReturnToMain));
        isInit = true;
    }
    public override void Show()
    {
        base.Show();
        //加载游戏人物图片
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Role", (atlas) => {
             imgIcon.sprite = atlas.GetSprite(name);
        });
        Time.timeScale = 0f;
        txtName.text = PlayerInfoManager.Instance.GetRole().roleID;
        txtScore.text = GameManager.Instance.Score.ToString();
        txtWave.text = GameManager.Instance.Wave.ToString();
        txtDeath.text = GameManager.Instance.Death.ToString();
        txtCollection.text = GameManager.Instance.Collection.ToString();
        int totalSeconds = Mathf.FloorToInt(Mathf.Max(0f, GameManager.Instance.Remain));
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        txtRemain.text = $"{minutes:00}:{seconds:00}";
    }

}
