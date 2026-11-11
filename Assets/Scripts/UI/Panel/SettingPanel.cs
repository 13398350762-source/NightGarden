using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
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
    protected override void Init()
    {
        if (isInit)
            return;
        btnBack.onClick.AddListener(() =>{
            AudioManager.Instance.LoadAudioData();
            UIManager.Instance.HidePanel<SettingPanel>();
            UIManager.Instance.ShowPanel<MainPanel>();
        });
        btnDefault.onClick.AddListener(() =>{
            AudioManager.Instance.SetDefault();
            sliderMain.value = 1.0f;
            sliderMusic.value = 1.0f;
            sliderSound.value = 1.0f;
        });
        btnSure.onClick.AddListener(() =>{
            AudioManager.Instance.SaveAudioData();
        });
        sliderMain.onValueChanged.AddListener((value) =>{
            AudioManager.Instance.SetMainVolume(value);
            txtMain.text = (value * 100).ToString("F2") + "%";
        });
        sliderMusic.onValueChanged.AddListener((value) => {
            AudioManager.Instance.SetMusicVolume(value);
            txtMusic.text = (value * 100).ToString("F2") + "%";
        });
        sliderSound.onValueChanged.AddListener((value) => {
            AudioManager.Instance.SetSoundVolume(value);
            txtSound.text = (value * 100).ToString("F2") + "%";
        });
        togDamage.onValueChanged.AddListener((value) =>{

        });
        togShake.onValueChanged.AddListener((value) =>{

        });
        dropWindow.onValueChanged.AddListener((value) =>{ 
        
        });
        dropRate.onValueChanged.AddListener((value) =>{ 
        
        });
        isInit = true;
    }
}
