using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using UnityEngine.InputSystem;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public List<Level> levels = new List<Level>();
    public List<Level> visitedLevels = new List<Level>();
    public Transform player;
    public Level activeLevel;
    public bool transitioning;

    public List<GameObject> persistentObjects;

    public List<LevelDoorway> sceneDoors = new List<LevelDoorway>();

    public InputAction enterAction;
    public bool enterActionInit;

    public PolygonCollider2D levelBoundary;
    public CinemachineConfiner confiner;

    public delegate void SceneChange(string newScene);
    public static event SceneChange OnSceneChange;
    public static event SceneChange OnSceneLateChange;

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
        levelBoundary.isTrigger = true;

        //confiner = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineConfiner>();
    }

    private void Start() {
        if(FindVisitedLevel(SceneManager.GetActiveScene().name) == null)
        {
            Level start = FindLevel(SceneManager.GetActiveScene().name);
            visitedLevels.Add(start);
            player.gameObject.SetActive(start.playerActive);
            activeLevel = start;
            AudioManager.Instance.SetBackgroundMusic(start.backgroundMusic);
            if (start.daySettings != null) TimeOfDayManager.Instance.SetTimeOfDay(start.daySettings);
        }

        confiner = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineConfiner>();
    }

    private void LateUpdate() {
        enterActionInit = false;
    }

    public void EnablePersistency(GameObject gameObject)
    {
        persistentObjects = persistentObjects.Where(p => p.gameObject != null).ToList();
        if(persistentObjects.Any(p => p.name == gameObject.name))
        {
            //There is a persistent object with the same name already
            //Destroy duplicate
            Debug.Log("Deleted Duplicate: " + gameObject.name);
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
        if (transitioning) return;
        StartCoroutine(TransitionLevelStart(level, doorwayID));
    }

    public IEnumerator TransitionLevelStart(Level level, string doorWayID)
    {
        transitioning = true;
        string previousSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Transitioning to Scene: " + level.sceneName + " from Scene: " + previousSceneName);
        AsyncOperation nextLevelOperation = SceneManager.LoadSceneAsync(level.sceneName);
        nextLevelOperation.allowSceneActivation = false;
        Tween screenFade = ScreenFader.Instance.FadeScene(1,-1, int.MaxValue);
        bool fadeComplete = false;

        screenFade.OnComplete(() => fadeComplete = true);

         //Wait until the screen fade is complete, and the next level is done loading
        while (!nextLevelOperation.isDone && !fadeComplete)
        {
            yield return null;
        }
        CameraZoom.Main.ResetZoom(0.2f);
        FollowOffset.main.ResetPosition();
        Debug.Log("Completed");
        activeLevel = level;
        nextLevelOperation.allowSceneActivation = true;

        yield return null;
        
        //We are now in the next level
        sceneDoors.Clear();
        OnSceneChange?.Invoke(level.sceneName); //Will also prompt level doors to add themselves to level manager
        if (level.daySettings != null) 
        {
            TimeOfDayManager.Instance.SetTimeOfDay(level.daySettings);
            screenFade = ScreenFader.Instance.FadeScene(level.daySettings.fadeAmount, 1, int.MaxValue);
            fadeComplete = false;
            screenFade.OnComplete(() => fadeComplete = true);
        }
        else
        {
            screenFade = ScreenFader.Instance.FadeScene(0,-1, 3);
            fadeComplete = false;
            screenFade.OnComplete(() => fadeComplete = true);
        }

        yield return new WaitUntil(() => {
            sceneDoors = sceneDoors.Where(s => s != null).ToList();
            
            List<LevelDoorway> actualDoorways = FindObjectsOfType<LevelDoorway>().ToList();
            actualDoorways = actualDoorways.Where(g => g.isActiveAndEnabled == true).ToList();

            Debug.Log($"Doorway Count : {actualDoorways.Count}");
            return (sceneDoors.Count == actualDoorways.Count);
        }); //Wait for doors

        LevelDoorway toDoor = FindDoorway(sceneDoors, doorWayID);
        if (toDoor == null) FindDoorway(sceneDoors, level.defaultDoorwayID);
        if (toDoor == null)
        {
            Debug.LogWarning($"Could not find level doorway: {doorWayID} or default doorway {level.defaultDoorwayID}");
            toDoor = (sceneDoors.Count > 0) ? sceneDoors[0] : null;
        }

        //Debug.Log(toDoor.transform.position);
        player.gameObject.SetActive(level.playerActive);

        if (toDoor != null)
        {
            player.position = toDoor.transform.position;
            Debug.Log("Teleporting To Door : " + toDoor);
            toDoor.OnDoorwayExit();
        }

        transitioning = false;
        Debug.Log($"Transitioning: {transitioning}");
        AudioManager.Instance.SetBackgroundMusic(level.backgroundMusic);

        yield return null;

        if (!visitedLevels.Contains(level))
        {
            visitedLevels.Add(FindLevel(level.sceneName));
        }

        yield return new WaitUntil(() => fadeComplete == true);
        
        OnSceneLateChange?.Invoke(level.sceneName);
    }

    public LevelDoorway FindDoorway(List<LevelDoorway> doorways, string find)
    {
        if (doorways == null) return null;
        if (doorways.Count == 0) return null;
        return doorways.Where((d) => d.doorwayID == find).FirstOrDefault();
    }

    public void SetLevelBoundary(PolygonCollider2D col)
    {
        if (confiner == null)
        {
            confiner = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineConfiner>();
        }
        confiner.m_BoundingShape2D = col;

        /*
        List<Vector2> points = new List<Vector2>();
        foreach(Vector2 pt in col.points)
        {
            points.Add(col.transform.TransformPoint(pt));
        }
        levelBoundary.points = points.ToArray();
        */
    }
}
