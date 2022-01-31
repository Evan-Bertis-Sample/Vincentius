using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchQuest :Interactable
{
    public Quest assign;

    public override void OnContact(GameObject player)
    {
        QuestManager.Instance.AssignQuest(assign);
    }

    public override void OnExit(GameObject player)
    {

    }

    public override void OnHold(GameObject player)
    {

    }

}
