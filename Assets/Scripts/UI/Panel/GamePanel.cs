using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    public Image imgRole;
    public Image imgHp;
    public Image imgExp;
    public Text txtRoleName;
    public Text txtLevel;
    public Text txtHp;
    public Text txtExp;
    public Text txtWave;
    public Text txtTime;
    public Text txtMoney;
    public Button btnStop;
    private bool isInit = false;
    public override void Show()
    {
        base.Show();
        DamageNumber.NumberFont = txtHp.font;
        txtWave.text = "第" + GameManager.Instance.Wave + "波";
        Camera.main.transform.SetParent(PlayerInfoManager.Instance.playerPos,false);
    }
    protected override void Init()
    {
        if (isInit)
            return;
        //读取人物图片
        RoleData data = PlayerInfoManager.Instance.GetRole();
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Role", (atlas) => {
            imgRole.sprite = atlas.GetSprite(data.roleID);
        });
        txtRoleName.text=data.roleID;
        EventCallBack.Instance.Bind(btnStop, nameof(EventCallBack.PauseGame));
        isInit = true;
    }

    protected override void Update()
    {
        base.Update();
        PlayerInfoManager player = PlayerInfoManager.Instance;
        RoleData role = player.GetRole();
        txtRoleName.text = role != null ? role.roleID : string.Empty;
        txtLevel.text = "Lv. " + player.GetLevel();
        txtMoney.text = player.GetMoney().ToString();

        int maxHp = Mathf.Max(0, player.GetHp());
        int nowHp = Mathf.Clamp(player.nowHp, 0, maxHp);
        imgHp.fillAmount = maxHp > 0 ? (float)nowHp / maxHp : 0f;
        txtHp.text = $"{nowHp} / {maxHp}";

        int nowExp = Mathf.Max(0, player.nowExp);
        int nextLevelExp = Mathf.Max(0, player.nextLevelExp);
        imgExp.fillAmount = nextLevelExp > 0 ? Mathf.Clamp01((float)nowExp / nextLevelExp) : 0f;
        txtExp.text = $"{nowExp} / {nextLevelExp}";

        int seconds = Mathf.CeilToInt(Mathf.Max(0f, GameManager.Instance.nowWaveRemainTime));
        txtTime.text = $"{seconds / 60:00}:{seconds % 60:00}";
    }
}
