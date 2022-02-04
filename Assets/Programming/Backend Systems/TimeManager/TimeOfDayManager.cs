using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDayManager : MonoBehaviour
{
    public static TimeOfDayManager Instance;
    public TimeOfDay current;

    public GameObject sky;
    private SpriteRenderer skySr;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        skySr = sky.GetComponent<SpriteRenderer>();
        DontDestroyOnLoad(gameObject);
    }

    public void SetTimeOfDay(TimeOfDay settings)
    {
        if (current == settings) return;

        current = settings;

        ScreenFader.Instance.SetAlpha(settings.fadeAmount, 150);

        skySr.sharedMaterial.SetColor("SkyBase", settings.baseSkyColor);
        skySr.sharedMaterial.SetColor("SkyShadow", settings.shadowSkyColor);
    }
}
