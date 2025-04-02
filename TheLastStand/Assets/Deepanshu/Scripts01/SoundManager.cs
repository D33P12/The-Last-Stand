using UnityEngine;

public class SoundManager : MonoBehaviour
{ 
    public enum AudioType
    {
        Bg
    }
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClip[] audioList;
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
        if (musicSettings != null)
        {
            _audioSource.volume = musicSettings.musicVolume;
        }
    }
    public void PlayAudio(AudioType sound, float volume = 0.25f)
    {
        if (audioList.Length > (int)sound && audioList[(int)sound] != null)
        {
            _audioSource.loop = false;
            _audioSource.PlayOneShot(audioList[(int)sound], volume);
        }
    }
    public void PlayAudioContinuous(AudioType sound, float volume = 0.5f)
    {
        if (audioList.Length > (int)sound && audioList[(int)sound] != null)
        {
            _audioSource.clip = audioList[(int)sound];
            _audioSource.loop = true;
            _audioSource.volume = volume;
            _audioSource.Play();
        }
    }
    public void StopPlaying()
    {
        _audioSource.loop = false;
        _audioSource.Stop();
    }
    public void SetMusicVolume(float volume)
    {
        if (musicSettings != null)
        {
            musicSettings.musicVolume = volume;
            _audioSource.volume = volume;
            Debug.Log("Music volume set to: " + volume); // Debug log
        }
    }
}
