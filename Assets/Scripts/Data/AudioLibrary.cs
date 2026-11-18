using UnityEngine;

public enum MusicTrack
{
    Login,
    Menu,
    Battle,
    Boss,
    Shop
}

public enum SoundCue
{
    Button,
    PlayerHurt,
    EnemyHit,
    EnemyDeath,
    Coin,
    Upgrade,
    WaveComplete,
    BossArrival,
    GameOver,
    Slash,
    Bow,
    Explosion,
    Denied
}

public class AudioLibrary : ScriptableObject
{
    [SerializeField] private AudioClip[] music;
    [SerializeField] private AudioClip[] sounds;

    public AudioClip GetMusic(MusicTrack track) => music[(int)track];
    public AudioClip GetSound(SoundCue cue) => sounds[(int)cue];
}
