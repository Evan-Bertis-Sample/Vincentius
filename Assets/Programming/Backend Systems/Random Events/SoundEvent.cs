using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Random Event/Sound Event")]
public class SoundEvent : RandomEvent
{
    public string soundName = "Chirp";
    public override void InitEvent()
    {
        AudioManager.Instance.PlaySound(soundName);
    }

    public override void EndEvent()
    {
        //AudioManager.Instance.StopSound(soundName);
    }
}
