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
        btnReStart.onClick.AddListener(() =>{
            RoleData data = PlayerInfoManager.Instance.GetRole();
            PlayerInfoManager.Instance.Clear();
            PlayerInfoManager.Instance.SetRole(data);
            PlayerInfoManager.Instance.SetInitialWeapon(PlayerInfoManager.Instance.initialWeapon);
            GameManager.Instance.Clear();
            UIManager.Instance.HidePanel<GameOverPanel>();
            Time.timeScale = 1.0f;
            //重新加载游戏
            GameObject.Find("Player").GetComponent<PlayerController>().Init();
        });
        btnRank.onClick.AddListener(() =>{
            //时间恢复正常
            Time.timeScale = 1f;
            PoolManager.Instance.Clear();
            UIManager.Instance.Clear();
            PlayerInfoManager.Instance.Clear();
            ScenesManager.Instance.LoadSceneAsync("BeginScene", () => {
                UIManager.Instance.ShowPanel<RankPanel>();
            });
        });
        btnReturn.onClick.AddListener(() => {
            //时间恢复正常
            Time.timeScale = 1f;
            PoolManager.Instance.Clear();
            UIManager.Instance.Clear();
            PlayerInfoManager.Instance.Clear();
            ScenesManager.Instance.LoadSceneAsync("BeginScene", () => {
                UIManager.Instance.ShowPanel<MainPanel>();
            });
        });
        isInit = true;
    }
    public override void Show()
    {
        base.Show();
        //加载游戏人物图片

        //时间暂停
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
