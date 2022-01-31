using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager Instance;
    public float globalVolume = 1;

    public AudioSource backgroundMusicSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);

        GameObject backgroundMusicObject = new GameObject("Background Music");
        backgroundMusicObject.transform.parent = transform;
        backgroundMusicSource = backgroundMusicObject.AddComponent<AudioSource>();
        backgroundMusicSource.loop = true;

        foreach (Sound s in sounds)
        {
            if (s.clips == null) continue;
            GameObject newObject = new GameObject(s.name + " source");

            s.source = newObject.AddComponent<AudioSource>();

            newObject.transform.parent = transform;
        }
    }

    private void Update()
    {
        backgroundMusicSource.volume = globalVolume;
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound: " + name + " was not found");
            return;
        }
        s.Play();
    }

    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound: " + name + " was not found");
            return;
        }
        s.source.Stop();
    }

    public AudioSource FindSoundSource(Sound s)
    {
        return transform.GetChild(Array.FindIndex(sounds, sound => sound.name == s.name)).GetComponent<AudioSource>();
    }

    public void SetBackgroundMusic(AudioClip clip)
    {
        if(backgroundMusicSource.clip = clip) return; //Keep song playing
        backgroundMusicSource.clip = clip;
        backgroundMusicSource.Play();
    }
}
