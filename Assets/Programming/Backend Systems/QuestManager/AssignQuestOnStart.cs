using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignQuestOnStart : MonoBehaviour
{
    public Quest assign;

    public LevelManager.SceneChange SceneChange;
    private void Start() {
        SceneChange = new LevelManager.SceneChange(sceneName => 
        {
            QuestManager.Instance.AssignQuest(assign);
            Debug.Log($"Assigning Quest: {assign.questName}");
        });
        LevelManager.OnSceneChange += SceneChange;
        LevelManager.OnSceneLateChange -= SceneChange;
    }
}
