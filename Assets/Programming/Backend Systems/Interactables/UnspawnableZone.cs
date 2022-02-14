using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnspawnableZone : Interactable
{
    public GameObject overridePos;
    private Vector3 enterPos;

    public override void OnContact(GameObject player)
    {
        RespawnManager.Instance.updateSafePos = false;
        enterPos = player.transform.position;
        RespawnManager.Instance.overrideSafePos = (overridePos == null) ? enterPos : overridePos.transform.position;
    }

    public override void OnExit(GameObject player)
    {
        if (LevelManager.Instance.player.gameObject.activeInHierarchy == false) return;
        RespawnManager.Instance.updateSafePos = true;
        RespawnManager.Instance.overrideSafePos = Vector3.zero;
    }

    public override void OnHold(GameObject player)
    {
        RespawnManager.Instance.updateSafePos = false;
        RespawnManager.Instance.overrideSafePos = (overridePos == null) ? enterPos : overridePos.transform.position;
    }
}
