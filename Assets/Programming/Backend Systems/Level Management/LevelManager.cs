using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public List<Level> levels = new List<Level>();
    public List<Level> visitedLevels = new List<Level>();
    public Transform player;

    public InputAction enterAction;
    public bool enterActionInit;

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
        Debug.Log("Transitioning to Scene: " + level.sceneName);
        AsyncOperation nextLevelOperation = SceneManager.LoadSceneAsync(level.sceneName);
        nextLevelOperation.allowSceneActivation = false;
        Tween screenFade = ScreenFader.Instance.FadeScene(1);
        bool fadeComplete = false;

        screenFade.OnComplete(() => fadeComplete = true);

        yield return new WaitUntil(() => fadeComplete && nextLevelOperation.progress >= 0.9f); //Wait until the screen fade is complete, and the next level is done loading

        nextLevelOperation.allowSceneActivation = true;
        //We are now in the next level
        visitedLevels.Add(FindLevel(level.sceneName));

        //We need to find the doorways
        List<LevelDoorway> sceneDoors = FindObjectsOfType<LevelDoorway>().ToList();

        LevelDoorway toDoor = FindDoorway(sceneDoors, doorWayID);
        if (toDoor == null) FindDoorway(sceneDoors, level.defaultDoorwayID);
        if (toDoor == null)
        {
            Debug.LogWarning($"Could not find level doorway: {doorWayID} or default doorway {level.defaultDoorwayID}");
            toDoor = sceneDoors[0];
        }

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
