using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class EventCallBack : BaseManager<EventCallBack>
{
    private int eventVersion = -1;
    private bool isOpeningWeaponPanel;
    private bool isChangingScene;
    private int sceneVersion;
    private BasePanel settingsOwner;

    public void Init()
    {
        EventCenter events = EventCenter.Instance;
        if (eventVersion == events.Version) return;
        events.AddListener(nameof(ContinueGame), ContinueGame);
        events.AddListener(nameof(RestartGame), RestartGame);
        events.AddListener<BasePanel>(nameof(OpenSettings), OpenSettings);
        events.AddListener(nameof(ReturnToMain), ReturnToMain);
        events.AddListener(nameof(ReturnToRank), ReturnToRank);
        events.AddListener(nameof(EnterRoleSelect), EnterRoleSelect);
        events.AddListener(nameof(QuitGame), QuitGame);
        events.AddListener(nameof(OpenRank), OpenRank);
        events.AddListener(nameof(Logout), Logout);
        events.AddListener<LoginPanel>(nameof(Login), Login);
        events.AddListener(nameof(OpenRegister), OpenRegister);
        events.AddListener(nameof(BackToLogin), BackToLogin);
        events.AddListener<RegisterPanel>(nameof(Register), Register);
        events.AddListener(nameof(CancelSettings), CancelSettings);
        events.AddListener<SettingPanel>(nameof(ResetSettings), ResetSettings);
        events.AddListener(nameof(SaveSettings), SaveSettings);
        events.AddListener<EventValue<float>>(nameof(ChangeMainVolume), ChangeMainVolume);
        events.AddListener<EventValue<float>>(nameof(ChangeMusicVolume), ChangeMusicVolume);
        events.AddListener<EventValue<float>>(nameof(ChangeSoundVolume), ChangeSoundVolume);
        events.AddListener<bool>(nameof(ChangeDamageDisplay), ChangeDamageDisplay);
        events.AddListener<bool>(nameof(ChangeCameraShake), ChangeCameraShake);
        events.AddListener<int>(nameof(ChangeWindowMode), ChangeWindowMode);
        events.AddListener<int>(nameof(ChangeResolution), ChangeResolution);
        events.AddListener<BasePanel>(nameof(BackToMain), BackToMain);
        events.AddListener<RoleSelectPanel>(nameof(ConfirmRole), ConfirmRole);
        events.AddListener<EventValue<int>>(nameof(SelectRole), SelectRole);
        events.AddListener(nameof(BackToRole), BackToRole);
        events.AddListener<WeaponSelectPanel>(nameof(StartGame), StartGame);
        events.AddListener<EventValue<int>>(nameof(SelectWeapon), SelectWeapon);
        events.AddListener(nameof(NextWave), NextWave);
        events.AddListener<ShopPanel>(nameof(RefreshShop), RefreshShop);
        events.AddListener<LevelUpPanel>(nameof(RefreshUpgrades), RefreshUpgrades);
        events.AddListener<WeaponItemPanel>(nameof(SellWeapon), SellWeapon);
        events.AddListener<WeaponItemPanel>(nameof(CompositeWeapon), CompositeWeapon);
        events.AddListener(nameof(PauseGame), PauseGame);
        events.AddListener(nameof(CloseTip), CloseTip);
        events.AddListener<AccessoryShopItem>(nameof(BuyAccessory), BuyAccessory);
        events.AddListener<WeaponShopItem>(nameof(BuyWeapon), BuyWeapon);
        events.AddListener<LevelUpItem>(nameof(ChooseUpgrade), ChooseUpgrade);
        events.AddListener<WeaponItem>(nameof(OpenWeapon), OpenWeapon);
        events.AddListener<EventValue<Vector2>>(nameof(MovePlayer), MovePlayer);
        eventVersion = events.Version;
    }

    public void Trigger(string name)
    {
        Init();
        EventCenter.Instance.EventTrigger(name);
    }

    public void Trigger<T>(string name, T value)
    {
        Init();
        EventCenter.Instance.EventTrigger(name, value);
    }

    public void Bind(Button button, string name)
    {
        Init();
        BindButton(button, () => Trigger(name));
    }

    public void Bind<T>(Button button, string name, T value)
    {
        Init();
        BindButton(button, () => Trigger(name, value));
    }

    private void BindButton(Button button, UnityAction action)
    {
        UnityEvent click = button.onClick;
        UnityAction listener = () => { if (button != null && button.isActiveAndEnabled && button.IsInteractable()) action(); };
        Binding(button).Replace(() => click.RemoveListener(listener));
        click.AddListener(listener);
    }

    public void BindSelection(Toggle toggle, string name, MonoBehaviour sender, int index)
    {
        Init();
        var request = new EventValue<int>(sender, index);
        UnityAction<bool> listener = value => { if (value) Trigger(name, request); };
        Binding(toggle).Replace(() => toggle.onValueChanged.RemoveListener(listener));
        toggle.onValueChanged.AddListener(listener);
    }

    public void Bind(Slider slider, string name, MonoBehaviour sender)
    {
        Init();
        UnityAction<float> listener = value => Trigger(name, new EventValue<float>(sender, value));
        Binding(slider).Replace(() => slider.onValueChanged.RemoveListener(listener));
        slider.onValueChanged.AddListener(listener);
    }

    public void Bind(Toggle toggle, string name)
    {
        Init();
        UnityAction<bool> listener = value => Trigger(name, value);
        Binding(toggle).Replace(() => toggle.onValueChanged.RemoveListener(listener));
        toggle.onValueChanged.AddListener(listener);
    }

    public void Bind(Dropdown dropdown, string name)
    {
        Init();
        UnityAction<int> listener = value => Trigger(name, value);
        Binding(dropdown).Replace(() => dropdown.onValueChanged.RemoveListener(listener));
        dropdown.onValueChanged.AddListener(listener);
    }

    private EventBinding Binding(Component control)
    {
        EventBinding binding = control.GetComponent<EventBinding>();
        if (binding == null) binding = control.gameObject.AddComponent<EventBinding>();
        return binding;
    }
}
