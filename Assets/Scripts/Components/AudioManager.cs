using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : BaseManager<AudioManager>
{
    private bool isMusicMute;
    private bool isSoundMute;
    private float musicVolume=1.0f;
    private float soundVolume=1.0f;
    private AudioSource audioSource=null;
    private List<AudioSource> soundList=null;
    private GameObject Soundobj;
    private GameObject BGMobj;
    public AudioManager()
    {
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
        audioSource.mute = isMusicMute;
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
    public void SetMusicIsMute(bool mute)
    {
        isMusicMute = mute;
        if (audioSource)
            audioSource.mute = mute;
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
        source.mute= isSoundMute;
        source.volume= soundVolume;
        source.Play();
        soundList.Add(source);
    }
    public void SetSoundIsMute(bool mute)
    {
        isSoundMute = mute;
        foreach (AudioSource source in soundList)
        {
            if (source)
                source.mute = mute;
        }
    }
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if(audioSource)
            audioSource.volume=volume;
    }
    public void SetSoundVolume(float volume)
    {
        soundVolume = volume;
        foreach (AudioSource source in soundList)
        {
            if (source)
                source.volume = volume;
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
}
