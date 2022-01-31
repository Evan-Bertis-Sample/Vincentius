using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomOnTouch : Interactable
{
    public float zoomAmount = 4f;
    public float zoomSpeed = 1f;

    public override void OnContact(GameObject player)
    {
        CameraZoom.Main.ZoomCamera(zoomAmount, zoomSpeed);
    }

    public override void OnExit(GameObject player)
    {
        CameraZoom.Main.ResetZoom(zoomSpeed);
    }

    public override void OnHold(GameObject player)
    {

    }
}
