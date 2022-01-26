using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;
    public PlayerController player;
    public float fadeSpeed = 0.25f;

    public Vector3 lastSafePos;
    public Queue<Vector3> safePos;
    public int numFrames;
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
        safePos = new Queue<Vector3>();
    }

    private void Update() 
    {
        if(player.timeOnGround >= safeTimeOnGround && updateSafePos)
        {
            if (safePos.Count> numFrames - 1)
            {
                safePos.Dequeue();
            }

            safePos.Enqueue(player.transform.position);
            lastSafePos = safePos.Peek();
        }
    }

    public void KillPlayer()
    {
        updateSafePos = false;
        player.canMove = false;
        ScreenFader.Instance.FadeSceneSpeed(1, fadeSpeed).OnComplete(() =>
        {
            player.transform.position = lastSafePos;
            player.canMove = true;
            updateSafePos = true;
            ScreenFader.Instance.FadeSceneSpeed(0, fadeSpeed);
        });
    }
}
