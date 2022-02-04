using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnspawnableZone : Interactable
{
    public override void OnContact(GameObject player)
    {
        RespawnManager.Instance.updateSafePos = false;
        RespawnManager.Instance.overrideSafePos = player.transform.position;
    }

    public override void OnExit(GameObject player)
    {
        RespawnManager.Instance.updateSafePos = true;
        RespawnManager.Instance.overrideSafePos = Vector3.zero;
    }

    public override void OnHold(GameObject player)
    {
        
    }
}
