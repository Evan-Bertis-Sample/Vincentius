using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Start()
    {
        ScreenFader.Instance.FadeScene(0.5f, 1f);
    }

    public void LoadScene(string sceneName)
    {
        Level newLevel = LevelManager.Instance.FindLevel(sceneName);
        LevelManager.Instance.TransitionLevel(newLevel, "Default");
    }
}
