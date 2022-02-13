using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowCamera : MonoBehaviour
{
    public Vector2 offset = Vector2.zero;
    public float zPos = 0;
    private Camera mainCamera;
    public string originalSceneName;

    public bool lockX = false;
    public bool lockY = true;
    private Vector2 originalPos;

    public bool destroyOnSceneChange = true;

    private LevelManager.SceneChange SceneChange;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        transform.position = mainCamera.transform.position + (Vector3)offset;
        transform.position = new Vector3(transform.position.x, transform.position.y, zPos);
        transform.parent = mainCamera.transform;
        originalSceneName = SceneManager.GetActiveScene().name;
        originalPos = transform.position;

        SceneChange = new LevelManager.SceneChange((sceneName => DestroyThis()));

        LevelManager.OnSceneChange += SceneChange;
    }

    private void Update() 
    {
        if (!lockX && !lockY) return;

        Vector2 pos = new Vector2(
            (lockX) ? originalPos.x : transform.position.x,
            (lockY) ? originalPos.y : transform.position.y
        );

        transform.position = pos;
    }

    private void OnDestroy() {
        if (gameObject == null) return;
        LevelManager.OnSceneChange -= SceneChange;
    }

    private void DestroyThis()
    {
        if (!destroyOnSceneChange) return;
        bool shouldDestroy = (LevelManager.Instance.activeLevel.sceneName != originalSceneName);
        Debug.Log($"{gameObject.name} : {shouldDestroy}");
        if (LevelManager.Instance.activeLevel.sceneName != originalSceneName && GetComponent<PersistentObject>() == null) Destroy(gameObject);
    }
}
