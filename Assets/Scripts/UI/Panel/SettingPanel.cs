using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    public Button btnBack;
    public Button btnSure;
    public Button btnDefault;
    public Slider sliderMain;
    public Slider sliderMusic;
    public Slider sliderSound;
    public Text txtMain;
    public Text txtMusic;
    public Text txtSound;
    public Dropdown dropWindow;
    public Dropdown dropRate;
    public Toggle togDamage;
    public Toggle togShake;
    private bool isInit;
    private BasePanel lastpanel;
    protected override void Init()
    {
        if (isInit)
            return;
        EventCallBack.Instance.Bind(btnBack, nameof(EventCallBack.CancelSettings));
        EventCallBack.Instance.Bind(btnDefault, nameof(EventCallBack.ResetSettings), this);
        EventCallBack.Instance.Bind(btnSure, nameof(EventCallBack.SaveSettings));
        EventCallBack.Instance.Bind(sliderMain, nameof(EventCallBack.ChangeMainVolume), this);
        EventCallBack.Instance.Bind(sliderMusic, nameof(EventCallBack.ChangeMusicVolume), this);
        EventCallBack.Instance.Bind(sliderSound, nameof(EventCallBack.ChangeSoundVolume), this);
        EventCallBack.Instance.Bind(togDamage, nameof(EventCallBack.ChangeDamageDisplay));
        EventCallBack.Instance.Bind(togShake, nameof(EventCallBack.ChangeCameraShake));
        EventCallBack.Instance.Bind(dropWindow, nameof(EventCallBack.ChangeWindowMode));
        EventCallBack.Instance.Bind(dropRate, nameof(EventCallBack.ChangeResolution));
        isInit = true;
    }

    public override void Show()
    {
        base.Show();
        RefreshAudioUI();
    }

    internal void RefreshAudioUI()
    {
        AudioManager audio = AudioManager.Instance;
        float main = audio.GetMainVolume();
        float music = audio.GetMusicVolume();
        float sound = audio.GetSoundVolume();
        sliderMain.SetValueWithoutNotify(main);
        sliderMusic.SetValueWithoutNotify(music);
        sliderSound.SetValueWithoutNotify(sound);
        txtMain.text = (main * 100).ToString("F2") + "%";
        txtMusic.text = (music * 100).ToString("F2") + "%";
        txtSound.text = (sound * 100).ToString("F2") + "%";
    }
}
