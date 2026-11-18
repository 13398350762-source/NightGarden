using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AudioManager : BaseManager<AudioManager>
{
    private const float musicGain = 0.55f;
    private float mainVolume;
    private float musicVolume;
    private float soundVolume;
    private AudioSource musicSource;
    private AudioSource soundSource;
    private AudioData audioData;
    private readonly AudioLibrary library;
    private readonly Dictionary<SoundCue, float> lastSoundTimes = new Dictionary<SoundCue, float>();

    public AudioManager()
    {
        library = Resources.Load<AudioLibrary>("AudioLibrary");
        if (library == null)
            Debug.LogWarning("找不到 AudioLibrary 音频配置");
        LoadAudioData();
        SceneManager.activeSceneChanged += (_, scene) => PlaySceneMusic(scene.name);
    }

    public void PlaySceneMusic(string sceneName)
    {
        switch (sceneName)
        {
            case "LoginScene": PlayMusic(MusicTrack.Login); break;
            case "BeginScene": PlayMusic(MusicTrack.Menu); break;
            case "GameScene": PlayMusic(MusicTrack.Battle); break;
        }
    }

    public void PlayMusic(MusicTrack track)
    {
        if (library != null)
            PlayMusic(library.GetMusic(track));
    }

    public void PlayMusic(AudioClip clip, UnityAction action = null)
    {
        if (clip == null) return;
        if (musicSource == null)
        {
            musicSource = CreateSource("BGM");
            musicSource.loop = true;
        }
        if (musicSource.clip != clip || !musicSource.isPlaying)
        {
            musicSource.clip = clip;
            ApplyVolume();
            musicSource.Play();
        }
        action?.Invoke();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PauseMusic()
    {
        if (musicSource != null)
            musicSource.Pause();
    }

    public void ContinueMusic()
    {
        if (musicSource != null)
            musicSource.UnPause();
    }

    public void PlaySound(SoundCue cue, float gain = 1f, float minInterval = 0f)
    {
        if (library == null || mainVolume <= 0f || soundVolume <= 0f) return;
        float now = Time.unscaledTime;
        if (lastSoundTimes.TryGetValue(cue, out float lastTime) && now - lastTime < minInterval)
            return;
        AudioClip clip = library.GetSound(cue);
        if (clip == null) return;
        lastSoundTimes[cue] = now;
        PlaySound(clip, gain);
    }

    public void PlaySound(AudioClip clip)
    {
        PlaySound(clip, 1f);
    }

    private void PlaySound(AudioClip clip, float gain)
    {
        if (clip == null) return;
        if (soundSource == null)
        {
            soundSource = CreateSource("Sound");
            ApplyVolume();
        }
        soundSource.PlayOneShot(clip, gain);
    }

    private static AudioSource CreateSource(string name)
    {
        var obj = new GameObject(name);
        var source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        GameObject.DontDestroyOnLoad(obj);
        return source;
    }

    public float GetMainVolume() => mainVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSoundVolume() => soundVolume;

    public void SetMainVolume(float volume)
    {
        mainVolume = Mathf.Clamp01(volume);
        ApplyVolume();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolume();
    }

    public void SetSoundVolume(float volume)
    {
        soundVolume = Mathf.Clamp01(volume);
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        if (musicSource != null)
            musicSource.volume = mainVolume * musicVolume * musicGain;
        if (soundSource != null)
            soundSource.volume = mainVolume * soundVolume;
    }

    public void LoadAudioData()
    {
        audioData = JsonManager.Instance.Load<AudioData>("AudioData") ?? new AudioData();
        mainVolume = Mathf.Clamp01(audioData.volumeMain);
        musicVolume = Mathf.Clamp01(audioData.volumeMusic);
        soundVolume = Mathf.Clamp01(audioData.volumeSound);
        ApplyVolume();
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
        mainVolume = musicVolume = soundVolume = 1f;
        ApplyVolume();
    }
}
