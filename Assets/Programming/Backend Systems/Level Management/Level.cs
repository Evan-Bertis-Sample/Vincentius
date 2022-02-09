using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[CreateAssetMenu(menuName = "Level")]
public class Level : ScriptableObject
{
    public string levelName;
    public string sceneName;
    public AudioClip backgroundMusic;
    public string defaultDoorwayID;
    public bool playerActive = true;
    public TimeOfDay daySettings;

    public bool displayNotification = true;
}
