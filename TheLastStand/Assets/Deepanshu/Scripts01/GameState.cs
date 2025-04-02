using UnityEngine;

[System.Serializable]
public class GameState 
{
    public float Timer;
    public float AimSensitivity;
    public float MusicVolume;
    public bool IsEventSystemActive;

    public GameState(float timer, float aimSensitivity, float musicVolume, bool isEventSystemActive)
    {
        Timer = timer;
        AimSensitivity = aimSensitivity;
        MusicVolume = musicVolume;
        IsEventSystemActive = isEventSystemActive;
    }
}
