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
    public Vector3 overrideSafePos;
    public int numFrames;
    public float safeTimeOnGround = 0.5f;
    public bool updateSafePos;

    public string deathSound;
    public GameObject deathParticles;

    public delegate void OnLife();
    public static OnLife OnDeath;

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
        Debug.Log("Killing Player");
        updateSafePos = false;
        player.canMove = false;
        player.gameObject.SetActive(false);
        GameObject particles = Instantiate(deathParticles, player.transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySound(deathSound);
        ScreenFader.Instance.FadeSceneSpeed(1, fadeSpeed, -1, int.MaxValue).OnComplete(() =>
        {
            Vector3 spawnPoint = (overrideSafePos == Vector3.zero) ? lastSafePos : overrideSafePos;
            Debug.Log($"Spawning at point {spawnPoint}");
            player.transform.position = spawnPoint;
            player.canMove = true;
            updateSafePos = true;
            player.gameObject.SetActive(true);
            Destroy(particles);

            OnDeath?.Invoke();
            
            ScreenFader.Instance.ResetFaderSpeed(fadeSpeed, int.MaxValue);
        });
    }
}
