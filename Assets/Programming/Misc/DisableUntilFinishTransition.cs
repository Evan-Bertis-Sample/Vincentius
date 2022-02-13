using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableUntilFinishTransition : MonoBehaviour
{
    public List<GameObject> toEnable = new List<GameObject>();
    LevelManager.SceneChange SceneChange;

    void Awake()
    {
        SetEnable(false);
        SceneChange = new LevelManager.SceneChange(sceneName =>
        {
            SetEnable(true);
            LevelManager.OnSceneLateChange -= SceneChange;
        });
    }
    void Start()
    {
        LevelManager.OnSceneLateChange += SceneChange;   
    }

    private void SetEnable(bool enabled)
    {
        foreach(GameObject g in toEnable)
        {
            g.SetActive(enabled);
        }
    }
}
