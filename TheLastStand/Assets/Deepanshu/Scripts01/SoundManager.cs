using UnityEngine;

public class SoundManager : MonoBehaviour
{ 
    public enum AudioType
    {
        BG
    }
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClip[] audioList;
    private AudioSource audioSource;
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
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (musicSettings != null)
        {
            audioSource.volume = musicSettings.musicVolume;
        }
    }
    public void PlayAudio(AudioType sound, float volume = 0.25f)
    {
        if (audioList.Length > (int)sound && audioList[(int)sound] != null)
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(audioList[(int)sound], volume);
        }
    }
    public void PlayAudioContinuous(AudioType sound, float volume = 0.5f)
    {
        if (audioList.Length > (int)sound && audioList[(int)sound] != null)
        {
            audioSource.clip = audioList[(int)sound];
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }
    public void StopPlaying()
    {
        audioSource.loop = false;
        audioSource.Stop();
    }
    public void SetMusicVolume(float volume)
    {
        if (musicSettings != null)
        {
            musicSettings.musicVolume = volume;
            audioSource.volume = volume;
        }
    }
}
