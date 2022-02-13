using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OptionsMenu : MonoBehaviour
{
    public Slider volumeSlider;

    private void Start() {
        volumeSlider.value = AudioManager.Instance.globalVolume;
    }

    public void SetGlobalVolume(float volume)
    {
        //AudioManager.Instance.backgroundMusicSource.DOKill();
        AudioManager.Instance.globalVolume = volume;
    }
}
