using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnTouch : Interactable
{
    public override void OnContact(GameObject player)
    {
        RespawnManager.Instance.KillPlayer();
    }

    public override void OnExit(GameObject player)
    {

    }

    public override void OnHold(GameObject player)
    {

    }

}
