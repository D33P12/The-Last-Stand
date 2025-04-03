using UnityEngine;

[CreateAssetMenu(fileName = "MusicSettings", menuName = "ScriptableObjects/MusicSettings", order = 1)]
public class MusicSettings : ScriptableObject
{
    [Range(0f, 1f)] public float musicVolume = 0.5f;
}
