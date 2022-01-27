using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [Header("State Management")]
    public bool paused;
    public InputAction pauseAction;
    
    [Header("Pause State")]
    public bool time;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        pauseAction.Enable();
        pauseAction.started += context => HandlePauseState();
    }

    public void Start()
    {
        paused = false;
    }

    public void HandlePauseState()
    {
        if (!paused)
        {
            paused = true;
            OnPause();
        }
        else
        {
            paused = false;
            OnPauseExit();
        }
    }

    public void OnPause()
    {
        Debug.Log("Pause");
        Time.timeScale = 0;
    }

    public void OnPauseExit()
    {
        Debug.Log("Resume");
        Time.timeScale = 1;
    }
}
