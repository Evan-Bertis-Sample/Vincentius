using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager Instance;
    public float globalVolume = 1;
    public bool transitioningBackgroundMusic;

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
        if(transitioningBackgroundMusic) return;

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

        if (s.source != null)
        {
            if (s.source.isPlaying) return;
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
        if(backgroundMusicSource.clip == clip) return; //Keep song playing

        //Fade song
        transitioningBackgroundMusic = true;
        backgroundMusicSource.DOKill();
        backgroundMusicSource.DOFade(0, ScreenFader.Instance.fadeTime).OnComplete(() =>
        {
            backgroundMusicSource.clip = clip;
            Debug.Log("Playing Background Music");
            backgroundMusicSource.Play();
            backgroundMusicSource.DOFade(globalVolume, ScreenFader.Instance.fadeTime).OnComplete(() => transitioningBackgroundMusic = false);
        });
    }
}
