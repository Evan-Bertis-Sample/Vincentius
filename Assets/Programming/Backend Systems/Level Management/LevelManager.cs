using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using UnityEngine.InputSystem;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public List<Level> levels = new List<Level>();
    public List<Level> visitedLevels = new List<Level>();
    public Transform player;

    public List<GameObject> persistentObjects;

    public List<LevelDoorway> sceneDoors = new List<LevelDoorway>();

    public InputAction enterAction;
    public bool enterActionInit;

    public delegate void SceneChange(string newScene);
    public static event SceneChange OnSceneChange;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
        enterAction.Enable();
        enterAction.started += (context) => enterActionInit = true;
        persistentObjects = new List<GameObject>();
    }

    private void Start() {
        if(FindVisitedLevel(SceneManager.GetActiveScene().name) == null)
        {
            visitedLevels.Add(FindLevel(SceneManager.GetActiveScene().name));
        }
    }

    private void LateUpdate() {
        enterActionInit = false;
    }

    public void EnablePersistency(GameObject gameObject)
    {
        if(persistentObjects.Any(p => p.name == gameObject.name))
        {
            //There is a persistent object with the same name already
            //Destroy duplicate
            Destroy(gameObject);
            return;
        }

        //There is no persistent object with name
        persistentObjects.Add(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    

    public Level FindLevel(string scene)
    {
        return levels.Where((l) => l.sceneName == scene).FirstOrDefault();
    }

    public Level FindVisitedLevel(string scene)
    {
        return visitedLevels.Where((l) => l.sceneName== scene).FirstOrDefault();
    }

    public void TransitionLevel(Level level, string doorwayID)
    {
        StartCoroutine(TransitionLevelStart(level, doorwayID));
    }

    public IEnumerator TransitionLevelStart(Level level, string doorWayID)
    {
        string previousSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Transitioning to Scene: " + level.sceneName + " from Scene: " + previousSceneName);
        AsyncOperation nextLevelOperation = SceneManager.LoadSceneAsync(level.sceneName);
        nextLevelOperation.allowSceneActivation = false;
        Tween screenFade = ScreenFader.Instance.FadeScene(1);
        bool fadeComplete = false;

        screenFade.OnComplete(() => fadeComplete = true);

         //Wait until the screen fade is complete, and the next level is done loading
        while (!nextLevelOperation.isDone && !fadeComplete)
        {
            yield return null;
        }
        nextLevelOperation.allowSceneActivation = true;

        yield return null;
        
        //We are now in the next level
        visitedLevels.Add(FindLevel(level.sceneName));
        sceneDoors.Clear();
        OnSceneChange?.Invoke(level.sceneName); //Will also prompt level doors to add themselves to level manager
        sceneDoors = FindObjectsOfType<LevelDoorway>().ToList();

        /*
        yield return new WaitUntil(() => {
            sceneDoors = sceneDoors.Where(s => s != null).ToList();
            return (sceneDoors.Count == (FindObjectsOfType<LevelDoorway>().Count()));
        }); //Wait for doors
        */

        LevelDoorway toDoor = FindDoorway(sceneDoors, doorWayID);
        if (toDoor == null) FindDoorway(sceneDoors, level.defaultDoorwayID);
        if (toDoor == null)
        {
            Debug.LogWarning($"Could not find level doorway: {doorWayID} or default doorway {level.defaultDoorwayID}");
            toDoor = sceneDoors[0];
        }

        Debug.Log(toDoor.transform.position);
        player.position = toDoor.transform.position;
        toDoor.OnDoorwayExit();

        screenFade = ScreenFader.Instance.FadeScene(0);

        //yield return new WaitUntil(() => screenFade.IsComplete());
    }

    public LevelDoorway FindDoorway(List<LevelDoorway> doorways, string find)
    {
        return doorways.Where((d) => d.doorwayID == find).FirstOrDefault();
    }
}
