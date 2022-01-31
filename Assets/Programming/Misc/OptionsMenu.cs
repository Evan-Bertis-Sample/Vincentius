using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider volumeSlider;

    private void Start() {
        volumeSlider.value = AudioManager.Instance.globalVolume;
    }

    public void SetGlobalVolume(float volume)
    {
        AudioManager.Instance.globalVolume = volume;
    }
}
