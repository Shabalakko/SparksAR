using UnityEngine;

public class AudioSourceType : MonoBehaviour
{
    public enum AudioType
    {
        Music,
        Effect
    }

    public AudioType audioType;
}