using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDoorway : MonoBehaviour
{
    public string doorwayID;
    public Level nextLevel;
    public string nextDoorwayID;
    public bool requireInput = true;
    private bool hovering;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        hovering = false;
        LevelManager.OnSceneChange += newScene => 
        {
            Debug.Log("Adding doorway");
            //LevelManager.Instance.sceneDoors.Add(this);
        };
    }

    private void Update()
    {
        if (hovering)
        {
            OnDoorwayHover();
            //Debug.Log(LevelManager.Instance.enterActionInit);
            if (LevelManager.Instance.enterActionInit && requireInput)
            {
                OnDoorwayEnter();
            }
        }
    }

    public virtual void OnDoorwayHover()
    {
        //Debug.Log($"Hovering over doorway {doorwayID}");
    }

    public void OnDoorwayEnter()
    {
        //Debug.Log("Entering Doorway");
        LevelManager.Instance.TransitionLevel(nextLevel, nextDoorwayID);
    }

    public virtual void OnDoorwayExit()
    {
        NotificationManager.Instance.RequestNotification(new Notification(SceneManager.GetActiveScene().name, "Area", 1f), 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == LevelManager.Instance.player.gameObject)
        {
            hovering = true;
            if (requireInput == false)
            {
                OnDoorwayEnter();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == LevelManager.Instance.player.gameObject)
        {
            hovering = false;
        }
    }
}
