using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignQuestOnStart : MonoBehaviour
{
    public Quest assign;

    private void Start() {
        QuestManager.Instance.AssignQuest(assign);
    }
}
