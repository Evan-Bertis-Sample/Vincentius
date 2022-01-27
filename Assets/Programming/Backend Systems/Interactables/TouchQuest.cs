using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchQuest :Interactable
{
    public Quest assign;

    public override void OnContact()
    {
        QuestManager.Instance.AssignQuest(assign);
    }

    public override void OnExit()
    {

    }

    public override void OnHold()
    {

    }

}
