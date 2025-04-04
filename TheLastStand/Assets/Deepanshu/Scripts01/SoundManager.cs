using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum AudioType
    {
        Bg, 
        Reloading,
        Walking,
        EnemyDamaged,
        shoot,
        ObstacleDamaged
    } 
    public static SoundManager Instance { get; private set; }
    public AudioClip[] audioList;
    private AudioSource _audioSource;
    public MusicSettings musicSettings;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (_audioSource != null)
        {
            _audioSource.volume = musicSettings.musicVolume;

            if (audioList.Length > 0 && audioList[(int)AudioType.Bg] != null)
            {
                PlayAudioContinuous(AudioType.Bg, musicSettings.musicVolume);
            }
        }
    }
    public void PlayAudioContinuous(AudioType sound, float volume)
    {
        if (_audioSource != null && audioList.Length > (int)sound && audioList[(int)sound] != null)
        {
            _audioSource.clip = audioList[(int)sound];
            _audioSource.loop = true;
            _audioSource.volume = volume;
            _audioSource.Play();
        }
    }
    public void PlayAudioOnce(AudioType sound, float volume)
    {
        if (_audioSource != null && audioList.Length > (int)sound && audioList[(int)sound] != null)
        {
            _audioSource.clip = audioList[(int)sound];
            _audioSource.loop = false;
            _audioSource.volume = volume;
            _audioSource.Play();
        }
    }
    public void SetMusicVolume(float volume)
    {
        if (_audioSource != null)
        {
            musicSettings.musicVolume = volume;
            _audioSource.volume = volume;
        }
    }
}
