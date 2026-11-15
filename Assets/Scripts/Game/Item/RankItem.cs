using System.Globalization;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class RankItem : MonoBehaviour
{
    public Image imgBK;
    public Image imgRole;
    public Text txtRank;
    public Text txtPlayer;
    public Text txtRole;
    public Text txtScore;
    public Text txtWave;
    public Text txtTime;
    public Text txtBest;
    public RankData data;

    public void Init(RankData data)
    {
        this.data = data;
        txtRank.text = data.rank.ToString();
        txtPlayer.text = data.playerName;
        txtRole.text = data.roleName;
        txtScore.text = data.score.ToString("N0", CultureInfo.InvariantCulture);
        txtWave.text = data.wave.ToString();
        int seconds = Mathf.FloorToInt(Mathf.Max(0f, data.remain));
        txtTime.text = $"{seconds / 60:00}:{seconds % 60:00}";
        txtBest.text = "个人最佳";
        txtBest.gameObject.SetActive(data.isBest);
        txtScore.color = data.isBest ? new Color32(239, 179, 68, 255) : new Color32(231, 225, 185, 255);
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Components", (atlas) => {
            if (this == null || this.data != data) return;
            string name = data.isBest ? "rank_row_best" : "rank_row_normal";
            imgBK.sprite = atlas.GetSprite(name);
        });
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Role", (atlas) => {
            if (this == null || this.data != data) return;
            imgRole.sprite = atlas.GetSprite(data.resName);
            imgRole.preserveAspect = true;
        });
    }
}
