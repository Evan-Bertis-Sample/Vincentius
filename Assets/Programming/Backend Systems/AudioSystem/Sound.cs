using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
[CreateAssetMenu (menuName = "Sound")]
public class Sound : ScriptableObject
{
    public AudioClip[] clips;
    public bool loop;
    public CenteralizedRange volume;
    public CenteralizedRange pitch;

    [HideInInspector]
    public AudioSource source;

    public void Play()
    {
        if (clips == null)
        {
            Debug.LogError("No Audio Clips in " + name);
            return;
        }
        AudioClip clip = clips[Random.Range(0, clips.Length - 1)];

        source.clip = clip;
        source.volume = volume.Evaluate() * AudioManager.Instance.globalVolume;
        source.pitch = pitch.Evaluate();
        source.loop = loop;

        source.Play();
    }
}
