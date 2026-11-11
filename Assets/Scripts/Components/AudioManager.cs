using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Unity.VisualScripting.Member;

public class AudioManager : BaseManager<AudioManager>
{
    private float mainVolume;
    private float musicVolume;
    private float soundVolume;
    private AudioSource audioSource=null;
    private List<AudioSource> soundList=null;
    private GameObject Soundobj;
    private GameObject BGMobj;
    private AudioData audioData;
    public AudioManager()
    {
        LoadAudioData();
        soundList = new List<AudioSource>();
        MonoManager.Instance.AddUpdateEvent(Update);
    }
    public void PlayMusic(AudioClip clip, UnityAction action = null)
    {
        if (BGMobj == null)
        {
            BGMobj = new GameObject("BGM");
            audioSource=BGMobj.AddComponent<AudioSource>();
            GameObject.DontDestroyOnLoad(BGMobj);
        }
        else
            audioSource= BGMobj.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = musicVolume;
        audioSource.loop = true;
        audioSource.Play();
        action?.Invoke();
    }
    public void PauseMusic()
    {
        if(audioSource)
            audioSource.Pause();
    }
    public void ContinueMusic()
    {
        if (audioSource)
            audioSource.UnPause();
    }
    public void PlaySound(AudioClip clip)
    {
        if (Soundobj == null)
        {
            Soundobj = new GameObject("Sound");
            GameObject.DontDestroyOnLoad(Soundobj);
        }
        AudioSource source= Soundobj.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop= false;
        source.volume= soundVolume;
        source.Play();
        soundList.Add(source);
    }
    public void SetMainVolume(float volume)
    {
        mainVolume = volume;
        if (audioSource)
        {
            audioSource.volume = volume * mainVolume;
        }
        foreach (AudioSource source in soundList)
        {
            if (source)
                source.volume = volume * mainVolume;
        }
    }
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if(audioSource)
            audioSource.volume=volume*mainVolume;
    }
    public void SetSoundVolume(float volume)
    {
        soundVolume = volume;
        foreach (AudioSource source in soundList)
        {
            if (source)
                source.volume = volume*mainVolume;
        }
    }
    public void Update()
    {
        for (int i = soundList.Count - 1; i >= 0; i--)
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }

    public void LoadAudioData()
    {
        audioData = JsonManager.Instance.Load<AudioData>("AudioData");
        if (audioData == null)
            audioData = new AudioData();
        SetMainVolume(audioData.volumeMain);
        SetMusicVolume(audioData.volumeMusic);
        SetSoundVolume(audioData.volumeSound);
    }
    public void SaveAudioData()
    {
        audioData.volumeMain = mainVolume;
        audioData.volumeMusic = musicVolume;
        audioData.volumeSound = soundVolume;
        JsonManager.Instance.Save(audioData, "AudioData");
    }
    public void SetDefault()
    {
        SetMainVolume(1.0f);
        SetMusicVolume(1.0f);
        SetSoundVolume(1.0f);
    }
}
