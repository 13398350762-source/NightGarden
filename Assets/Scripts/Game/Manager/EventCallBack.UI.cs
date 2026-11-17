using UnityEngine;
using UnityEngine.U2D;

// 面板只负责显示；按钮业务统一从 EventCenter 到这里执行。
public partial class EventCallBack
{
    public void Login(LoginPanel view)
    {
        if (isChangingScene) return;
        if (view.inputUN.text == string.Empty)
        {
            UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                panel.ChangeInfo("用户名不能为空");
            });
            return;
        }
        if (view.inputPW.text == string.Empty)
        {
            UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                panel.ChangeInfo("密码不能为空");
            });
            return;
        }
        if (DataManager.Instance.CheckLogin(view.inputUN.text, view.inputPW.text))
        {
            UserInfo user = DataManager.Instance.GetUser(view.inputUN.text);
            UserManager.Instance.SetNowUser(user);
            LoadScene("BeginScene", () => {
                UIManager.Instance.HidePanel<LoginPanel>();
                UIManager.Instance.ShowPanel<MainPanel>();
            });
        }
        else
        {
            UIManager.Instance.ShowPanel<TipPanel>((panel) => {
                panel.ChangeInfo("密码不正确");
            });
        }
    }

    public void Register(RegisterPanel view)
    {
        if (view.inputUN.text == string.Empty)
        {
            UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                panel.ChangeInfo("用户名不能为空");
            });
            return;
        }
        if (view.inputPW.text == string.Empty)
        {
            UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                panel.ChangeInfo("密码不能为空");
            });
            view.inputSU.text= string.Empty;
            return;
        }
        if (view.inputSU.text != view.inputPW.text)
        {
            UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                panel.ChangeInfo("第二次输入密码与第一次不同");
            });
            view.inputSU.text = string.Empty;
            view.inputPW.text = string.Empty;
            return;
        }
        if (DataManager.Instance.CheckUser(view.inputUN.text))
        {
            UIManager.Instance.ShowPanel<TipPanel>((panel) => {
                panel.ChangeInfo("用户名已存在");
            });
            view.inputSU.text = string.Empty;
            view.inputPW.text = string.Empty;
            view.inputUN.text = string.Empty;
            return;
        }
        //显示提示面板
        DataManager.Instance.AddUser(new UserInfo(view.inputUN.text,view.inputPW.text));
        UIManager.Instance.HidePanel<RegisterPanel>();
        UIManager.Instance.ShowPanel<LoginPanel>();
        UIManager.Instance.ShowPanel<TipPanel>((panel) => {
            panel.ChangeInfo("注册成功");
        });
    }

    public void ResetSettings(SettingPanel view)
    {
        AudioManager.Instance.SetDefault();
        view.RefreshAudioUI();
    }

    public void ChangeMainVolume(EventValue<float> request)
    {
        var view = request.sender as SettingPanel;
        if (view == null) return;
        float value = request.value;
        AudioManager.Instance.SetMainVolume(value);
        view.txtMain.text = (value * 100).ToString("F2") + "%";
    }

    public void ChangeMusicVolume(EventValue<float> request)
    {
        var view = request.sender as SettingPanel;
        if (view == null) return;
        float value = request.value;
        AudioManager.Instance.SetMusicVolume(value);
        view.txtMusic.text = (value * 100).ToString("F2") + "%";
    }

    public void ChangeSoundVolume(EventValue<float> request)
    {
        var view = request.sender as SettingPanel;
        if (view == null) return;
        float value = request.value;
        AudioManager.Instance.SetSoundVolume(value);
        view.txtSound.text = (value * 100).ToString("F2") + "%";
    }

    public void ConfirmRole(RoleSelectPanel view)
    {
        //如果有角色被选中
        if (PlayerInfoManager.Instance.GetRole() == null)
        {
            UIManager.Instance.ShowPanel<TipPanel>((panel) =>{
                panel.ChangeInfo("请选择角色");
            });
            return;
        }
        if (PlayerInfoManager.Instance.GetRole() == view.roleList[3] && !UserManager.Instance.IsChiliUnlock())
        {
            UIManager.Instance.ShowPanel<TipPanel>((panel) => {
                panel.ChangeInfo("角色未解锁");
            });
            return;
        }
        if (PlayerInfoManager.Instance.GetRole() == view.roleList[4] && !UserManager.Instance.IsPeaUnlock())
        {
            UIManager.Instance.ShowPanel<TipPanel>((panel) => {
                panel.ChangeInfo("角色未解锁");
            });
            return;
        }
        UIManager.Instance.ShowPanel<WeaponSelectPanel>();
        UIManager.Instance.HidePanel<RoleSelectPanel>();
    }

    public void StartGame(WeaponSelectPanel view)
    {
        if (isChangingScene) return;
        if (!view.isReady)
        {
            UIManager.Instance.ShowPanel<TipPanel>((panel) => {
                panel.ChangeInfo("请选择武器");
            });
            return;
        }
        LoadScene("GameScene", () => {
            UIManager.Instance.HidePanel<WeaponSelectPanel>();
            InitPlayer();
            UIManager.Instance.ShowPanel<GamePanel>((panel) => {
                string name= PlayerInfoManager.Instance.GetRole().roleID;
                panel.txtRoleName.text = name;
                ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas","Role",(atlas) =>{
                    panel.imgRole.sprite=atlas.GetSprite(name);
                });
            });
        });
    }

    public void RefreshShop(ShopPanel view)
    {
        int price = ShopManager.Instance.GetRefreshPrice();
        if (PlayerInfoManager.Instance.GetMoney() < price)
            return;

        PlayerInfoManager.Instance.AddMoney(-price);
        ShopManager.Instance.refreshCount++;
        view.Refresh();
    }

    public void RefreshUpgrades(LevelUpPanel view)
    {
        if (!view.isChoosing) return;
        int price = LevelUpManager.Instance.GetRefreshPrice();
        if (PlayerInfoManager.Instance.GetMoney() < price) return;
        // 先成功生成，再扣费，配置或资源错误时不消耗金币。
        PlayerInfoManager.Instance.AddMoney(-price);
        view.Refresh();
        LevelUpManager.Instance.refreshCount++;
        view.UpdateRefreshInfo();
        UIManager.Instance.GetPanel<ShopPanel>(panel => panel.updateTXTDic());
    }

}
