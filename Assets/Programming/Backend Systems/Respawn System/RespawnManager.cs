using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;
    public PlayerController player;
    public Vector3 lastSafePos;
    public float safeTimeOnGround = 0.5f;
    public bool updateSafePos;

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
        DontDestroyOnLoad(gameObject);
    }

    private void Start() 
    {
        lastSafePos = player.transform.position;
        updateSafePos = true;
    }

    private void Update() 
    {
        if(player.timeOnGround >= safeTimeOnGround && updateSafePos)
        {
            lastSafePos = player.transform.position;
        }
    }

    public void KillPlayer()
    {
        updateSafePos = false;
        player.canMove = false;
        ScreenFader.Instance.FadeScene(1).OnComplete(() =>
        {
            player.transform.position = lastSafePos;
            player.canMove = true;
            updateSafePos = true;
            ScreenFader.Instance.FadeScene(0);
        });
    }
}
