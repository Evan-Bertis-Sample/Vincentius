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
    public bool canEnter = true;
    private bool hovering;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        hovering = false;
        LevelManager.OnSceneChange += newScene => 
        {
            Debug.Log("Adding doorway");
            LevelManager.Instance.sceneDoors.Add(this);
        };
        OnStart();
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

    public virtual void OnStart()
    {

    }

    public virtual void OnDoorwayHover()
    {
        //Debug.Log($"Hovering over doorway {doorwayID}");
    }

    public virtual void OnDoorwayEnter()
    {
        if (canEnter == false) return;
        //Debug.Log("Entering Doorway");
        LevelManager.Instance.TransitionLevel(nextLevel, nextDoorwayID);
    }

    public virtual void OnDoorwayLeave()
    {
        
    }

    public virtual void OnDoorwayExit()
    {
        Level level = LevelManager.Instance.FindLevel(SceneManager.GetActiveScene().name);
        if (level == null)
        {
            Debug.Log("Level was not found");
            return;
        }
        
        bool visited = (LevelManager.Instance.FindVisitedLevel(level.sceneName) != null);
        //Debug.Log($"{level} was { ((visited) ? "visited" : " not visited") }");
        if (visited) return;


        string displayName = (level.levelName != "") ? level.levelName : level.sceneName;
        if (level.displayNotification) NotificationManager.Instance.RequestNotification(new Notification(displayName, "Area", 1f), 0);
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
            OnDoorwayLeave();
        }
    }
}
