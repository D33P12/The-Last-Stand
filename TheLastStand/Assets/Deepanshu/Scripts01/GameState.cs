using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class GameState 
{
    [FormerlySerializedAs("Timer")] public float timer;
    [FormerlySerializedAs("AimSensitivity")] public float aimSensitivity;
    [FormerlySerializedAs("MusicVolume")] public float musicVolume;
    [FormerlySerializedAs("IsEventSystemActive")] public bool isEventSystemActive;
    public GameState(float timer, float aimSensitivity, float musicVolume, bool isEventSystemActive)
    {
        this.timer = timer;
        this.aimSensitivity = aimSensitivity;
        this.musicVolume = musicVolume;
        this.isEventSystemActive = isEventSystemActive;
    }
}
