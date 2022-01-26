using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnTouch : Interactable
{
    public override void OnContact()
    {
        RespawnManager.Instance.KillPlayer();
    }

    public override void OnExit()
    {

    }

    public override void OnHold()
    {

    }

}
