using System;
using UnityEngine;

public enum SoundType
{
    NONE,
    Swim,
    Fish,
    Camera
}

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [SerializeField] public AudioClip sound;
}


[ExecuteInEditMode]
public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private SoundList[] soundList;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource bgmAudioSource;

    public static void PlaySound(SoundType sound, float volume = 1f)
    {
        instance.sfxAudioSource.PlayOneShot(instance.soundList[(int)sound].sound, volume);
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
}