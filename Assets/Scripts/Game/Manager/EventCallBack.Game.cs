using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public partial class EventCallBack
{
    public void RestartGame()
    {
        if (isChangingScene) return;
        PlayerInfoManager player = PlayerInfoManager.Instance;
        RoleData role = player.GetRole();
        WeaponData weapon = player.initialWeapon;
        if (role == null) return;
        player.Clear();
        player.SetRole(role);
        player.SetInitialWeapon(weapon);
        GameManager.Instance.Clear();
        Time.timeScale = 1f;
        UIManager.Instance.HidePanel<PausePanel>();
        UIManager.Instance.HidePanel<GameOverPanel>();
        InitPlayer();
    }

    private void InitPlayer()
    {
        PlayerController player = Object.FindObjectOfType<PlayerController>();
        if (player != null)
            player.Init();
        else
        {
            int version = sceneVersion;
            ResManager.Instance.LoadResAsync<GameObject>("player", "Player", obj => {
                if (obj == null) return;
                if (version != sceneVersion) { Object.Destroy(obj); return; }
                if (obj != null) obj.GetComponent<PlayerController>().Init();
            });
        }
    }

    public void ReturnToMain() { ReturnToMenu(false); }
    public void ReturnToRank() { ReturnToMenu(true); }

    private void ReturnToMenu(bool rank)
    {
        if (isChangingScene) return;
        Time.timeScale = 1f;
        settingsOwner = null;
        isOpeningWeaponPanel = false;
        GameManager.Instance.Clear();
        UIManager.Instance.Clear();
        PoolManager.Instance.Clear();
        PlayerInfoManager.Instance.Clear();
        LoadScene("BeginScene", () => {
            if (rank) UIManager.Instance.ShowPanel<RankPanel>();
            else UIManager.Instance.ShowPanel<MainPanel>();
        });
    }

    private void LoadScene(string scene, UnityAction callback)
    {
        if (isChangingScene) return;
        isChangingScene = true;
        sceneVersion++;
        ScenesManager.Instance.LoadSceneAsync(scene, () => {
            isChangingScene = false;
            callback?.Invoke();
        });
    }

    public void PauseGame() { UIManager.Instance.ShowPanel<PausePanel>(); }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        UIManager.Instance.HidePanel<PausePanel>();
    }

    public void MovePlayer(EventValue<Vector2> request)
    {
        PlayerController player = request.sender as PlayerController;
        if (player != null) player.moveDir = request.value.normalized;
    }

    public void EnterRoleSelect()
    {
        UIManager.Instance.ShowPanel<RoleSelectPanel>();
        UIManager.Instance.HidePanel<MainPanel>();
    }

    public void BackToMain(BasePanel panel)
    {
        if (panel is RoleSelectPanel) UIManager.Instance.HidePanel<RoleSelectPanel>();
        else if (panel is RankPanel) UIManager.Instance.HidePanel<RankPanel>();
        UIManager.Instance.ShowPanel<MainPanel>();
    }

    public void OpenRank()
    {
        UIManager.Instance.ShowPanel<RankPanel>();
        UIManager.Instance.HidePanel<MainPanel>();
    }

    public void OpenSettings(BasePanel owner)
    {
        settingsOwner = owner;
        UIManager.Instance.ShowPanel<SettingPanel>();
        if (owner is MainPanel) UIManager.Instance.HidePanel<MainPanel>();
    }

    public void CancelSettings()
    {
        AudioManager.Instance.LoadAudioData();
        UIManager.Instance.HidePanel<SettingPanel>();
        if (settingsOwner is MainPanel) UIManager.Instance.ShowPanel<MainPanel>();
        settingsOwner = null;
    }

    public void SaveSettings() { AudioManager.Instance.SaveAudioData(); }
    // 原控件回调尚未实现的设置项，保留事件入口。
    public void ChangeDamageDisplay(bool value) { }
    public void ChangeCameraShake(bool value) { }
    public void ChangeWindowMode(int value) { }
    public void ChangeResolution(int value) { }

    public void Logout()
    {
        if (isChangingScene) return;
        UIManager.Instance.HidePanel<MainPanel>();
        LoadScene("LoginScene", () => UIManager.Instance.ShowPanel<LoginPanel>());
    }

    public void OpenRegister()
    {
        UIManager.Instance.HidePanel<LoginPanel>();
        UIManager.Instance.ShowPanel<RegisterPanel>();
    }

    public void BackToLogin()
    {
        UIManager.Instance.HidePanel<RegisterPanel>();
        UIManager.Instance.ShowPanel<LoginPanel>();
    }

    public void BackToRole()
    {
        UIManager.Instance.HidePanel<WeaponSelectPanel>();
        UIManager.Instance.ShowPanel<RoleSelectPanel>();
        System.Array.Clear(PlayerInfoManager.Instance.weaponList, 0, PlayerInfoManager.Instance.weaponList.Length);
        PlayerInfoManager.Instance.initialWeapon = null;
    }

    public void SelectRole(EventValue<int> request)
    {
        RoleSelectPanel panel = request.sender as RoleSelectPanel;
        if (panel != null && request.value >= 0 && request.value < panel.roleList.Count)
            panel.ChangeRoleInfo(panel.roleList[request.value]);
    }

    public void SelectWeapon(EventValue<int> request)
    {
        WeaponSelectPanel panel = request.sender as WeaponSelectPanel;
        if (panel == null || request.value < 0 || request.value >= panel.weaponDatas.Count) return;
        System.Array.Clear(PlayerInfoManager.Instance.weaponList, 0, PlayerInfoManager.Instance.weaponList.Length);
        panel.ChangeSelectWeapon(panel.weaponDatas[request.value]);
    }

    public void NextWave()
    {
        if (GameManager.Instance.IsRunning) return;
        UIManager.Instance.HidePanel<ShopPanel>();
        PlayerController player = Object.FindObjectOfType<PlayerController>();
        if (player != null) player.RefreshWeapon();
        GameManager.Instance.StartNextWave();
        UIManager.Instance.ShowPanel<GamePanel>();
    }
    public void CloseTip() { UIManager.Instance.HidePanel<TipPanel>(); }
    public void QuitGame() { Application.Quit(); }
}
